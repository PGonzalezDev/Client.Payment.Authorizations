namespace Authorizations.Reverser.Worker.Services
{
    public class ClientPaymentAuthorizationService : IClientPaymentAuthorizationService
    {
        private readonly ILogger<ClientPaymentAuthorizationService> _logger;
        private readonly HttpClient _client;
        private readonly string _reverseAuthorizationRoute;

        public ClientPaymentAuthorizationService(
            ILogger<ClientPaymentAuthorizationService> logger,
            IConfiguration config
        )
        {
            _logger = logger;

            string baseUrl = config["ClientPaymentAuthorizationHost"];
            
            _client = new HttpClient();
            _client.BaseAddress = new Uri(baseUrl);
            _client.DefaultRequestHeaders.Add("x-api-key", config["ApiKey"]);
            _client.DefaultRequestHeaders.Add("x-api-token", config["ApiToken"]);

            _reverseAuthorizationRoute = config["ReverseAuthorizationRoute"];
        }

        public async Task<bool> ReverseAuthorizationPost()
        {
            try
            {
                HttpResponseMessage response = await _client.PostAsync(_reverseAuthorizationRoute, null);

                _logger.LogInformation($"Http post '//reverse-auth' is Success status code: {response.IsSuccessStatusCode}");
                _logger.LogInformation($"Http post '//reverse-auth' status code: {response.StatusCode}");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception error msg: {ex.Message}");
            }

            return false;
        }
    }
}
