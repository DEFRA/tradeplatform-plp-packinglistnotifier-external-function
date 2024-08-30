// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Application.Validators;

public static class PlNotifierValidationMessages
{
    public const string EqualField = "{PropertyName} must be {ComparisonValue}";
    public const string GuidField = "{PropertyName} is not a valid guid";
    public const string ApprovalStatus = "{PropertyName} : {PropertyValue} is not a valid approval status";
}
