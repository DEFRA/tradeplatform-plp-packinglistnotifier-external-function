// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

using AutoMapper;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Models;
using Inbound = Defra.Trade.Events.IDCOMS.PLNotifier.Application.Dtos.Inbound;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Application.Mappers;

public class ApprovalProfile : Profile
{
    public ApprovalProfile()
    {
        CreateMap<Inbound.Approval, Approval>()
            .ForMember(dest => dest.ApplicationId, opt => opt.MapFrom(src => src.ApplicationId))
            .ForMember(dest => dest.ApprovalStatus, opt => opt.MapFrom(src => src.ApprovalStatus));
    }
}
