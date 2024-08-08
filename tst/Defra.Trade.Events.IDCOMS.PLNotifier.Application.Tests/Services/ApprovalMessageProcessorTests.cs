// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

using System.Diagnostics.CodeAnalysis;
using System.Net;
using Azure.Messaging.ServiceBus;

using Defra.Trade.Common.Functions.Interfaces;
using Defra.Trade.Common.Functions.Models;
using Defra.Trade.Crm;
using Defra.Trade.Crm.Exceptions;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Services;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Application.Tests.Services;

[ExcludeFromCodeCoverage]
public sealed class ApprovalMessageProcessorTests
{
    private readonly ICrmClient _crmClient;
    private readonly ILogger<ApprovalMessageProcessor> _logger;
    private readonly IMessageRetryContextAccessor _retry;
    private readonly ApprovalMessageProcessor _sut;

    public ApprovalMessageProcessorTests()
    {
        _crmClient = A.Fake<ICrmClient>();
        _logger = A.Fake<ILogger<ApprovalMessageProcessor>>();
        _retry = A.Fake<IMessageRetryContextAccessor>(p => p.Strict());
        _sut = new ApprovalMessageProcessor(_crmClient, _logger, _retry);
    }

    [Fact]
    public async Task BuildCustomMessageHeaderAsync_ReturnsCustomMessageHeader()
    {
        // act
        var result = await _sut.BuildCustomMessageHeaderAsync();

        // assert
        result.ShouldNotBeNull();
        result.ShouldBeOfType(typeof(CustomMessageHeader));
    }

    [Theory]
    [InlineData(false, false, false)]
    [InlineData(false, false, true)]
    [InlineData(false, true, false)]
    [InlineData(true, false, false)]
    public void Ctor_WithNullParams_ThrowsArgumentNullException(bool p1NotNull, bool p2NotNull, bool p3NotNull)
    {
        // arrange
        var sut = () => new ApprovalMessageProcessor(
            p1NotNull ? _crmClient : null!,
            p2NotNull ? _logger : null!,
            p3NotNull ? _retry : null!);

        // act && assert
        sut.ShouldThrow<ArgumentNullException>();
    }

