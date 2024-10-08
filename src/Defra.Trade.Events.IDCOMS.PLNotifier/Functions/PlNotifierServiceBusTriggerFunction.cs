// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Diagnostics.CodeAnalysis;
using Azure.Messaging.ServiceBus;
using Defra.Trade.Common.Functions;
using Defra.Trade.Common.Functions.Interfaces;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Extensions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Logging;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Functions;

[ExcludeFromCodeCoverage(Justification = "Only testable output is logger extensions used which cannot be feasibly tested")]
public sealed class PlNotifierServiceBusTriggerFunction
{
    private readonly IBaseMessageProcessorService<Inbound.Approval> _baseMessageProcessorService;
    private readonly IMessageRetryService _retry;
    public PlNotifierServiceBusTriggerFunction(IBaseMessageProcessorService<Inbound.Approval> baseMessageProcessorService, IMessageRetryService retry)
    {
        ArgumentNullException.ThrowIfNull(baseMessageProcessorService);
        _baseMessageProcessorService = baseMessageProcessorService;
        _retry = retry;
    }

    [ServiceBusAccount(Models.PlNotifierSettings.ConnectionStringConfigurationKey)]
    [FunctionName(nameof(PlNotifierServiceBusTriggerFunction))]
    public async Task RunAsync(
        [ServiceBusTrigger(queueName: Models.PlNotifierSettings.DefaultQueueName, IsSessionsEnabled = false)] ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions,
        ExecutionContext executionContext,
        [ServiceBus(Models.PlNotifierSettings.TradeEventInfo)] IAsyncCollector<ServiceBusMessage> eventStoreCollector,
        [ServiceBus(Models.PlNotifierSettings.DefaultQueueName)] IAsyncCollector<ServiceBusMessage> retryQueue,
        ILogger logger)
    {
        logger.MessageReceived(message.MessageId, executionContext.FunctionName);
        _retry.SetContext(message, retryQueue);
        await RunInternal(message, messageActions, eventStoreCollector, executionContext, logger);

        logger.MessageProcessed(message.MessageId, executionContext.FunctionName);
    }

    private async Task RunInternal(
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions,
        IAsyncCollector<ServiceBusMessage> eventStoreCollector,
        ExecutionContext executionContext,
        ILogger logger)
    {
        try
        {
            await _baseMessageProcessorService.ProcessAsync(
                executionContext.InvocationId.ToString(),
                Models.PlNotifierSettings.DefaultQueueName,
                Models.PlNotifierSettings.PublisherId,
                message,
                messageActions,
                eventStoreCollector,
                originalCrmPublisherId: Models.PlNotifierSettings.PublisherId,
                originalSource: Models.PlNotifierSettings.DefaultQueueName,
                originalRequestName: "Update");
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, ex.Message);
        }
    }
}
