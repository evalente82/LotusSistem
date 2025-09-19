using Dominio.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aplicacao.Interfaces
{
    public interface IChatHistoricoRepositorio
    {
        Task AdicionarMensagemAsync(ChatHistorico mensagem);
        // Agora precisamos saber de qual conversa obter o histórico
        Task<List<ChatHistorico>> ObterHistoricoDaConversaAsync(int conversaId, int limite = 100);
        Task LimparHistoricoAntigoAsync(System.DateTime dataLimite);
    }
}