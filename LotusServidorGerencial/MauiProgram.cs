using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Infra.Persistencia.DbContexts;
using Microsoft.EntityFrameworkCore;
using Aplicacao.Interfaces;
using Infra.Persistencia.Repositorios;
using Microsoft.Extensions.DependencyInjection;
using System;
using LotusServidorGerencial.Services;

namespace LotusServidorGerencial
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var config = configurationBuilder.Build();

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Configuration.AddConfiguration(config);
            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            // --- INJEÇÃO DE DEPENDÊNCIA ---
            var connectionString = config.GetConnectionString("ServidorDb");
            builder.Services.AddDbContext<ServidorDbContext>(options =>
                options.UseNpgsql(connectionString));

            builder.Services.AddScoped<DbContext, ServidorDbContext>();
            builder.Services.AddScoped<IProdutoRepositorio, ProdutoRepositorio>();
            builder.Services.AddScoped<IVendaRepositorio, VendaRepositorio>();

            // --- MUDANÇA PRINCIPAL AQUI ---
            // Apenas registramos o serviço como Singleton. Não usamos mais IHostedService.
            builder.Services.AddSingleton<TcpServerService>();

            var app = builder.Build();

            // --- INÍCIO MANUAL DO SERVIÇO ---
            // Após o app ser construído, pegamos nosso serviço do container
            // e mandamos ele iniciar.
            var tcpService = app.Services.GetRequiredService<TcpServerService>();
            tcpService.Start();

            return app;
        }
    }
}