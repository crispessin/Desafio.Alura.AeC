using Desafio.Alura.AeC.Domain.Conteudo;

namespace Desafio.Alura.AeC.Application
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly string _url;
        private readonly string _termo;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;

            _url = configuration?.GetValue<string>("UrlSite") ?? throw new ArgumentException("Propriedade 'UrlSite' não encontrada no appsettings.json");
            _termo = configuration?.GetValue<string>("Termo") ?? throw new ArgumentException("Propriedade 'Termo' não encontrada no appsettings.json");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Consume Scoped Service Hosted Service running.");

            await DoWork(stoppingToken);
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Consume Scoped Service Hosted Service is working.");

                using IServiceScope scope = _serviceProvider.CreateScope();

                var coletor = scope.ServiceProvider.GetRequiredService<IConteudoColetor>();

                coletor.ColetarEArmazenar(_url, _termo);

                await Task.Delay(10000, stoppingToken);
            }
        }

    }
}