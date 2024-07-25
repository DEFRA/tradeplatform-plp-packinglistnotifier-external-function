// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

using Defra.Trade.Crm.Metadata;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Application.Dtos.Dynamics;

[CrmEntity("trd_exportapplication")]
public sealed class ApprovalPayload
{
    [CrmKey]
    [CrmProperty("trd_exportapplicationid")]
    public Guid ExportApplicationId { get; set; }

    [CrmProperty("trd_applicationreference")]
    public string? ApplicationId { get; set; }

    [CrmProperty("rms_automatedpackinglistcheck")]
    public int? ApprovalStatus { get; set; }
}
