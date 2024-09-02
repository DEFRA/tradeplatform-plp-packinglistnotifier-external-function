// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Defra.Trade.Common.Functions.Models;
using Defra.Trade.Common.Functions.Models.Enum;
using Defra.Trade.Common.Functions.Validation;
using FluentValidation;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Application.Validators;

public sealed class MessageHeaderValidator : AbstractValidator<TradeEventMessageHeader>
{
    public MessageHeaderValidator() : base()
    {
        RuleFor(m => m.CausationId!)
            .Must(PlNotifierValidationHelpers.BeAGuid).WithMessage(PlNotifierValidationMessages.GuidField);

        RuleFor(m => m.ContentType!)
            .Equal(Models.PlNotifierHeaderConstants.ContentType, StringComparer.OrdinalIgnoreCase).WithMessage(PlNotifierValidationMessages.EqualField);

        RuleFor(m => m.CorrelationId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ValidationMessages.NullField)
            .Must(PlNotifierValidationHelpers.BeAGuid).WithMessage(PlNotifierValidationMessages.GuidField);

        RuleFor(m => m.EntityKey)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ValidationMessages.NullField);

        RuleFor(m => m.Label)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ValidationMessages.NullField)
            .Equal(Models.PlNotifierHeaderConstants.Label, StringComparer.OrdinalIgnoreCase).WithMessage(PlNotifierValidationMessages.EqualField);

        RuleFor(m => m.MessageId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ValidationMessages.NullField)
            .Must(PlNotifierValidationHelpers.BeAGuid).WithMessage(PlNotifierValidationMessages.GuidField);

        RuleFor(m => m.OrganisationId!)
            .Must(PlNotifierValidationHelpers.BeAGuid).WithMessage(PlNotifierValidationMessages.GuidField);

        RuleFor(m => m.PublisherId!)
            .Equal(Models.PlNotifierHeaderConstants.PublisherId, StringComparer.OrdinalIgnoreCase).WithMessage(PlNotifierValidationMessages.EqualField);

        RuleFor(m => m.Status)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ValidationMessages.NullField)
            .Equal(Models.PlNotifierHeaderConstants.Status, StringComparer.OrdinalIgnoreCase).WithMessage(PlNotifierValidationMessages.EqualField);

        RuleFor(m => m.TimestampUtc)
            .GreaterThan(0).WithMessage(ValidationMessages.NullField);

        RuleFor(m => m.Type)
            .Equal(EventType.Internal).WithMessage(PlNotifierValidationMessages.EqualField);
    }
}
