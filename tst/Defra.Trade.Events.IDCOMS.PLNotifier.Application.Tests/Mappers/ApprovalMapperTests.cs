// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

using System.Diagnostics.CodeAnalysis;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Mappers;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Application.Tests.Mappers;

[ExcludeFromCodeCoverage]
public sealed class ApprovalMapperTests
{
    private readonly IMapper _mapper;

    public ApprovalMapperTests()
    {
        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile<ApprovalProfile>());
        _mapper = mapperConfig.CreateMapper();
    }

    [Fact]
    public void ApprovalMaps()
    {
        // arrange
        var inboundApproval = new Inbound.Approval
        {
            ApplicationId = $"mock {Guid.NewGuid()}",
            ApprovalStatus = "approved"
        };

        // act
        var result = _mapper.Map<Models.Approval>(inboundApproval);

        // assert

        result.ApprovalStatus.ShouldBe(inboundApproval.ApprovalStatus);
        result.ApplicationId.ShouldBe(inboundApproval.ApplicationId);
    }
}
