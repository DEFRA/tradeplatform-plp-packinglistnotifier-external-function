// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Models;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Application.Extensions;
public static class ApprovalExtensions
{

    public static bool IsRejectedWithoutReason(this Approval approval)
    {
        return approval.ApprovalStatus.Equals("rejected", StringComparison.OrdinalIgnoreCase) &&
            string.IsNullOrEmpty(approval.FailureReasons);
    }
}
