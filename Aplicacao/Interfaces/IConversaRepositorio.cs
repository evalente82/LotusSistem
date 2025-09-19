using Dominio.Entidades;

namespace Aplicacao.Interfaces
{
    public interface IConversaRepositorio
    {
        Task<Conversa> CriarConversaAsync(Conversa novaConversa);
        Task<List<Conversa>> ObterTodasConversasAsync();
        Task<Conversa?> ObterConversaPorIdAsync(int id);
    }
}