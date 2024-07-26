// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

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
        var input = Guid.NewGuid().ToString();

        // act
        var result = PlNotifierValidationHelpers.BeAGuid(input);

        // assert
        result.ShouldBeTrue();
    }

    [Fact]
    public void BeAGuid_WithNonGuid_ReturnsFalse()
    {
        // arrange
        var input = "notAGuid";

        // act
        var result = PlNotifierValidationHelpers.BeAGuid(input);

        // assert
        result.ShouldBeFalse();
    }
}
