// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Diagnostics.CodeAnalysis;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Validators;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Application.Tests.Validators;

[ExcludeFromCodeCoverage]
public sealed class ApprovalValidatorTests
{
    private readonly ApprovalValidator _sut;

    public ApprovalValidatorTests()
    {
        _sut = new ApprovalValidator();
    }

    [Theory]
    [InlineData("approved")]
    [InlineData("Approved")]
    [InlineData("rejected")]
    [InlineData("Rejected")]
    public void ApprovalValidator_WithValidData_PassesValidation(string approvalStatus)
    {
        // arrange
        var approvalMock = new Inbound.Approval
        {
            ApplicationId = $"someRef {Guid.NewGuid()}",
            ApprovalStatus = approvalStatus
        };

        // act
        var result = _sut.Validate(approvalMock);

        // assert
        result.IsValid.ShouldBe(true);
    }

    [Fact]
    public void ApprovalValidator_WithInvalidData_FailsValidation()
    {
        // arrange
        var approvalMock = new Inbound.Approval
        {
            ApplicationId = $"\n\t\r",
            ApprovalStatus = "invalidApprovalStatus"
        };

        // act
        var result = _sut.Validate(approvalMock);

        // assert
        result.IsValid.ShouldBe(false);
        result.Errors.Count.ShouldBe(2);
        result.Errors[0].ErrorMessage.ShouldBe("Application Id cannot be null");
        result.Errors[1].ErrorMessage.ShouldBe("Approval Status : invalidApprovalStatus is not a valid approval status");
    }
}
