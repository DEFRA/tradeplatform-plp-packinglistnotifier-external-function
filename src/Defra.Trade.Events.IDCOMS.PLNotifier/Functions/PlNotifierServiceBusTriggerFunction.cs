// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Diagnostics.CodeAnalysis;
using Azure.Messaging.ServiceBus;
using Defra.Trade.Common.Functions.Isolated;
using Defra.Trade.Common.Functions.Isolated.Interfaces;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Functions;

[ExcludeFromCodeCoverage(Justification = "Only testable output is logger extensions used which cannot be feasibly tested")]
public sealed class PlNotifierServiceBusTriggerFunction
{
    private readonly IBaseMessageProcessorService<Inbound.Approval> _baseMessageProcessorService;
    private readonly IMessageRetryService _retry;
    private readonly ServiceBusClient _serviceBusClient;
    private readonly ILogger<PlNotifierServiceBusTriggerFunction> _logger;

    public PlNotifierServiceBusTriggerFunction(IBaseMessageProcessorService<Inbound.Approval> baseMessageProcessorService, IMessageRetryService retry, ServiceBusClient serviceBusClient, ILogger<PlNotifierServiceBusTriggerFunction> logger)
    {
        ArgumentNullException.ThrowIfNull(baseMessageProcessorService);
        _baseMessageProcessorService = baseMessageProcessorService;
        _retry = retry;
        _serviceBusClient = serviceBusClient;
        _logger = logger;
    }

    [Function(nameof(PlNotifierServiceBusTriggerFunction))]
    public async Task RunAsync(
        [ServiceBusTrigger(queueName: Models.PlNotifierSettings.DefaultQueueName,
            IsSessionsEnabled = false,
            Connection = Models.PlNotifierSettings.ConnectionStringConfigurationKey)]
            ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions,
        FunctionContext context)
    {
        _logger.MessageReceived(message.MessageId, context.FunctionDefinition.Name);
        
        await RunInternal(message, messageActions, context);

        _logger.MessageProcessed(message.MessageId, context.FunctionDefinition.Name);
    }

    private async Task RunInternal(
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions,
        FunctionContext context)
    {
        try
        {
            var retrySender = _serviceBusClient.CreateSender(Models.PlNotifierSettings.DefaultQueueName);
            _retry.SetContext(message, retrySender);
            var eventStoreSender = _serviceBusClient.CreateSender(Models.PlNotifierSettings.TradeEventInfo);

            await _baseMessageProcessorService.ProcessAsync(
                context.InvocationId.ToString(),
                Models.PlNotifierSettings.DefaultQueueName,
                Models.PlNotifierSettings.PublisherId,
                message,
                messageActions,
                eventStoreSender,
                originalCrmPublisherId: Models.PlNotifierSettings.PublisherId,
                originalSource: Models.PlNotifierSettings.DefaultQueueName,
                originalRequestName: "Update");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, ex.Message);
        }
    }
}
