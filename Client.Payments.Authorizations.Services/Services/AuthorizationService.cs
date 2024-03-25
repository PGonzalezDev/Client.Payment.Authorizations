using Client.Payments.Authorizations.Models;
using Client.Payments.Authorizations.Services.DTOs;
using Client.Payments.Authorizations.Services.Repositories;
using Microsoft.Extensions.Configuration;

namespace Client.Payments.Authorizations.Services;

public class AuthorizationService : IAuthorizationService
{
    private readonly IAuthorizationRepository _repository;
    private readonly IClientRepository _clientRepository;
    private readonly IReverseAuthorizationsService _reverseAuthorizationsService;
    private readonly IPaymentsAuthorizerService _paymentsAuthorizerService;
    private readonly IRabbitMqService _publisherService;
    private readonly IConfiguration _config;

    public AuthorizationService(
        IAuthorizationRepository repository,
        IClientRepository clientRepository,
        IReverseAuthorizationsService reverseAuthorizationsService,
        IPaymentsAuthorizerService paymentsAuthorizerService,
        IRabbitMqService publisherService,
        IConfiguration config
    )
    {
        _repository = repository;
        _clientRepository = clientRepository;
        _reverseAuthorizationsService = reverseAuthorizationsService;
        _paymentsAuthorizerService = paymentsAuthorizerService;
        _publisherService = publisherService;
        _config = config;
    }

    public async Task<CreatedAuthorization> AddAuthorizationAsync(Models.Authorization createAuthorization)
    {
        ArgumentNullException.ThrowIfNull(createAuthorization, nameof(createAuthorization));

        bool approved = await _paymentsAuthorizerService.PaymentApproval(createAuthorization.Amount);

        createAuthorization.CreatedDate = DateTime.Now;
        createAuthorization.Approved = approved;

        var result = await _repository.AddAsync(createAuthorization);

        if(approved)
        {
            _publisherService.Publish(new ApprovedAuthorization(createAuthorization));
        }

        return new CreatedAuthorization(result);
    }

    public async Task<ConfirmAuthorizationResult> ConfirmAuthorizationAsync(ConfirmAuthorization confirmAuthorization)
    {
        ArgumentNullException.ThrowIfNull(confirmAuthorization, nameof(confirmAuthorization));

        bool exists = await _repository.ExistsAsync(confirmAuthorization.AuthorizationId);

        if (!exists)
        {
            return new ConfirmAuthorizationResult()
            {
                AuthorizationId = confirmAuthorization.AuthorizationId,
                ResultCode = ResultCodeEnum.NotFound,
                ErrorMsg = $"AuthorizationId {confirmAuthorization.AuthorizationId} Not Found."
            };
        }

        (bool, ConfirmAuthorizationResult) clientTupleValidation = await ValidateClientAuthorizationMode(
            confirmAuthorization.AuthorizationId
        );

        if (!clientTupleValidation.Item1)
        {
            return clientTupleValidation.Item2;
        }

        Models.Authorization authorization = await _repository.FirstOrDefaultAsync(
            a => a.Id == confirmAuthorization.AuthorizationId
        );

        (bool, ConfirmAuthorizationResult) authorizationTupleValidation = await ValidateAuthorization(
           authorization
        );

        if (!authorizationTupleValidation.Item1)
        {
            return authorizationTupleValidation.Item2;
        }

        (bool, ConfirmAuthorizationResult) notExpiredTupleValidation = await ValidateNotExpiredAuthorization(
           authorization
        );

        if (!notExpiredTupleValidation.Item1)
        {
            var expiredAuth = new List<Models.Authorization>() { authorization };

            await _reverseAuthorizationsService.UnconfirmExpiredAuthorizations(
                expiredAuth
            );

            await _reverseAuthorizationsService.ReverseExpiredAuthorizations(
                expiredAuth
            );

            return notExpiredTupleValidation.Item2;
        }

        Models.Authorization result = await _repository.ConfirmAuthorizationAsync(
            confirmAuthorization.AuthorizationId,
            confirmAuthorization.Confirm
        );

        return new ConfirmAuthorizationResult()
        {
            AuthorizationId = result.Id,
            Confirmed = result.Confirmed,
            ResultCode = ResultCodeEnum.Ok
        };
    }

    public virtual async Task<(bool, ConfirmAuthorizationResult)> ValidateClientAuthorizationMode(Guid authorizationId)
    {
        Models.Client client = await _clientRepository.GetClientByAuthorizationIdAsync(authorizationId);
        ClientAuthModeEnum authoMode = (ClientAuthModeEnum)client.AuthorizationMode;

        if (authoMode != ClientAuthModeEnum.APPROVE_AND_CONFIRM)
        {
            var result = new ConfirmAuthorizationResult()
            {
                AuthorizationId = authorizationId,
                ResultCode = ResultCodeEnum.UnprocessableEntity,
                ErrorMsg = $"Invalid Authorization Mode to {client.Name} Client."
            };

            return (false, result);
        }

        return (true, null);
    }

    public virtual async Task<(bool, ConfirmAuthorizationResult)> ValidateAuthorization(Models.Authorization authorization)
    {
        if (!authorization.Approved)
        {
            var confirmedErrorResult = new ConfirmAuthorizationResult()
            {
                AuthorizationId = authorization.Id,
                ResultCode = ResultCodeEnum.UnprocessableEntity,
                ErrorMsg = "Authorization is not approved."
            };

            return (false, confirmedErrorResult);
        }

        if (authorization.Confirmed.HasValue && authorization.Confirmed.Value)
        {
            var confirmedErrorResult = new ConfirmAuthorizationResult()
            {
                AuthorizationId = authorization.Id,
                ResultCode = ResultCodeEnum.UnprocessableEntity,
                ErrorMsg = "Authorization already confirmed."
            };

            return (false, confirmedErrorResult);
        }

        return (true, null);
    }

    public virtual async Task<(bool, ConfirmAuthorizationResult)> ValidateNotExpiredAuthorization(Models.Authorization authorization)
    {
        int minutes = int.Parse(_config["SetConfirmMinutes"] ?? "5");
        bool validTimeToConfirm = DateTime.Now.Subtract(authorization.CreatedDate).Minutes < minutes;

        if (!validTimeToConfirm)
        {
            var expiredResult = new ConfirmAuthorizationResult()
            {
                AuthorizationId = authorization.Id,
                ResultCode = ResultCodeEnum.UnprocessableEntity,
                ErrorMsg = "Expired Confirmation Time."
            };

            return (false, expiredResult);
        }

        return (true, null);
    }
}
