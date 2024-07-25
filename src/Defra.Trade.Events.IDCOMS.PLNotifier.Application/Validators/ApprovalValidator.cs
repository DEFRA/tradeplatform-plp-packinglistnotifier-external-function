// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

using Defra.Trade.Common.Functions.Validation;
using FluentValidation;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Application.Validators;

public sealed class ApprovalValidator : AbstractValidator<Inbound.Approval>
{
    private readonly List<string> _approvalStatus = new() { "approved", "rejected" };

    public ApprovalValidator()
    {
        RuleFor(m => m.ApplicationId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ValidationMessages.NullField);

        RuleFor(m => m.ApprovalStatus)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(ValidationMessages.NullField)
            .Must(BeApprovalStatus!).WithMessage(PlNotifierValidationMessages.ApprovalStatus);
    }

    private bool BeApprovalStatus(string status)
    {
        return _approvalStatus.Contains(status.ToLower());
    }
}
