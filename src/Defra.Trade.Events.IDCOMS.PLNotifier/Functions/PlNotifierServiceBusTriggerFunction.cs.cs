// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Extensions;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Logging;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Functions;

public sealed class PlNotifierServiceBusTriggerFunction

{
    [ServiceBusAccount(PlNotifierSettings.ConnectionStringConfigurationKey)]
    [FunctionName(nameof(PlNotifierServiceBusTriggerFunction))]
    public async Task RunAsync(
        [ServiceBusTrigger(queueName: PlNotifierSettings.DefaultQueueName, IsSessionsEnabled = true)] ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions,
        ExecutionContext executionContext,
        [ServiceBus(PlNotifierSettings.TradeEventInfo)] IAsyncCollector<ServiceBusMessage> eventStoreCollector,
        ILogger logger)
    {
        await RunInternalAsync(message, messageActions, eventStoreCollector, executionContext, logger);
    }

    private async Task RunInternalAsync(
    ServiceBusReceivedMessage message,
    ServiceBusMessageActions messageReceiver,
    IAsyncCollector<ServiceBusMessage> eventStoreCollector,
    ExecutionContext executionContext,
    ILogger logger)
    {
        try
        {
            logger.MessageReceived(message.MessageId, executionContext.FunctionName);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, ex.Message);
        }
    }
}
