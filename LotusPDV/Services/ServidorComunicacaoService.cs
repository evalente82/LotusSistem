using Aplicacao.DTOs;
using Dominio.Entidades;
using Microsoft.Extensions.Configuration;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace LotusPDV.Services
{
    public class ServidorComunicacaoService
    {
        private readonly string _ipServidor;
        private readonly int _portaServidor;

        public ServidorComunicacaoService(IConfiguration configuration)
        {
            _ipServidor = configuration["PdvSettings:EnderecoServidor"];
            _portaServidor = int.Parse(configuration["PdvSettings:PortaServidor"]);
        }

        public async Task<List<Produto>> ObterProdutosAsync()
        {
            Console.WriteLine($"[PDV] Tentando obter produtos do servidor em {_ipServidor}:{_portaServidor}...");
            try
            {
                var request = new ComunicacaoRequest
                {
                    Comando = "OBTER_PRODUTOS",
                    PayloadJson = ""
                };

                var response = await EnviarRequisicaoAsync(request);

                if (response.Sucesso)
                {
                    var produtos = JsonSerializer.Deserialize<List<Produto>>(response.PayloadJson);
                    Console.WriteLine($"[PDV] Sucesso! {produtos.Count} produtos recebidos do servidor.");
                    return produtos;
                }
                else
                {
                    var erroMsg = response.MensagemErro ?? "Erro desconhecido ao obter produtos do servidor.";
                    Console.WriteLine($"[PDV] O servidor respondeu com um erro: {erroMsg}");
                    throw new Exception(erroMsg);
                }
            }
            catch (Exception ex)
            {
                // Este é o log mais importante para erros de conexão!
                Console.WriteLine($"[PDV] FALHA CRÍTICA de comunicação ao obter produtos: {ex.ToString()}");
                return null;
            }
        }

        private async Task<ComunicacaoResponse> EnviarRequisicaoAsync(ComunicacaoRequest request)
        {
            using var client = new TcpClient();
            await client.ConnectAsync(_ipServidor, _portaServidor);

            using var stream = client.GetStream();

            // Envia a requisição
            string jsonRequest = JsonSerializer.Serialize(request);
            byte[] messageBuffer = Encoding.UTF8.GetBytes(jsonRequest);
            byte[] lengthBuffer = BitConverter.GetBytes(messageBuffer.Length);
            await stream.WriteAsync(lengthBuffer, 0, 4);
            await stream.WriteAsync(messageBuffer, 0, messageBuffer.Length);

            // Lê a resposta
            byte[] lengthBufferResponse = new byte[4];
            await stream.ReadExactlyAsync(lengthBufferResponse, 0, 4);
            int messageLength = BitConverter.ToInt32(lengthBufferResponse, 0);

            byte[] messageBufferResponse = new byte[messageLength];
            await stream.ReadExactlyAsync(messageBufferResponse, 0, messageLength);

            string jsonResponse = Encoding.UTF8.GetString(messageBufferResponse);
            return JsonSerializer.Deserialize<ComunicacaoResponse>(jsonResponse);
        }

        // Adicione este método dentro da classe ServidorComunicacaoService
        public async Task<bool> RegistrarVendaAsync(Venda venda) // Recebe Venda como antes
        {
            Console.WriteLine($"[PDV] Tentando registrar a venda {venda.Id} no servidor...");
            try
            {
                // --- MUDANÇA CRÍTICA AQUI ---
                // Criamos o DTO manualmente, garantindo que apenas os IDs e valores
                // sejam enviados, não o objeto 'Produto' inteiro.
                var vendaDto = new RegistrarVendaDto
                {
                    Id = venda.Id,
                    IdPDV = venda.IdPDV,
                    MomentoDaVendaUTC = venda.MomentoDaVendaUTC,
                    Itens = venda.Itens.Select(item => new ItemVendaDto
                    {
                        IdProduto = item.IdProduto, // Apenas o ID do produto!
                        Quantidade = item.Quantidade,
                        PrecoUnitario = item.PrecoUnitario
                    }).ToList()
                };

                var request = new ComunicacaoRequest
                {
                    Comando = "REGISTRAR_VENDA",
                    PayloadJson = JsonSerializer.Serialize(vendaDto) // Enviamos o DTO simples
                };

                var response = await EnviarRequisicaoAsync(request);

                if (response.Sucesso)
                {
                    Console.WriteLine($"[PDV] Venda {venda.Id} registrada no servidor com sucesso!");
                }
                else
                {
                    Console.WriteLine($"[PDV] Servidor retornou erro ao registrar venda: {response.MensagemErro}");
                }

                return response.Sucesso;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[PDV] FALHA CRÍTICA de comunicação ao registrar venda: {ex.ToString()}");
                return false;
            }
        }
    }
}