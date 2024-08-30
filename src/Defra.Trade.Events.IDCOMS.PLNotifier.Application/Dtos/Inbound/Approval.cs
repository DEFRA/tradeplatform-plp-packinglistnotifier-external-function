// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Application.Dtos.Inbound;

public sealed class Approval
{
    // Entity key
    public string? ApplicationId { get; set; }

    public string? ApprovalStatus { get; set; }
}
