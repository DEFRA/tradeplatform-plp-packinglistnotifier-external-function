// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

using System.Diagnostics.CodeAnalysis;

using Defra.Trade.Common.Functions.Extensions;
using System.Net;
using Defra.Trade.Common.Functions.Interfaces;
using Defra.Trade.Common.Functions.Models;
using Defra.Trade.Crm;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Extensions;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Models;
using Microsoft.Extensions.Logging;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Application.Services;

public sealed class ApprovalMessageProcessor : IMessageProcessor<Models.Approval, TradeEventMessageHeader>
{
    private readonly ICrmClient _crmClient;
    private readonly ILogger<ApprovalMessageProcessor> _logger;
    private readonly TimeSpan messageRetryEnqueueTime;
    private readonly IMessageRetryContextAccessor _retry;

    public ApprovalMessageProcessor(
        ICrmClient crmClient,
        ILogger<ApprovalMessageProcessor> logger,
        IMessageRetryContextAccessor retry)
    {
        ArgumentNullException.ThrowIfNull(crmClient);
        ArgumentNullException.ThrowIfNull(logger);
        _crmClient = crmClient;
        _logger = logger;
        _retry = retry;
        messageRetryEnqueueTime = new TimeSpan(0, 0, 0, PlNotifierSettings.MessageRetry.EnqueueTimeSeconds);
    }

    public Task<CustomMessageHeader> BuildCustomMessageHeaderAsync() => Task.FromResult(new CustomMessageHeader());

    public Task<string> GetSchemaAsync(TradeEventMessageHeader messageHeader) => Task.FromResult(string.Empty);

    public async Task<StatusResponse<Models.Approval>> ProcessAsync(Models.Approval message, TradeEventMessageHeader messageHeader)
    {
        _logger.ProcessingNotification(message.ApplicationId!);
        return await ProcessInternalAsync(message, messageHeader);
    }

    [ExcludeFromCodeCoverage(Justification = "Unable to mock static logger method.")]
    public async Task<StatusResponse<Models.Approval>> ProcessInternalAsync(
        Models.Approval message,
        TradeEventMessageHeader messageHeader)
    {
        try
        {
            var dynamicsPayload = MapToDynamics(message);
            await SendToDynamics(dynamicsPayload);

            _logger.ProcessingNotificationSuccess(message.ApplicationId!);
        }
        catch (HttpRequestException ex) when (ex.StatusCode is null or 0 or (>= (HttpStatusCode)500 and <= (HttpStatusCode)599) && _retry.Context is { } context)
        {
            _logger.LogError(ex, context.Message.MessageId, context.Message.RetryCount());

            await context.RetryMessage(messageRetryEnqueueTime, messageRetryEnqueueTime, ex);
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
