using Aplicacao.Interfaces;
using Dominio.Entidades;
using Infra.Persistencia.DbContexts;
using Microsoft.EntityFrameworkCore;
namespace Infra.Persistencia.Repositorios
{
    public class ChatHistoricoRepositorio : IChatHistoricoRepositorio
    {
        private readonly ServidorDbContext _context;

        public ChatHistoricoRepositorio(ServidorDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarMensagemAsync(ChatHistorico mensagem)
        {
            await _context.ChatHistorico.AddAsync(mensagem);
            await _context.SaveChangesAsync();
        }

        // Método atualizado para filtrar por conversa
        public async Task<List<ChatHistorico>> ObterHistoricoDaConversaAsync(int conversaId, int limite = 100)
        {
            return await _context.ChatHistorico
                .Where(m => m.ConversaId == conversaId) // A filtragem acontece aqui
                .OrderByDescending(m => m.Timestamp)
                .Take(limite)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
        }

        public async Task LimparHistoricoAntigoAsync(DateTime dataLimite)
        {
            await _context.ChatHistorico
                .Where(m => m.Timestamp < dataLimite)
                .ExecuteDeleteAsync();
        }
    }
}