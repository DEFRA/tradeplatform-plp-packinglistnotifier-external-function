// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Diagnostics.CodeAnalysis;
using System.Text;
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
    [InlineData("approved", null)]
    [InlineData("Approved", null)]
    [InlineData("rejected", "failureReason")]
    [InlineData("Rejected", "failureReason")]
    public void ApprovalValidator_WithValidData_PassesValidation(string approvalStatus, string? failureReason)
    {
        // arrange
        var approvalMock = new Inbound.Approval
        {
            ApplicationId = $"someRef {Guid.NewGuid()}",
            ApprovalStatus = approvalStatus,
            FailureReasons = failureReason
        };

        // act
        var result = _sut.Validate(approvalMock);

        // assert
        result.IsValid.ShouldBe(true);
    }

    [Theory]
    [InlineData("approved", "failureReason")]
    [InlineData("Approved", "failureReason")]
    public void ApprovalValidator_WithValidData_FailsValidation_Status(string approvalStatus, string? failureReason)
    {
        // arrange
        var approvalMock = new Inbound.Approval
        {
            ApplicationId = $"someRef {Guid.NewGuid()}",
            ApprovalStatus = approvalStatus,
            FailureReasons = failureReason
        };

        // act
        var result = _sut.Validate(approvalMock);

        // assert
        result.IsValid.ShouldBe(false);
    }

    [Theory]
    [InlineData("rejected")]
    [InlineData("Rejected")]
    public void ApprovalValidator_WithValidData_FailsValidation_TooLong(string approvalStatus)
    {
        // arrange
        const int maxLength = 2001;
        StringBuilder errorBuilder = new(maxLength);
        for (int i = 0; i < maxLength; i++) errorBuilder.Append("1");

        var approvalMock = new Inbound.Approval
        {
            ApplicationId = $"someRef {Guid.NewGuid()}",
            ApprovalStatus = approvalStatus,
            FailureReasons = errorBuilder.ToString()
        };

        // act
        var result = _sut.Validate(approvalMock);

        // assert
        result.IsValid.ShouldBe(false);
        result.Errors.Count.ShouldBe(1);
        result.Errors[0].ErrorMessage.ShouldBe("Failure Reasons : Value is too long");
    }

    [Fact]
    public void ApprovalValidator_WithInvalidData_FailsValidation()
    {
        // arrange
        var approvalMock = new Inbound.Approval
        {
            ApplicationId = $"\n\t\r",
            ApprovalStatus = "invalidApprovalStatus",
            FailureReasons = "bob"
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
