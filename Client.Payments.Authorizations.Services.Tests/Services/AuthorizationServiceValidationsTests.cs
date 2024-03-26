using Client.Payments.Authorizations.Models;
using Client.Payments.Authorizations.Services.DTOs;
using Client.Payments.Authorizations.Services.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Client.Payments.Authorizations.Services.Tests;

[TestClass]
public class AuthorizationServiceValidationsTests
{
    private readonly Mock<IAuthorizationRepository> _repositoryMock;
    private readonly Mock<IClientRepository> _clientRepositoryMock;
    private readonly Mock<IReverseAuthorizationsService> _reverseAuthorizationsServiceMock;
    private readonly Mock<IPaymentsAuthorizerService> _paymentsAuthorizerServiceMock;
    private readonly Mock<IRabbitMqService> _publisherServiceMock;
    private readonly IConfiguration _config;
    private readonly AuthorizationService _authorizationService;

    public AuthorizationServiceValidationsTests()
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
    public async Task WhenValidateClientAuthorizationMode_WithValidClientMode_ThenTrueResult()
    {
        // Arrange
        Guid authorizationId = Guid.NewGuid();

        _clientRepositoryMock
            .Setup(mock => mock.GetClientByAuthorizationIdAsync(authorizationId))
            .ReturnsAsync(new Models.Client()
            {
                Id = authorizationId,
                AuthorizationMode = (int)ClientAuthModeEnum.APPROVE_AND_CONFIRM
            })
            .Verifiable();

        // Act
        var tupleResult = await _authorizationService.ValidateClientAuthorizationMode(authorizationId);

        // Assert
        Assert.IsNotNull(tupleResult);
        Assert.IsInstanceOfType(tupleResult, typeof(ValueTuple<bool, ConfirmAuthorizationResult>));
        Assert.IsTrue(tupleResult.Item1);
        Assert.IsNull(tupleResult.Item2);

        _clientRepositoryMock.Verify(
            mock => mock.GetClientByAuthorizationIdAsync(authorizationId),
            Times.Once
        );
    }

    [TestMethod]
    public async Task WhenValidateClientAuthorizationMode_WithInvalidClientMode_ThenFalseResult()
    {
        // Arrange
        Guid authorizationId = Guid.NewGuid();
        string clientName = "Little Jhon";

        _clientRepositoryMock
            .Setup(mock => mock.GetClientByAuthorizationIdAsync(authorizationId))
            .ReturnsAsync(new Models.Client()
            {
                Id = authorizationId,
                Name = clientName,
                AuthorizationMode = (int)ClientAuthModeEnum.APPROVE
            })
            .Verifiable();

        // Act
        var tupleResult = await _authorizationService.ValidateClientAuthorizationMode(authorizationId);

        // Assert
        Assert.IsNotNull(tupleResult);
        Assert.IsInstanceOfType(tupleResult, typeof(ValueTuple<bool, ConfirmAuthorizationResult>));
        Assert.IsFalse(tupleResult.Item1);
        
        Assert.IsNotNull(tupleResult.Item2);
        Assert.AreEqual(authorizationId, tupleResult.Item2.AuthorizationId);
        Assert.AreEqual(ResultCodeEnum.UnprocessableEntity, tupleResult.Item2.ResultCode);
        Assert.AreEqual($"Invalid Authorization Mode to {clientName} Client.", tupleResult.Item2.ErrorMsg);

        _clientRepositoryMock.Verify(
            mock => mock.GetClientByAuthorizationIdAsync(authorizationId),
            Times.Once
        );
    }

    [TestMethod]
    public async Task WhenValidateAuthorization_AndAuthorizationIsNotConfirmedYet_ThenTrueResult()
    {
        // Arrange
        Models.Authorization authorization = new Models.Authorization()
        {
            Id = Guid.NewGuid(),
            Approved = true,
            Confirmed = null
        };

        // Act
        var tupleResult = await _authorizationService.ValidateAuthorization(authorization);

        // Assert
        Assert.IsNotNull(tupleResult);
        Assert.IsInstanceOfType(tupleResult, typeof(ValueTuple<bool, ConfirmAuthorizationResult>));
        Assert.IsTrue(tupleResult.Item1);

        Assert.IsNull(tupleResult.Item2);
    }

