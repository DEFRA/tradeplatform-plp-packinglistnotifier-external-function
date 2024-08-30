// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Application.Validators;

public static class PlNotifierValidationHelpers
{
    public static bool BeAGuid(string? value) => value is null || Guid.TryParse(value, out _);
}
