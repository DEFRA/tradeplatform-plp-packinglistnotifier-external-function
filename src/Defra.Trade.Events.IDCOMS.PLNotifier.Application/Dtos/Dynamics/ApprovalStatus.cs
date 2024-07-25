// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

using System.ComponentModel;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Application.Dtos.Dynamics;

public enum ApprovalStatus
{
    [Description("approved")]
    Approved = 179640000,

    [Description("rejected")]
    Rejected = 179640001
}
