using Client.Payments.Authorizations.Models;
using Client.Payments.Authorizations.Services.Repositories;
using Microsoft.Extensions.Configuration;

namespace Client.Payments.Authorizations.Services;

public class ReverseAuthorizationsService : IReverseAuthorizationsService
{
    private readonly IAuthorizationRepository _repository;
    private readonly IClientRepository _clientRepository;
    private readonly IConfiguration _config;

    public ReverseAuthorizationsService(
        IAuthorizationRepository repository,
        IClientRepository clientRepository,
        IConfiguration config
    )
    {
        _repository = repository;
        _clientRepository = clientRepository;
        _config = config;
    }

    public async Task ReverseAuthorizationsProcessAsync()
    {
        IEnumerable<Guid> clientIds = await _clientRepository.GetClientIdsByAuthorizationMode(
            ClientAuthModeEnum.APPROVE_AND_CONFIRM
        );

        foreach (Guid clientId in clientIds)
        {
            IEnumerable<Models.Authorization> toConfirmAuth = await _repository.GetDailyAuthorizationsToConfirmAsync(clientId);

            if (toConfirmAuth != null && toConfirmAuth.Any())
            {
                int minutes = int.Parse(_config["SetConfirmMinutes"] ?? "5");
                var expiredAuth = toConfirmAuth
                        .Where(x => DateTime.Now.Subtract(x.CreatedDate).Minutes > minutes)
                        .ToList();

                await UnconfirmExpiredAuthorizations(expiredAuth);
                await ReverseExpiredAuthorizations(expiredAuth);
            }
        }
    }

    public virtual async Task UnconfirmExpiredAuthorizations(IEnumerable<Models.Authorization> expiredAuth)
    {
        if (expiredAuth != null && expiredAuth.Any())
        {
            expiredAuth.ToList().ForEach(x => x.Confirmed = false);

            await _repository.UpdateRangeAsync(expiredAuth);
        }
    }

    public virtual async Task ReverseExpiredAuthorizations(IEnumerable<Models.Authorization> expiredAuth)
    {
        if (expiredAuth != null && expiredAuth.Any())
        {
            IEnumerable<Models.Authorization> reversed = expiredAuth.Select(x => new Models.Authorization()
            {
                Id = Guid.Empty,
                Amount = x.Amount,
                ClientId = x.ClientId,
                CreatedDate = DateTime.Now,
                AuthorizationType = (int)AuthorizationTypeEnum.REVERSE,
                Approved = true,
                Confirmed = null,
                ReversedAuthorizationId = x.Id
            });

            await _repository.AddRangeAsync(reversed);
        }
    }
}
