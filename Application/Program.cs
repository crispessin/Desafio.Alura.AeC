using Desafio.Alura.AeC.Application;
using Desafio.Alura.AeC.Domain.Conteudo;
using Desafio.Alura.AeC.Infrastructure.Persistence.DbContexts;
using Desafio.Alura.AeC.Infrastructure.Persistence.Repositories;
using Desafio.Alura.AeC.Infrastructure.Scrapers.Selenium;
using Microsoft.EntityFrameworkCore;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services
        .AddDbContext<AppDbContext>(options =>
            options
                .UseNpgsql(hostContext.Configuration.GetConnectionString("DbConnection"))
                .UseSnakeCaseNamingConvention(), ServiceLifetime.Singleton)
        .AddScoped<IConteudoRepository, ConteudoRepository>()
        .AddScoped<IConteudoScraper, ConteudoChromeScraper>()
        .AddScoped<IConteudoColetor, ConteudoColetor>()
        .AddHostedService<Worker>();
    })
    .Build();

// Aplicando migração de dados
using var scope = host.Services.CreateScope();

var services = scope.ServiceProvider;
var dbContext = services.GetRequiredService<AppDbContext>();

dbContext.Database.Migrate();

host.Run();