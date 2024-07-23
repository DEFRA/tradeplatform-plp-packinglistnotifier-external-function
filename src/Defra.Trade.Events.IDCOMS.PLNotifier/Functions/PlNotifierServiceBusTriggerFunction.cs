// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Defra.Trade.Common.Functions;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Extensions;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Logging;
using Inbound = Defra.Trade.Events.IDCOMS.PLNotifier.Application.Dtos.Inbound;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Functions;

public sealed class PlNotifierServiceBusTriggerFunction
{
    private readonly IBaseMessageProcessorService<Inbound.Approval> _baseMessageProcessorService;

    public PlNotifierServiceBusTriggerFunction(IBaseMessageProcessorService<Inbound.Approval> baseMessageProcessorService)
    {
        ArgumentNullException.ThrowIfNull(baseMessageProcessorService);
        _baseMessageProcessorService = baseMessageProcessorService;
    }

    [ServiceBusAccount(PlNotifierSettings.ConnectionStringConfigurationKey)]
    [FunctionName(nameof(PlNotifierServiceBusTriggerFunction))]
    public async Task Run(
        [ServiceBusTrigger(queueName: PlNotifierSettings.DefaultQueueName, IsSessionsEnabled = false)] ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions,
        ExecutionContext executionContext,
        [ServiceBus(PlNotifierSettings.TradeEventInfo)] IAsyncCollector<ServiceBusMessage> eventStoreCollector,
        ILogger logger)
    {
        logger.MessageReceived(message.MessageId, executionContext.FunctionName);

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
                PlNotifierSettings.DefaultQueueName,
                PlNotifierSettings.PublisherId,
                message,
                messageActions,
                eventStoreCollector,
                originalCrmPublisherId: PlNotifierSettings.PublisherId,
                originalSource: PlNotifierSettings.DefaultQueueName,
                originalRequestName: "Update");
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, ex.Message);
        }
    }
}
