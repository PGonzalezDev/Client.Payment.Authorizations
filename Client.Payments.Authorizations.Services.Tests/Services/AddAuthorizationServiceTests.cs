using Client.Payments.Authorizations.Models;
using Client.Payments.Authorizations.Services.DTOs;
using Client.Payments.Authorizations.Services.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Client.Payments.Authorizations.Services.Tests;

[TestClass]
public class AddAuthorizationServiceTests
{
    private readonly Mock<IAuthorizationRepository> _repositoryMock;
    private readonly Mock<IClientRepository> _clientRepositoryMock;
    private readonly Mock<IReverseAuthorizationsService> _reverseAuthorizationsServiceMock;
    private readonly Mock<IPaymentsAuthorizerService> _paymentsAuthorizerServiceMock;
    private readonly Mock<IRabbitMqService> _publisherServiceMock;
    private readonly IConfiguration _config;
    private readonly AuthorizationService _authorizationService;

    public AddAuthorizationServiceTests()
    {
        _repositoryMock = new Mock<IAuthorizationRepository>();
        _clientRepositoryMock = new Mock<IClientRepository>();
        _reverseAuthorizationsServiceMock = new Mock<IReverseAuthorizationsService>();
        _paymentsAuthorizerServiceMock = new Mock<IPaymentsAuthorizerService>();
        _publisherServiceMock = new Mock<IRabbitMqService>();

        _config = new ConfigurationBuilder()
            .AddJsonFile($"appsettings.json")
            .Build();

        _authorizationService = new AuthorizationService(
            _repositoryMock.Object,
            _clientRepositoryMock.Object,
            _reverseAuthorizationsServiceMock.Object,
            _paymentsAuthorizerServiceMock.Object,
            _publisherServiceMock.Object,
            _config
        );
    }

    [TestMethod]
    public async Task WhenAddAuthorization_WithAuthorizedPayment_ThenApprovedResult()
    {
        // Arrange
        Models.Authorization createAuthorization = new Models.Authorization()
        {
            Amount = 2500M,
            ClientId = Guid.NewGuid(),
            AuthorizationType = (int)AuthorizationTypeEnum.PAYMENTS
        };

        _paymentsAuthorizerServiceMock
            .Setup(mock => mock.PaymentApproval(createAuthorization.Amount))
            .ReturnsAsync(true);

        _repositoryMock
            .Setup(mock => mock.AddAsync(createAuthorization))
            .ReturnsAsync((Models.Authorization createdAuth) =>
            {
                createdAuth.Id = Guid.NewGuid();
                return createdAuth;
            });

        _publisherServiceMock
            .Setup(mock => mock.Publish(It.IsAny<ApprovedAuthorization>()))
            .Verifiable();

        // Act
        var result = await _authorizationService.AddAuthorizationAsync(createAuthorization);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(CreatedAuthorization));

        Assert.AreNotEqual(Guid.Empty, result.Id);
        Assert.AreEqual(createAuthorization.Amount, result.Amount);
        Assert.AreEqual(createAuthorization.Amount, result.Amount);
        Assert.AreEqual(createAuthorization.ClientId, result.ClientId);
        Assert.AreEqual(AuthorizationTypeEnum.PAYMENTS.ToString(), result.AuthorizationType);
        Assert.IsTrue(result.Approved);

        _paymentsAuthorizerServiceMock.Verify(
            mock => mock.PaymentApproval(createAuthorization.Amount),
            Times.Once
        );

        _repositoryMock.Verify(
            mock => mock.AddAsync(createAuthorization),
            Times.Once
        );

        _publisherServiceMock.Verify(
            mock => mock.Publish(It.IsAny<ApprovedAuthorization>()),
            Times.Once
        );
    }

    [TestMethod]
    public async Task WhenAddAuthorization_WithUnauthorizedPayment_ThenNotApprovedResult()
    {
        // Arrange
        Models.Authorization createAuthorization = new Models.Authorization()
        {
            Amount = 2500.05M,
            ClientId = Guid.NewGuid(),
            AuthorizationType = (int)AuthorizationTypeEnum.PAYMENTS
        };

        _paymentsAuthorizerServiceMock
            .Setup(mock => mock.PaymentApproval(createAuthorization.Amount))
            .ReturnsAsync(false);

        _repositoryMock
            .Setup(mock => mock.AddAsync(createAuthorization))
            .ReturnsAsync((Models.Authorization createdAuth) =>
            {
                createdAuth.Id = Guid.NewGuid();
                return createdAuth;
            });

        _publisherServiceMock
            .Setup(mock => mock.Publish(It.IsAny<ApprovedAuthorization>()))
            .Verifiable();

        // Act
        var result = await _authorizationService.AddAuthorizationAsync(createAuthorization);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(CreatedAuthorization));

        Assert.AreNotEqual(Guid.Empty, result.Id);
        Assert.AreEqual(createAuthorization.Amount, result.Amount);
        Assert.AreEqual(createAuthorization.Amount, result.Amount);
        Assert.AreEqual(createAuthorization.ClientId, result.ClientId);
        Assert.AreEqual(AuthorizationTypeEnum.PAYMENTS.ToString(), result.AuthorizationType);
        Assert.IsFalse(result.Approved);

        _paymentsAuthorizerServiceMock.Verify(
            mock => mock.PaymentApproval(createAuthorization.Amount),
            Times.Once
        );

        _repositoryMock.Verify(
            mock => mock.AddAsync(createAuthorization),
            Times.Once
        );

        _publisherServiceMock.Verify(
            mock => mock.Publish(It.IsAny<ApprovedAuthorization>()),
            Times.Never
        );
    }

    [TestMethod]
    public async Task WhenAddAuthorization_WithNullParam_ThrowArgumentNullException()
    {
        // Arrange
        Models.Authorization createAuthorization = null;

        _paymentsAuthorizerServiceMock
            .Setup(mock => mock.PaymentApproval(It.IsAny<decimal>()))
            .Verifiable();

        _repositoryMock
            .Setup(mock => mock.AddAsync(It.IsAny<Authorization>()))
            .Verifiable();

        _publisherServiceMock
            .Setup(mock => mock.Publish(It.IsAny<ApprovedAuthorization>()))
            .Verifiable();

        // Assert
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(() =>
            // Act
            _authorizationService.AddAuthorizationAsync(createAuthorization)
        );

        _paymentsAuthorizerServiceMock.Verify(
            mock => mock.PaymentApproval(It.IsAny<decimal>()),
            Times.Never
        );

        _repositoryMock.Verify(
            mock => mock.AddAsync(It.IsAny<Authorization>()),
            Times.Never
        );

        _publisherServiceMock.Verify(
            mock => mock.Publish(It.IsAny<ApprovedAuthorization>()),
            Times.Never
        );
    }
}