    [TestMethod]
    public async Task WhenValidateAuthorization_AndAuthorizationIsNotApproved_ThenFalseResult()
    {
        // Arrange
        Models.Authorization authorization = new Models.Authorization()
        {
            Id = Guid.NewGuid(),
            Approved = false,
            Confirmed = null
        };

        // Act
        var tupleResult = await _authorizationService.ValidateAuthorization(authorization);

        // Assert
        Assert.IsNotNull(tupleResult);
        Assert.IsInstanceOfType(tupleResult, typeof(ValueTuple<bool, ConfirmAuthorizationResult>));
        Assert.IsFalse(tupleResult.Item1);

        Assert.IsNotNull(tupleResult.Item2);
        Assert.AreEqual(authorization.Id, tupleResult.Item2.AuthorizationId);
        Assert.AreEqual(ResultCodeEnum.UnprocessableEntity, tupleResult.Item2.ResultCode);
        Assert.AreEqual("Authorization is not approved.", tupleResult.Item2.ErrorMsg);
    }

    [TestMethod]
    public async Task WhenValidateAuthorization_AndAuthorizationIsAlreadyConfirmed_ThenFalseResult()
    {
        // Arrange
        Models.Authorization authorization = new Models.Authorization()
        {
            Id = Guid.NewGuid(),
            Approved = true,
            Confirmed = true
        };

        // Act
        var tupleResult = await _authorizationService.ValidateAuthorization(authorization);

        // Assert
        Assert.IsNotNull(tupleResult);
        Assert.IsInstanceOfType(tupleResult, typeof(ValueTuple<bool, ConfirmAuthorizationResult>));
        Assert.IsFalse(tupleResult.Item1);

        Assert.IsNotNull(tupleResult.Item2);
        Assert.AreEqual(authorization.Id, tupleResult.Item2.AuthorizationId);
        Assert.AreEqual(ResultCodeEnum.UnprocessableEntity, tupleResult.Item2.ResultCode);
        Assert.AreEqual("Authorization already confirmed.", tupleResult.Item2.ErrorMsg);
    }

    [TestMethod]
    public async Task WhenValidateNotExpiredAuthorization_WithAuthorizationNotExpired_ThenTrueResult()
    {
        // Arrange
        Models.Authorization authorization = new Models.Authorization()
        {
            Id = Guid.NewGuid(),
            CreatedDate = DateTime.Now
        };

        // Act
        var tupleResult = await _authorizationService.ValidateNotExpiredAuthorization(authorization);

        // Assert
        Assert.IsNotNull(tupleResult);
        Assert.IsInstanceOfType(tupleResult, typeof(ValueTuple<bool, ConfirmAuthorizationResult>));
        Assert.IsTrue(tupleResult.Item1);

        Assert.IsNull(tupleResult.Item2);
    }

    [TestMethod]
    public async Task WhenValidateNotExpiredAuthorization_WithAuthorizationExpired_ThenFalseResult()
    {
        // Arrange
        int confirmMinutes = int.Parse(_config["SetConfirmMinutes"]);

        Models.Authorization authorization = new Models.Authorization()
        {
            Id = Guid.NewGuid(),
            CreatedDate = DateTime.Now.AddMinutes(-confirmMinutes)
        };

        // Act
        var tupleResult = await _authorizationService.ValidateNotExpiredAuthorization(authorization);

        // Assert
        Assert.IsNotNull(tupleResult);
        Assert.IsInstanceOfType(tupleResult, typeof(ValueTuple<bool, ConfirmAuthorizationResult>));
        Assert.IsFalse(tupleResult.Item1);

        Assert.IsNotNull(tupleResult.Item2);
        Assert.AreEqual(authorization.Id, tupleResult.Item2.AuthorizationId);
        Assert.AreEqual(ResultCodeEnum.UnprocessableEntity, tupleResult.Item2.ResultCode);
        Assert.AreEqual("Expired Confirmation Time.", tupleResult.Item2.ErrorMsg);
    }
}
