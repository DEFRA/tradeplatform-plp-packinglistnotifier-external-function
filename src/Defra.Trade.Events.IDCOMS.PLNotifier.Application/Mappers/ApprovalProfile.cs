// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Application.Mappers;

public class ApprovalProfile : Profile
{
    public ApprovalProfile()
    {
        CreateMap<Inbound.Approval, Models.Approval>();
    }
}
