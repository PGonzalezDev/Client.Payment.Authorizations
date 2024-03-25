using Client.Payments.Authorizations.Models;
using Client.Payments.Authorizations.Services.Repositories;
using Client.Payments.Authorizations.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Client.Payments.Authorizations.Services.Tests;

[TestClass]
public class ReverseAuthorizationsServiceTests
{
    private readonly Mock<IAuthorizationRepository> _repositoryMock;
    private readonly Mock<IClientRepository> _clientRepositoryMock;
    private readonly IConfiguration _config;

    private readonly Mock<ReverseAuthorizationsService> _serviceMock;

    public ReverseAuthorizationsServiceTests()
    {
        _repositoryMock = new Mock<IAuthorizationRepository>();
        _clientRepositoryMock = new Mock<IClientRepository>();

        _config = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json")
            .Build();

        _serviceMock = new Mock<ReverseAuthorizationsService>(
            _repositoryMock.Object,
            _clientRepositoryMock.Object,
            _config
        )
        {
            CallBase = true
        };
    }

    [TestMethod]
    public async Task WhenReverseExpiredAuthorizations_WithValidInput_ThenOkResult()
    {
        // Arrange
        Guid clientId = Guid.NewGuid();

        var expiredAuth = new List<Models.Authorization>()
        {
            new Models.Authorization()
            {
                Id = Guid.NewGuid(),
                Amount = 1001.00M,
                ClientId = clientId,
                CreatedDate = DateTime.Now.AddMinutes(-5)
            },
            new Models.Authorization()
            {
                Id = Guid.NewGuid(),
                Amount = 1002.00M,
                ClientId = clientId,
                CreatedDate = DateTime.Now.AddMinutes(-5)
            },
        };

        _repositoryMock
            .Setup(mock => mock.AddRangeAsync(It.IsAny<IEnumerable<Models.Authorization>>()))
            .Verifiable();

        // Act
        await _serviceMock.Object.ReverseExpiredAuthorizations(expiredAuth);

        // Assert
        _repositoryMock.Verify(
            mock => mock.AddRangeAsync(It.IsAny<IEnumerable<Models.Authorization>>()),
            Times.Once
        );
    }

    [TestMethod]
    public async Task WhenUnconfirmExpiredAuthorizations_WithValidInput_ThenOkResult()
    {
        // Arrange
        Guid clientId = Guid.NewGuid();

        var expiredAuth = new List<Models.Authorization>()
        {
            new Models.Authorization()
            {
                Id = Guid.NewGuid(),
                Amount = 1001.00M,
                ClientId = clientId,
                CreatedDate = DateTime.Now.AddMinutes(-5)
            },
            new Models.Authorization()
            {
                Id = Guid.NewGuid(),
                Amount = 1002.00M,
                ClientId = clientId,
                CreatedDate = DateTime.Now.AddMinutes(-5)
            },
        };

        _repositoryMock
            .Setup(mock => mock.UpdateRangeAsync(It.IsAny<IEnumerable<Models.Authorization>>()))
            .Verifiable();

        // Act
        await _serviceMock.Object.UnconfirmExpiredAuthorizations(expiredAuth);

        // Assert
        _repositoryMock.Verify(
            mock => mock.UpdateRangeAsync(It.IsAny<IEnumerable<Models.Authorization>>()),
            Times.Once
        );
    }

    [TestMethod]
    public async Task WhenReverseAuthorizationsProcess_WithEmptyParams_ThenOkResult()
    {
        // Arrange
        List<Guid> clientIds = new List<Guid>()
        {
            Guid.NewGuid(),
            Guid.NewGuid()
        };

        var expiredAuth = new List<Models.Authorization>()
        {
            new Models.Authorization()
            {
                Id = Guid.NewGuid(),
                Amount = 1001.00M,
                ClientId = clientIds[0],
                CreatedDate = DateTime.Now.AddMinutes(-5)
            },
            new Models.Authorization()
            {
                Id = Guid.NewGuid(),
                Amount = 1002.00M,
                ClientId = clientIds[0],
                CreatedDate = DateTime.Now.AddMinutes(-5)
            },
        };

        _clientRepositoryMock
            .Setup(mock => mock.GetClientIdsByAuthorizationMode(ClientAuthModeEnum.APPROVE_AND_CONFIRM))
            .ReturnsAsync(clientIds);

        _repositoryMock
            .Setup(mock => mock.GetDailyAuthorizationsToConfirmAsync(clientIds[0]))
            .ReturnsAsync(expiredAuth);

        _serviceMock
            .Setup(mock => mock.UnconfirmExpiredAuthorizations(It.IsAny<IEnumerable<Models.Authorization>>()))
            .Verifiable();

        _serviceMock
            .Setup(mock => mock.ReverseExpiredAuthorizations(It.IsAny<IEnumerable<Models.Authorization>>()))
            .Verifiable();

        // Act
        await _serviceMock.Object.ReverseAuthorizationsProcessAsync();

        // Assert
        _clientRepositoryMock.Verify(
            mock => mock.GetClientIdsByAuthorizationMode(ClientAuthModeEnum.APPROVE_AND_CONFIRM),
            Times.Once
        );

        _repositoryMock.Verify(
            mock => mock.GetDailyAuthorizationsToConfirmAsync(clientIds[0]),
            Times.Once
        );

        _serviceMock.Verify(
            mock => mock.UnconfirmExpiredAuthorizations(It.IsAny<IEnumerable<Models.Authorization>>()),
            Times.Once
        );

        _serviceMock.Verify(
            mock => mock.ReverseExpiredAuthorizations(It.IsAny<IEnumerable<Models.Authorization>>()),
            Times.Once
        );
    }
}