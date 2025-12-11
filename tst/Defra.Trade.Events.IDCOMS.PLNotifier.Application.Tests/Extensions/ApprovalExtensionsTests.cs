// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Diagnostics.CodeAnalysis;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Extensions;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Models;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Application.Tests.Extensions;

[ExcludeFromCodeCoverage]
public sealed class ApprovalExtensionsTests
{

    [Fact]
    public void IsRejectedWithoutReason_True()
    {
        Approval input = new()
        {
            ApprovalStatus = "rejected"
        };

        bool result = input.IsRejectedWithoutReason();

        result.ShouldBeTrue();
    }

    [Fact]
    public void IsRejectedWithoutReason_False_Reason()
    {
        Approval input = new()
        {
            ApprovalStatus = "rejected",
            FailureReasons = "A reason"
        };

        bool result = input.IsRejectedWithoutReason();

        result.ShouldBeFalse();
    }

    [Fact]
    public void IsRejectedIneligibleWithoutReason_True()
    {
        Approval input = new()
        {
            ApprovalStatus = "rejected_ineligible"
        };

        bool result = input.IsRejectedWithoutReason();

        result.ShouldBeTrue();
    }

    [Fact]
    public void IsRejectedIneligibleWithoutReason_False_Reason()
    {
        Approval input = new()
        {
            ApprovalStatus = "rejected_ineligible",
            FailureReasons = "A reason"
        };

        bool result = input.IsRejectedWithoutReason();

        result.ShouldBeFalse();

    }

    [Fact]
    public void IsRejectedCooWithoutReason_True()
    {
        Approval input = new()
        {
            ApprovalStatus = "rejected_coo"
        };

        bool result = input.IsRejectedWithoutReason();

        result.ShouldBeTrue();
    }

    [Fact]
    public void IsRejectedCooWithoutReason_False_Reason()
    {
        Approval input = new()
        {
            ApprovalStatus = "rejected_coo",
            FailureReasons = "A reason"
        };

        bool result = input.IsRejectedWithoutReason();

        result.ShouldBeFalse();
    }

    [Fact]
    public void IsRejectedOtherWithoutReason_True()
    {
        Approval input = new()
        {
            ApprovalStatus = "rejected_other"
        };

        bool result = input.IsRejectedWithoutReason();

        result.ShouldBeTrue();
    }

    [Fact]
    public void IsRejectedOtherWithoutReason_False_Reason()
    {
        Approval input = new()
        {
            ApprovalStatus = "rejected_other",
            FailureReasons = "A reason"
        };

        bool result = input.IsRejectedWithoutReason();

        result.ShouldBeFalse();
    }

    [Fact]
    public void IsRejectedWithoutReason_False_Accepted()
    {
        Approval input = new()
        {
            ApprovalStatus = "accepted",
        };

        bool result = input.IsRejectedWithoutReason();

        result.ShouldBeFalse();
    }
}
