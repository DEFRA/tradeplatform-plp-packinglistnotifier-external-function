// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

using Defra.Trade.Common.Functions.Models;
using Defra.Trade.Common.Functions.Models.Enum;
using Defra.Trade.Common.Functions.Validation;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Dtos.Inbound;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Models;
using FluentValidation;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Application.Validators;

public sealed class MessageHeaderValidator : AbstractValidator<TradeEventMessageHeader>
{
    public MessageHeaderValidator() : base()
    {
        RuleFor(m => m.CausationId!)
            .Must(PlNotifierValidationHelpers.BeAGuid).WithMessage(PlNotifierValidationMessages.GuidField);

        RuleFor(m => m.ContentType!)
            .Equal(PlNotifierHeaderConstants.ContentType, StringComparer.OrdinalIgnoreCase).WithMessage(PlNotifierValidationMessages.EqualField);

        RuleFor(m => m.CorrelationId)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(ValidationMessages.NullField)
            .Must(PlNotifierValidationHelpers.BeAGuid).WithMessage(PlNotifierValidationMessages.GuidField);

        RuleFor(m => m.EntityKey)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(ValidationMessages.NullField);

        RuleFor(m => m.Label)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(ValidationMessages.NullField)
            .Equal(PlNotifierHeaderConstants.Label, StringComparer.OrdinalIgnoreCase).WithMessage(PlNotifierValidationMessages.EqualField);

        RuleFor(m => m.MessageId)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(ValidationMessages.NullField)
            .Must(PlNotifierValidationHelpers.BeAGuid).WithMessage(PlNotifierValidationMessages.GuidField);

        RuleFor(m => m.OrganisationId!)
            .Must(PlNotifierValidationHelpers.BeAGuid).WithMessage(PlNotifierValidationMessages.GuidField);

        RuleFor(m => m.PublisherId!)
            .Equal(PlNotifierHeaderConstants.PublisherId, StringComparer.OrdinalIgnoreCase).WithMessage(PlNotifierValidationMessages.EqualField);

        //RuleFor(m => m.Schema)
        //    .Cascade(CascadeMode.Stop)
        //    .NotNull().WithMessage(ValidationMessages.NullField)
        //    .Must(m => m == PlNotifierHeaderConstants.Schema);

        RuleFor(m => m.Status)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage(ValidationMessages.NullField)
            .Equal(PlNotifierHeaderConstants.Status, StringComparer.OrdinalIgnoreCase).WithMessage(PlNotifierValidationMessages.EqualField);

        RuleFor(m => m.TimestampUtc)
            .GreaterThan(0).WithMessage(ValidationMessages.NullField);

        RuleFor(m => m.Type)
            .Equal(EventType.Internal).WithMessage(PlNotifierValidationMessages.EqualField);
    }
}
