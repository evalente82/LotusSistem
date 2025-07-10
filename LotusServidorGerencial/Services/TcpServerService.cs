

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Aplicacao.DTOs;
using Aplicacao.Interfaces;
using Dominio.Entidades;

namespace LotusServidorGerencial.Services
{
    // Versão simplificada sem IHostedService
    public class TcpServerService : IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private TcpListener _listener;
        private CancellationTokenSource _cancellationTokenSource;

        public TcpServerService(IServiceProvider serviceProvider)
        {
            Console.WriteLine("[Servidor-DIAGNÓSTICO] O CONSTRUTOR do TcpServerService foi chamado.");
            _serviceProvider = serviceProvider;
        }

        public void Start()
        {
            Console.WriteLine("[Servidor-DIAGNÓSTICO] O MÉTODO Start foi chamado.");
            _cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() => ListenForClients(_cancellationTokenSource.Token));
            Console.WriteLine("[Servidor-DIAGNÓSTICO] A Task para ouvir clientes foi INICIADA.");
        }

        private async Task ListenForClients(CancellationToken token)
        {
            Console.WriteLine("[Servidor-DIAGNÓSTICO] EXECUTANDO dentro da Task. Tentando criar o TcpListener...");
            try
            {
                _listener = new TcpListener(IPAddress.Any, 8888);
                _listener.Start();
                Console.WriteLine("[Servidor] SERVIDOR REALMENTE INICIADO na porta 8888. Aguardando conexões...");

                while (!token.IsCancellationRequested)
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync(token);
                    _ = HandleClientAsync(client, token);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Servidor] ERRO CRÍTICO ao iniciar o listener: {ex.ToString()}");
            }
            finally
            {
                _listener?.Stop();
            }
        }

        // O método HandleClientAsync e os métodos auxiliares continuam exatamente iguais
        private async Task HandleClientAsync(TcpClient client, CancellationToken token)
        {
            var clientEndPoint = client.Client.RemoteEndPoint?.ToString() ?? "desconhecido";
            Console.WriteLine($"[Servidor] Cliente conectado de {clientEndPoint}. Aguardando requisição...");
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                using (var networkStream = client.GetStream())
                {
                    var produtoRepo = scope.ServiceProvider.GetRequiredService<IProdutoRepositorio>();
                    var vendaRepo = scope.ServiceProvider.GetRequiredService<IVendaRepositorio>();
                    var request = await ReadRequestAsync(networkStream);
                    Console.WriteLine($"[Servidor] Comando recebido: '{request.Comando}' do cliente {clientEndPoint}.");
                    ComunicacaoResponse response = new ComunicacaoResponse { Sucesso = false, MensagemErro = "Comando desconhecido." };
                    if (request.Comando == "OBTER_PRODUTOS")
                    {
                        var produtos = await produtoRepo.ObterTodosAsync();
                        response.PayloadJson = JsonSerializer.Serialize(produtos);
                        response.Sucesso = true;
                        Console.WriteLine($"[Servidor] Sucesso! {produtos.Count()} produtos encontrados para enviar.");
                    }
                    else if (request.Comando == "REGISTRAR_VENDA")
                    {
                        var vendaDto = JsonSerializer.Deserialize<RegistrarVendaDto>(request.PayloadJson);

                        var momentoDaVendaUtcCorrigido = DateTime.SpecifyKind(vendaDto.MomentoDaVendaUTC, DateTimeKind.Utc);

                        var novaVenda = new Venda
                        {
                            Id = vendaDto.Id,
                            IdPDV = vendaDto.IdPDV,
                            MomentoDaVendaUTC = momentoDaVendaUtcCorrigido,
                            SincronizadoEmUTC = DateTime.UtcNow,
                            Itens = vendaDto.Itens.Select(itemDto => new ItemVenda
                            {
                                Id = Guid.NewGuid(),
                                IdProduto = itemDto.IdProduto, // <-- O servidor usa este ID para ligar ao seu produto
                                Quantidade = itemDto.Quantidade,
                                PrecoUnitario = itemDto.PrecoUnitario
                            }).ToList()
                        };
                        novaVenda.ValorTotal = novaVenda.Itens.Sum(i => i.Quantidade * i.PrecoUnitario);

                        await vendaRepo.AdicionarVendaAsync(novaVenda);

                        response.Sucesso = true;
                        Console.WriteLine($"[Servidor] Sucesso! Venda do PDV '{novaVenda.IdPDV}' registrada.");
                    }
                    await SendResponseAsync(networkStream, response);
                    Console.WriteLine($"[Servidor] Resposta enviada para o cliente {clientEndPoint}.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Servidor] ERRO ao lidar com o cliente {clientEndPoint}: {ex.ToString()}");
            }
            finally
            {
                client.Close();
                Console.WriteLine($"[Servidor] Conexão com o cliente {clientEndPoint} encerrada.");
            }
        }

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

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
        }

        public void Dispose()
        {
            Stop();
        }
    }
}