using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Infra.Persistencia.DbContexts;
using Microsoft.EntityFrameworkCore;
using Aplicacao.Interfaces;
using Infra.Persistencia.Repositorios;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Aplicacao.DTOs;
using System.Text.Json;
using Dominio.Entidades;
using System.Linq;
using Microsoft.Extensions.Hosting;

namespace LotusServidorGerencial
{
    public static class MauiProgram
    {
        public static IConfiguration Configuration { get; private set; }

        public static MauiApp CreateMauiApp()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = configurationBuilder.Build();

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            builder.Configuration.AddConfiguration(Configuration);

            // --- INJEÇÃO DE DEPENDÊNCIA (mesma de antes) ---
            var connectionString = Configuration.GetConnectionString("ServidorDb");
            builder.Services.AddDbContext<ServidorDbContext>(options =>
                options.UseNpgsql(connectionString));

            builder.Services.AddScoped<DbContext, ServidorDbContext>();
            builder.Services.AddScoped<IProdutoRepositorio, ProdutoRepositorio>();
            builder.Services.AddScoped<IVendaRepositorio, VendaRepositorio>();

            // --- NOVO SERVIÇO DE SERVIDOR TCP ---
            builder.Services.AddSingleton<TcpServerService>();
            builder.Services.AddSingleton<IHostedService>(provider => provider.GetRequiredService<TcpServerService>());


            return builder.Build();
        }
    }

    // Classe que implementa nosso servidor de comunicação TCP direto.
    public class TcpServerService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private TcpListener _listener;
        private CancellationTokenSource _cancellationTokenSource;

        public TcpServerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => ListenForClients(_cancellationTokenSource.Token));
            return Task.CompletedTask;
        }

        private async Task ListenForClients(CancellationToken token)
        {
            try
            {
                _listener = new TcpListener(IPAddress.Any, 8888); // Escuta em todas as interfaces de rede na porta 8888
                _listener.Start();
                Console.WriteLine("Servidor TCP iniciado na porta 8888. Aguardando conexões...");

                while (!token.IsCancellationRequested)
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync(token);
                    Console.WriteLine("Cliente conectado!");
                    // Dispara uma nova Task para lidar com o cliente, sem bloquear o loop principal
                    _ = HandleClientAsync(client, token);
                }
            }
            catch (OperationCanceledException)
            {
                // Esperado quando o servidor é parado
                Console.WriteLine("Servidor TCP parado.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro crítico no servidor TCP: {ex.Message}");
            }
            finally
            {
                _listener?.Stop();
            }
        }

        private async Task HandleClientAsync(TcpClient client, CancellationToken token)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var produtoRepo = scope.ServiceProvider.GetRequiredService<IProdutoRepositorio>();
                    var vendaRepo = scope.ServiceProvider.GetRequiredService<IVendaRepositorio>();
                    var networkStream = client.GetStream();

                    // Lógica para ler a requisição do PDV
                    var request = await ReadRequestAsync(networkStream);
                    ComunicacaoResponse response = new ComunicacaoResponse { Sucesso = false, MensagemErro = "Comando desconhecido." };

                    // Processa o comando
                    if (request.Comando == "OBTER_PRODUTOS")
                    {
                        var produtos = await produtoRepo.ObterTodosAsync();
                        response.PayloadJson = JsonSerializer.Serialize(produtos);
                        response.Sucesso = true;
                    }
                    else if (request.Comando == "REGISTRAR_VENDA")
                    {
                        var vendaDto = JsonSerializer.Deserialize<RegistrarVendaDto>(request.PayloadJson);
                        var novaVenda = new Venda
                        {
                            Id = vendaDto.Id,
                            IdPDV = vendaDto.IdPDV,
                            MomentoDaVendaUTC = vendaDto.MomentoDaVendaUTC,
                            Itens = vendaDto.Itens.Select(i => new ItemVenda { Id = Guid.NewGuid(), IdProduto = i.IdProduto, Quantidade = i.Quantidade, PrecoUnitario = i.PrecoUnitario }).ToList()
                        };
                        novaVenda.ValorTotal = novaVenda.Itens.Sum(i => i.Quantidade * i.PrecoUnitario);
                        await vendaRepo.AdicionarVendaAsync(novaVenda);
                        response.Sucesso = true;
                    }

                    // Envia a resposta de volta para o PDV
                    await SendResponseAsync(networkStream, response);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao lidar com o cliente: {ex.Message}");
            }
            finally
            {
                client.Close();
                Console.WriteLine("Cliente desconectado.");
            }
        }

        // Métodos auxiliares para ler e escrever na rede de forma segura
        private async Task<ComunicacaoRequest> ReadRequestAsync(NetworkStream stream)
        {
            byte[] lengthBuffer = new byte[4];
            await stream.ReadExactlyAsync(lengthBuffer, 0, 4);
            int messageLength = BitConverter.ToInt32(lengthBuffer, 0);

            byte[] messageBuffer = new byte[messageLength];
            await stream.ReadExactlyAsync(messageBuffer, 0, messageLength);

            string json = Encoding.UTF8.GetString(messageBuffer);
            return JsonSerializer.Deserialize<ComunicacaoRequest>(json);
        }

        private async Task SendResponseAsync(NetworkStream stream, ComunicacaoResponse response)
        {
            string json = JsonSerializer.Serialize(response);
            byte[] messageBuffer = Encoding.UTF8.GetBytes(json);
            byte[] lengthBuffer = BitConverter.GetBytes(messageBuffer.Length);

            await stream.WriteAsync(lengthBuffer, 0, 4);
            await stream.WriteAsync(messageBuffer, 0, messageBuffer.Length);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource?.Cancel();
            _listener?.Stop();
            return Task.CompletedTask;
        }
    }
}