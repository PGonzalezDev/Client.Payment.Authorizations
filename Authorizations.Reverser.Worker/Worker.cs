using Authorizations.Reverser.Worker.Services;

namespace Authorizations.Reverser.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IClientPaymentAuthorizationService _clientPaymentAuthorizationService;
        private readonly int _milisecondsDelay;

        public Worker(
            ILogger<Worker> logger,
            IConfiguration configuration,
            IClientPaymentAuthorizationService clientPaymentAuthorizationService
        )
        {
            _logger = logger;
            _configuration = configuration;
            _clientPaymentAuthorizationService = clientPaymentAuthorizationService;


            bool parsed = int.TryParse(_configuration["SetWorkerMinutesDelay"], out int delayMinutes);
            delayMinutes = parsed ? delayMinutes : 5;

            _milisecondsDelay = (delayMinutes * 60000);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

                await _clientPaymentAuthorizationService.ReverseAuthorizationPost();

                await Task.Delay(_milisecondsDelay, stoppingToken);
            }
        }
    }
}
