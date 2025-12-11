// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.ComponentModel;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Application.Dtos.Dynamics;

public enum ApprovalStatus
{
    [Description("approved")]
    Approved = 179640000,

    [Description("rejected")]
    Rejected = 179640001,

    [Description("rejected_ineligible")]
    Rejected_Ineligible = 179640002,

    [Description("rejected_coo")]
    Rejected_Coo = 179640003,

    [Description("rejected_other")]
    Rejected_Other = 179640004
}
