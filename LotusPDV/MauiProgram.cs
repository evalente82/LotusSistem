using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.IO;
using Infra.Persistencia.DbContexts;
using Microsoft.EntityFrameworkCore;
using Aplicacao.Interfaces;
using Infra.Persistencia.Repositorios;
using LotusPDV.Services; // Adicione este using
using Microsoft.Extensions.DependencyInjection; // Adicione este using se não existir

namespace LotusPDV
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
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

            var appSettingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            var config = new ConfigurationBuilder().AddJsonFile(appSettingsPath).Build();
            builder.Configuration.AddConfiguration(config);

            // --- INJEÇÃO DE DEPENDÊNCIA PARA O PDV ---
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "lotuspdv.db");
            builder.Services.AddDbContext<PdvDbContext>(options =>
                options.UseSqlite($"DataSource={dbPath}"));

            builder.Services.AddScoped<DbContext, PdvDbContext>();
            builder.Services.AddScoped<IProdutoRepositorio, ProdutoRepositorio>();
            builder.Services.AddScoped<IVendaRepositorio, VendaRepositorio>();

            builder.Services.AddSingleton<ServidorComunicacaoService>();

            // --- MUDANÇA PRINCIPAL AQUI ---
            // Apenas registramos o serviço como Singleton.
            builder.Services.AddSingleton<SincronizacaoService>();


            var app = builder.Build();

            // --- INÍCIO MANUAL DO SERVIÇO DE SINCRONIZAÇÃO ---
            // Após o app ser construído, pegamos nosso serviço e mandamos ele iniciar.
            var syncService = app.Services.GetRequiredService<SincronizacaoService>();
            syncService.Start();

            return app;
        }
    }
}