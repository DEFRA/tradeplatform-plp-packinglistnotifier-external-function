// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

using Defra.Trade.Common.Functions.Interfaces;
using Defra.Trade.Common.Functions.Models;
using Defra.Trade.Crm;
using Defra.Trade.Crm.Exceptions;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Extensions;
using Microsoft.Extensions.Logging;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Application.Services;

public sealed class ApprovalMessageProcessor : IMessageProcessor<Models.Approval, TradeEventMessageHeader>
{
    private readonly ICrmClient _crmClient;
    private readonly ILogger<ApprovalMessageProcessor> _logger;

    public ApprovalMessageProcessor(
        ICrmClient crmClient,
        ILogger<ApprovalMessageProcessor> logger)
    {
        ArgumentNullException.ThrowIfNull(crmClient);
        ArgumentNullException.ThrowIfNull(logger);
        _crmClient = crmClient;
        _logger = logger;
    }

    public Task<CustomMessageHeader> BuildCustomMessageHeaderAsync() => Task.FromResult(new CustomMessageHeader());

    public Task<string> GetSchemaAsync(TradeEventMessageHeader messageHeader) => Task.FromResult(string.Empty);

    public async Task<StatusResponse<Models.Approval>> ProcessAsync(Models.Approval message, TradeEventMessageHeader messageHeader)
    {
        _logger.ProcessingNotification(message.ApplicationId!);

        try
        {
            var dynamicsPayload = MapToDynamics(message);
            await SendToDynamics(dynamicsPayload);

            _logger.ProcessingNotificationSuccess(message.ApplicationId!);
        }
        catch (ArgumentOutOfRangeException ex)
        {
            _logger.MapToDynamicsFailure(ex, message.ApplicationId!);
            throw;
        }
        catch (CrmException ex)
        {
            _logger.SendPayloadFailure(ex, message.ApplicationId!);
            throw;
        }
        catch (Exception ex)
        {
            _logger.ProcessingNotificationFailure(ex, message.ApplicationId!);
            throw;
        }

        return new StatusResponse<Models.Approval>() { ForwardMessage = false, Response = message };
    }

    public Task<bool> ValidateMessageLabelAsync(TradeEventMessageHeader messageHeader)
        => Task.FromResult(messageHeader.Label!.Equals(Models.PlNotifierHeaderConstants.Label, StringComparison.OrdinalIgnoreCase));

    private Dynamics.ApprovalPayload MapToDynamics(Models.Approval message)
    {
        _logger.MapToDynamics(message.ApplicationId!);

        var dynamicsApprovalStatus = message.ApprovalStatus!.ToLower() switch
        {
            "approved" => Dynamics.ApprovalStatus.Approved,
            "rejected" => Dynamics.ApprovalStatus.Rejected,
            _ => throw new ArgumentOutOfRangeException(nameof(message))
        };

        var payload = new Dynamics.ApprovalPayload
        {
            ApplicationId = message.ApplicationId,
            ApprovalStatus = (int)dynamicsApprovalStatus
        };

        _logger.MapToDynamicsSuccess(payload.ApplicationId!);

        return payload;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Bug", "S1751:Loops with at most one iteration should be refactored", Justification = "Crm client library only provides IAsyncEnumerator for get action, and only 1 result is possible")]
    private async Task SendToDynamics(Dynamics.ApprovalPayload payload)
    {
        _logger.SendPayload(payload.ApplicationId!);

        _logger.GetExportApplicationId(payload.ApplicationId!);

        var exportAppEnumerator = _crmClient.ListPagedAsync<Dynamics.ApprovalPayload>($"trd_applicationreference eq '{payload.ApplicationId}'", default);

        await foreach (var exportApp in exportAppEnumerator)
        {
            payload.ExportApplicationId = exportApp.First().ExportApplicationId;
            break;
        }

        _logger.GetExportApplicationIdSuccess(payload.ApplicationId!, payload.ExportApplicationId!);

        await _crmClient.UpdateAsync(payload);

        _logger.SendPayloadSuccess(payload.ApplicationId!);
    }
}
