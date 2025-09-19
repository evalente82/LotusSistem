using Aplicacao.Interfaces;
using Infra.Persistencia.DbContexts;
using Infra.Persistencia.Repositorios;
using LotusServidorGerencial.Services;
using LotusSistem.IA.Interfaces;
using LotusSistem.IA.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

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

            // --- ADICIONE A LINHA ABAIXO ---
            // Registra o serviço de IA como Singleton para carregar o modelo apenas uma vez.
            builder.Services.AddSingleton<ILLMService, Phi3MiniLLMService>();


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