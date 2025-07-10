using Aplicacao.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LotusPDV.Services
{
    // Versão simplificada sem IHostedService
    public class SincronizacaoService : IDisposable
    {
        private readonly IServiceProvider _serviceProvider;
        private Timer _timer;

        public SincronizacaoService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Start()
        {
            Console.WriteLine("[Sincronizacao] Serviço de sincronização em background iniciado.");
            // Inicia o timer para verificar a cada 30 segundos
            _timer = new Timer(FazerTrabalho, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(30));
        }

        private void FazerTrabalho(object state)
        {
            Console.WriteLine("[Sincronizacao] Verificando se há vendas para sincronizar...");
            // Dispara a tarefa de sincronização sem esperar (para não bloquear o timer)
            _ = SincronizarDadosAsync();
        }

        private async Task SincronizarDadosAsync()
        {
            // Usamos um escopo para obter instâncias "novas" dos nossos serviços
            using (var scope = _serviceProvider.CreateScope())
            {
                var vendaRepoLocal = scope.ServiceProvider.GetRequiredService<IVendaRepositorio>();
                var comunicacaoService = scope.ServiceProvider.GetRequiredService<ServidorComunicacaoService>();

                // 1. SINCRONIZAR VENDAS
                var vendasParaSincronizar = await vendaRepoLocal.ObterVendasNaoSincronizadasAsync();

                if (vendasParaSincronizar.Any())
                {
                    Console.WriteLine($"[Sincronizacao] {vendasParaSincronizar.Count()} venda(s) encontrada(s) para sincronizar.");
                    var idsSincronizadosComSucesso = new List<Guid>();

                    foreach (var venda in vendasParaSincronizar)
                    {
                        // Passamos o objeto 'venda' completo para o serviço de comunicação
                        bool sucesso = await comunicacaoService.RegistrarVendaAsync(venda);
                        if (sucesso)
                        {
                            idsSincronizadosComSucesso.Add(venda.Id);
                        }
                    }

                    if (idsSincronizadosComSucesso.Any())
                    {
                        await vendaRepoLocal.MarcarVendasComoSincronizadasAsync(idsSincronizadosComSucesso);
                        Console.WriteLine($"[Sincronizacao] {idsSincronizadosComSucesso.Count} venda(s) marcadas como sincronizadas no banco local.");
                    }
                }
                else
                {
                    Console.WriteLine("[Sincronizacao] Nenhuma venda nova para sincronizar.");
                }

                // 2. SINCRONIZAR PRODUTOS (a lógica que já funcionava)
                try
                {
                    Console.WriteLine("[Sincronizacao] Verificando catálogo de produtos...");
                    var produtosDoServidor = await comunicacaoService.ObterProdutosAsync();

                    if (produtosDoServidor != null)
                    {
                        var produtoRepoLocal = scope.ServiceProvider.GetRequiredService<IProdutoRepositorio>();
                        await produtoRepoLocal.SincronizarCatalogoAsync(produtosDoServidor);
                        Console.WriteLine("[Sincronizacao] Catálogo de produtos local atualizado com sucesso.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Sincronizacao] Falha ao sincronizar catálogo de produtos: {ex.Message}");
                }
            }
        }
        public void Stop()
        {
            Console.WriteLine("[Sincronizacao] Serviço de sincronização parado.");
            _timer?.Change(Timeout.Infinite, 0);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}