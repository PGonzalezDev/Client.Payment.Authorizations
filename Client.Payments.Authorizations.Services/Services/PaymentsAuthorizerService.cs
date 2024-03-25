using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;

namespace Client.Payments.Authorizations.Services;

public class PaymentsAuthorizerService : IPaymentsAuthorizerService
{
    private readonly ILogger<PaymentsAuthorizerService> _logger;
    private readonly HttpClient _client;
    private readonly string _paymentApprovalRoute;

    public PaymentsAuthorizerService(
        ILogger<PaymentsAuthorizerService> logger,
        IConfiguration config
    )
    {
        _logger = logger;

        string baseUrl = config["PaymentsAuthorizerHost"].ToString();
        _client = new HttpClient();
        _client.BaseAddress = new Uri(baseUrl);

        _paymentApprovalRoute = config["PaymentApprovalRoute"];
    }

    public async Task<bool> PaymentApproval(decimal amount)
    {
        try
        {
            var json = new { amount = amount };
            JsonContent jsonContent = JsonContent.Create(json);
            HttpResponseMessage response = await _client.PostAsync(_paymentApprovalRoute, jsonContent);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(responseContent);
                var jsonElement = jsonDoc.RootElement.GetProperty("approved");

                bool approved = jsonElement.GetBoolean();

                _logger.LogInformation($"Http post '//payment-approval' response: {responseContent}");

                return approved;
            }
            else
            {
                _logger.LogError($"Http post '//payment-approval' response status code: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception error msg: {ex.Message}");
        }

        return false;
    }
}
