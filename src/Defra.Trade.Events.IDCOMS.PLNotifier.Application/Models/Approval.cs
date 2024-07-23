﻿// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Application.Models;

public sealed class Approval
{
    // Entity key
    public string? ApplicationId { get; set; }

    public string? ApprovalStatus { get; set; }
}
