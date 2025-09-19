using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration; // Este using já estava aqui
using System.IO;

namespace Infra.Persistencia.DbContexts
{
    /// <summary>
    /// Esta classe ensina as ferramentas de linha de comando do Entity Framework (como Add-Migration)
    /// a criar uma instância do ServidorDbContext sem precisar executar o projeto MAUI.
    /// Ela resolve o erro "executável não é um aplicativo válido".
    /// </summary>
    public class ServidorDbContextFactory : IDesignTimeDbContextFactory<ServidorDbContext>
    {
        public ServidorDbContext CreateDbContext(string[] args)
        {
            // O objetivo aqui é ler o appsettings.json para pegar a string de conexão.
            // Para isso, precisamos encontrar o caminho para o projeto principal.
            var basePath = Directory.GetCurrentDirectory();
            // Este caminho relativo assume que o comando está sendo executado a partir da pasta 'Infra'
            var configurationPath = Path.Combine(basePath, "../LotusServidorGerencial/appsettings.json");

            if (!File.Exists(configurationPath))
            {
                // Fallback para o caso de o caminho relativo não funcionar
                configurationPath = Path.Combine(basePath, "appsettings.json");
                if (!File.Exists(configurationPath))
                {
                    throw new FileNotFoundException($"Não foi possível encontrar o arquivo de configuração 'appsettings.json'. Verifique o caminho em ServidorDbContextFactory.cs e se o arquivo está configurado para ser copiado para o diretório de saída no projeto LotusServidorGerencial.");
                }
            }

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(configurationPath))
                // Agora o AddJsonFile será encontrado
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("ServidorDb");

            var optionsBuilder = new DbContextOptionsBuilder<ServidorDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            return new ServidorDbContext(optionsBuilder.Options);
        }
    }
}