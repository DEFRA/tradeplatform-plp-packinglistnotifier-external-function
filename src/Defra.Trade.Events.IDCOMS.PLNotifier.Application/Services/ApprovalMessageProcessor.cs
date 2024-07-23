// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

using AutoMapper;
using Defra.Trade.Common.Functions.Interfaces;
using Inbound = Defra.Trade.Events.IDCOMS.PLNotifier.Application.Dtos.Inbound;
using Microsoft.Extensions.Logging;
using Defra.Trade.Common.Functions.Models;

using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Dtos.Inbound;

using Models = Defra.Trade.Events.IDCOMS.PLNotifier.Application.Models;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Application.Services;

public sealed class ApprovalMessageProcessor : IMessageProcessor<Models.Approval, TradeEventMessageHeader>
{
    private readonly ILogger<ApprovalMessageProcessor> _logger;
    private readonly IMapper _mapper; // TODO

    public ApprovalMessageProcessor(IMapper mapper, ILogger<ApprovalMessageProcessor> logger)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task<CustomMessageHeader> BuildCustomMessageHeaderAsync() => Task.FromResult(new CustomMessageHeader());

    public Task<string> GetSchemaAsync(TradeEventMessageHeader messageHeader) => Task.FromResult(string.Empty);

    public async Task<StatusResponse<Models.Approval>> ProcessAsync(Models.Approval message, TradeEventMessageHeader messageHeader)
    {
        try
        {
            var x = _mapper.Map<Approval>(message);

            await MapToDynamics();
            //await SendToDynamics();

            _logger.LogInformation(nameof(ProcessAsync));
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, ex.Message);
        }

        return new StatusResponse<Models.Approval>() { ForwardMessage = false, Response = message };
    }

    public Task<bool> ValidateMessageLabelAsync(TradeEventMessageHeader messageHeader)
        => Task.FromResult(messageHeader.Label.Equals(Models.PlNotifierHeaderConstants.Label, StringComparison.OrdinalIgnoreCase));

    private async Task MapToDynamics()
    {
        // TODO
    }

    private async Task SendToDynamics()
    {
        // TODO
    }
}
