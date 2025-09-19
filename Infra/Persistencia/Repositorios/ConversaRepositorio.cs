using Aplicacao.Interfaces;
using Dominio.Entidades;
using Infra.Persistencia.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infra.Persistencia.Repositorios
{
    public class ConversaRepositorio : IConversaRepositorio
    {
        private readonly ServidorDbContext _context;

        public ConversaRepositorio(ServidorDbContext context)
        {
            _context = context;
        }

        public async Task<Conversa> CriarConversaAsync(Conversa novaConversa)
        {
            await _context.Conversas.AddAsync(novaConversa);
            await _context.SaveChangesAsync();
            return novaConversa;
        }

        public async Task<List<Conversa>> ObterTodasConversasAsync()
        {
            return await _context.Conversas.OrderByDescending(c => c.DataCriacao).ToListAsync();
        }

        public async Task<Conversa?> ObterConversaPorIdAsync(int id)
        {
            return await _context.Conversas.FindAsync(id);
        }
    }
}