    [Fact]
    public void Ctor_WithParams_Instantiates()
    {
        // arrange
        var sut = new ApprovalMessageProcessor(_crmClient, _logger, _retry);

        // act && assert
        sut.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetSchemaAsync_ReturnsEmptyString()
    {
        // arrange
        var header = new TradeEventMessageHeader();

        // act
        var result = await _sut.GetSchemaAsync(header);

        // assert
        result.ShouldBe(string.Empty);
    }

    [Fact]
    public async Task ProcessAsync_WithAnyException_ThrowsTheException()
    {
        // arrange
        var applicationId = $"someReference {Guid.NewGuid()}";
        var header = GetValidTradeEventMessageHeader(applicationId);

        var message = new Models.Approval
        {
            ApplicationId = applicationId,
            ApprovalStatus = "rejected"
        };

        A.CallTo(() => _crmClient.ListPagedAsync<Dynamics.ApprovalPayload>(A<string>._, default))
            .Throws(new DivideByZeroException());

        // act && assert
        var result = await _sut.ProcessAsync(message, header).ShouldThrowAsync<DivideByZeroException>();
        result.Message.ShouldBe("Attempted to divide by zero.");
    }

    [Fact]
    public async Task ProcessAsync_WithDynamicsError_ThrowsCrmException()
    {
        // arrange
        var applicationId = $"someReference {Guid.NewGuid()}";
        var header = GetValidTradeEventMessageHeader(applicationId);

        var message = new Models.Approval
        {
            ApplicationId = applicationId,
            ApprovalStatus = "rejected"
        };

        var mockExportApplication = new Dynamics.ApprovalPayload
        {
            ApplicationId = message.ApplicationId,
            ExportApplicationId = Guid.NewGuid()
        };

        A.CallTo(() => _crmClient.ListPagedAsync<Dynamics.ApprovalPayload>(A<string>._, default))
            .ReturnsLazily(() => ToListPagedAsync(mockExportApplication));

        A.CallTo(() => _crmClient.UpdateAsync(A<Dynamics.ApprovalPayload>._, default))
            .ThrowsAsync(new CrmException("500", HttpStatusCode.InternalServerError, "mocked server error"));

        // act && assert
        var result = await _sut.ProcessAsync(message, header).ShouldThrowAsync<CrmException>();
        result.Message.ShouldBe("mocked server error");
        A.CallTo(() => _crmClient.ListPagedAsync<Dynamics.ApprovalPayload>(A<string>._, default)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _crmClient.UpdateAsync(A<Dynamics.ApprovalPayload>._, default)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ProcessAsync_WithInvalidApprovalStatus_ThrowsArgumentOutOfRangeException()
    {
        // arrange
        var applicationId = $"someReference {Guid.NewGuid()}";
        var header = GetValidTradeEventMessageHeader(applicationId);

        var message = new Models.Approval
        {
            ApplicationId = applicationId,
            ApprovalStatus = "invalidApprovalStatus"
        };

        // act && assert
        var result = await _sut.ProcessAsync(message, header).ShouldThrowAsync<ArgumentOutOfRangeException>();
        result.Message.ShouldBe("Specified argument was out of the range of valid values. (Parameter 'message')");
    }

    [Fact]
    public async Task ProcessMessage_Should_IgnoreOtherRequestExceptions()
    {
        // Arrange
        var applicationId = $"someReference {Guid.NewGuid()}";
        var header = GetValidTradeEventMessageHeader(applicationId);

        var message = new Models.Approval
        {
            ApplicationId = applicationId,
            ApprovalStatus = "rejected"
        };
        var mockedMessage = ServiceBusModelFactory.ServiceBusReceivedMessage(properties: new Dictionary<string, object?>
        {
            ["RetryCount"] = 1
        });

        var exception = new CrmException(HttpStatusCode.InternalServerError.ToString(), HttpStatusCode.InternalServerError, "Test exception");

        var messageRetryContext = A.Fake<IMessageRetryContext>();
        var mockExportApplication = new Dynamics.ApprovalPayload
        {
            ApplicationId = message.ApplicationId,
            ExportApplicationId = Guid.NewGuid()
        };

        A.CallTo(() => _retry.Context).Returns(messageRetryContext);
        A.CallTo(() => messageRetryContext.Message).Returns(mockedMessage);

        A.CallTo(() => _crmClient.ListPagedAsync<Dynamics.ApprovalPayload>(A<string>._, default))
            .ReturnsLazily(() => ToListPagedAsync(mockExportApplication));

        A.CallTo(() => _crmClient.UpdateAsync(A<Dynamics.ApprovalPayload>._, default))
            .ThrowsAsync(exception);

        var test = async () => await _sut.ProcessAsync(message, header);

        // Act
        var actual = await test.ShouldThrowAsync<CrmException>();

        //Assert
        actual.ShouldBeSameAs(exception);
    }

    [Theory]
    [InlineData("Approved")]
    [InlineData("approved")]
    [InlineData("Rejected")]
    [InlineData("rejected")]
    public async Task ProcessAsync_WithValidParams_SendsToDynamics(string approvalStatus)
    {
        // arrange
        var applicationId = $"someReference {Guid.NewGuid()}";
        var header = GetValidTradeEventMessageHeader(applicationId);

        var message = new Models.Approval
        {
            ApplicationId = applicationId,
            ApprovalStatus = approvalStatus
        };

        var mockExportApplication = new Dynamics.ApprovalPayload
        {
            ApplicationId = message.ApplicationId,
            ExportApplicationId = Guid.NewGuid()
        };

        A.CallTo(() => _crmClient.ListPagedAsync<Dynamics.ApprovalPayload>(A<string>._, default))
            .ReturnsLazily(() => ToListPagedAsync(mockExportApplication));

        A.CallTo(() => _crmClient.UpdateAsync(A<Dynamics.ApprovalPayload>._, default))
            .ReturnsLazily(() => mockExportApplication);

        // act
        var result = await _sut.ProcessAsync(message, header);

        // assert
        result.ForwardMessage.ShouldBeFalse();
        result.Response.ShouldBe(message);
        A.CallTo(() => _crmClient.ListPagedAsync<Dynamics.ApprovalPayload>(A<string>._, default))
            .MustHaveHappenedOnceExactly();
        A.CallTo(() => _crmClient.UpdateAsync(A<Dynamics.ApprovalPayload>._, default))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ValidateMessageLabelAsync_WithValidLabel_ReturnsTrue()
    {
        // act
        var header = new TradeEventMessageHeader()
        {
            Label = "plp.idcoms.parsed"
        };

        // assert
        var result = await _sut.ValidateMessageLabelAsync(header);

        // arrange
        result.ShouldBeTrue();
    }

    private static TradeEventMessageHeader GetValidTradeEventMessageHeader(string entityKey = null!)
    {
        return new TradeEventMessageHeader
        {
            CausationId = Guid.NewGuid().ToString(),
            ContentType = "application/json",
            CorrelationId = Guid.NewGuid().ToString(),
            EntityKey = entityKey is null ? $"someReference {Guid.NewGuid()}" : entityKey,
            Label = "plp.idcoms.parsed",
            MessageId = Guid.NewGuid().ToString(),
            OrganisationId = Guid.NewGuid().ToString(),
            PublisherId = "PLP",
            Status = "Complete",
            TimestampUtc = 1704067200,
            Type = Common.Functions.Models.Enum.EventType.Internal
        };
    }

    private static async IAsyncEnumerable<IEnumerable<Dynamics.ApprovalPayload>> ToListPagedAsync(Dynamics.ApprovalPayload payload)
    {
        var page = new List<Dynamics.ApprovalPayload>() { payload };

        yield return page;

        await Task.CompletedTask;
    }
}
