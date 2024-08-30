// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Diagnostics.CodeAnalysis;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Validators;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Application.Tests.Validators;

[ExcludeFromCodeCoverage]
public sealed class PlNotifierValidationHelpersTests
{
    [Fact]
    public void BeAGuid_WithGuid_ReturnsTrue()
    {
        // arrange
        string input = Guid.NewGuid().ToString();

        // act
        bool result = PlNotifierValidationHelpers.BeAGuid(input);

        // assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void BeAGuid_WithNonGuid_ReturnsFalse()
    {
        // arrange
        string input = "notAGuid";

        // act
        bool result = PlNotifierValidationHelpers.BeAGuid(input);

        // assert
        result.ShouldBeFalse();
    }
}