// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

using Defra.Trade.Common.Functions.Models;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Validators;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Application.Tests.Validators;

public sealed class MessageHeaderValidatorTests
{
    private readonly MessageHeaderValidator _sut;

    public MessageHeaderValidatorTests()
    {
        _sut = new MessageHeaderValidator();
    }

    [Fact]
    public void MessageHeaderValidator_WithValidData_PassesValidation()
    {
        // arrange
        var mockHeader = new TradeEventMessageHeader
        {
            CausationId = Guid.NewGuid().ToString(),
            ContentType = "application/json",
            CorrelationId = Guid.NewGuid().ToString(),
            EntityKey = $"someReference {Guid.NewGuid()}",
            Label = "plp.idcoms.parsed",
            MessageId = Guid.NewGuid().ToString(),
            OrganisationId = Guid.NewGuid().ToString(),
            PublisherId = "PLP",
            Status = "Complete",
            TimestampUtc = 1704067200,
            Type = Common.Functions.Models.Enum.EventType.Internal
        };

        // act
        var result = _sut.Validate(mockHeader);

        // assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void MessageHeaderValidator_WithInvalidData_FailsValidation()
    {
        // arrange
        var mockHeader = new TradeEventMessageHeader
        {
            CausationId = "not a guid",
            ContentType = "text/plain",
            CorrelationId = "not a guid",
            EntityKey = "\r\t\n",
            Label = "wrong label",
            MessageId = "not a guid",
            OrganisationId = "not a guid",
            PublisherId = "wrong publisher",
            Status = "wrong status",
            TimestampUtc = 0,
            Type = Common.Functions.Models.Enum.EventType.None
        };

        // act
        var result = _sut.Validate(mockHeader);

        // assert
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBe(11);
        result.Errors[0].ErrorMessage.ShouldBe("Causation Id is not a valid guid");
        result.Errors[1].ErrorMessage.ShouldBe("Content Type must be application/json");
        result.Errors[2].ErrorMessage.ShouldBe("Correlation Id is not a valid guid");
        result.Errors[3].ErrorMessage.ShouldBe("Entity Key cannot be null");
        result.Errors[4].ErrorMessage.ShouldBe("Label must be plp.idcoms.parsed");
        result.Errors[5].ErrorMessage.ShouldBe("Message Id is not a valid guid");
        result.Errors[6].ErrorMessage.ShouldBe("Organisation Id is not a valid guid");
        result.Errors[7].ErrorMessage.ShouldBe("Publisher Id must be PLP");
        result.Errors[8].ErrorMessage.ShouldBe("Status must be Complete");
        result.Errors[9].ErrorMessage.ShouldBe("Timestamp Utc cannot be null");
        result.Errors[10].ErrorMessage.ShouldBe("Type must be Internal");
    }
}
