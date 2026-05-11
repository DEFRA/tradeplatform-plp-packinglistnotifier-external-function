// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Azure.Messaging.ServiceBus;
using Defra.Trade.Common.Functions.Isolated;
using Defra.Trade.Common.Functions.Isolated.Interfaces;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Models;
using Defra.Trade.Events.IDCOMS.PLNotifier.Functions;
using Defra.Trade.Events.IDCOMS.PLNotifier.Tests.Helpers;
using FakeItEasy;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Approval = Defra.Trade.Events.IDCOMS.PLNotifier.Application.Dtos.Inbound.Approval;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Tests.Functions;

public class PlNotifierServiceBusTriggerFunctionTests
{
    private readonly IBaseMessageProcessorService<Approval> _processor;
    private readonly IMessageRetryService _retry;
    private readonly PlNotifierServiceBusTriggerFunction _sut;
    private readonly ServiceBusClient _sbClient;
    private readonly ILogger<PlNotifierServiceBusTriggerFunction> _logger;

    public PlNotifierServiceBusTriggerFunctionTests()
    {
        _processor = A.Fake<IBaseMessageProcessorService<Approval>>(opt => opt.Strict());
        _retry = A.Fake<IMessageRetryService>(opt => opt.Strict());
        _sbClient = A.Fake<ServiceBusClient>(opt => opt.Strict());
        _logger = A.Fake<ILogger<PlNotifierServiceBusTriggerFunction>>(opt => opt.Strict());
        _sut = new PlNotifierServiceBusTriggerFunction(_processor, _retry, _sbClient, _logger);

    }

    [Fact]
    public async Task RunAsync_CallsTheProcessorWithTheCorrectArguments()
    {
        // arrange
        string messageId = Guid.NewGuid().ToString();
        var invocationId = Guid.NewGuid();
        string functionName = Guid.NewGuid().ToString();
        var message = ServiceBusModelFactory.ServiceBusReceivedMessage(messageId: messageId, body: BinaryData.FromString("{\"GCId\": \"123\"}"));

        var actions = A.Fake<ServiceBusMessageActions>(opt => opt.Strict());
        var context = A.Fake<FunctionContext>();
        var functionDefinition = A.Fake<FunctionDefinition>();
        A.CallTo(() => context.InvocationId).Returns(invocationId.ToString());
        A.CallTo(() => context.FunctionDefinition).Returns(functionDefinition);
        A.CallTo(() => functionDefinition.Name).Returns(functionName);
        var eventStore = A.Fake<ServiceBusSender>(opt => opt.Strict());
        A.CallTo(() => _sbClient.CreateSender(PlNotifierSettings.TradeEventInfo)).Returns(eventStore);
        var retryQueue = A.Fake<ServiceBusSender>(opt => opt.Strict());
        A.CallTo(() => _sbClient.CreateSender(PlNotifierSettings.DefaultQueueName)).Returns(retryQueue);

        var setRetryContext = A.CallTo(() => _retry.SetContext(message, retryQueue));
        var processAsyncCall = A.CallTo(() => _processor.ProcessAsync(
            invocationId.ToString(),
            PlNotifierSettings.DefaultQueueName,
            PlNotifierSettings.PublisherId,
            message,
            actions,
            eventStore,
            (string)null,
            PlNotifierSettings.PublisherId,
            PlNotifierSettings.DefaultQueueName,
            "Update"
        ));

        var loggerStart = LoggerFakeHelper.LoggerCall(_logger, LogLevel.Information, 0, null, "Messages Id : {MessageId} received on {FunctionName}", () => new[] { messageId, functionName });
        var loggerReceived = LoggerFakeHelper.LoggerCall(_logger, LogLevel.Information, 0, null, "Message Id : {MessageId} received on {FunctionName}", () => new[] { "123", functionName });
        var loggerEnd = LoggerFakeHelper.LoggerCall(_logger, LogLevel.Information, 0, null, "Finished processing Messages Id : {MessageId} received on {FunctionName}", () => new[] { messageId, functionName });

        processAsyncCall.Returns(true);
        loggerStart.DoesNothing();
        loggerReceived.DoesNothing();
        loggerEnd.DoesNothing();
        setRetryContext.DoesNothing();

        // act
        await _sut.RunAsync(message, actions, context);

        // assert
        setRetryContext.MustHaveHappenedOnceExactly()
            .Then(processAsyncCall.MustHaveHappenedOnceExactly());
    }

    [Fact]
    public async Task RunAsync_LogsWhenTheProcessorThrows()
    {
        // arrange
        string messageId = Guid.NewGuid().ToString();
        var invocationId = Guid.NewGuid();
        string functionName = Guid.NewGuid().ToString();
        var message = ServiceBusModelFactory.ServiceBusReceivedMessage(messageId: messageId);
        var actions = A.Fake<ServiceBusMessageActions>(opt => opt.Strict());
        var context = A.Fake<FunctionContext>();
        var functionDefinition = A.Fake<FunctionDefinition>();
        A.CallTo(() => context.InvocationId).Returns(invocationId.ToString());
        A.CallTo(() => context.FunctionDefinition).Returns(functionDefinition);
        A.CallTo(() => functionDefinition.Name).Returns(functionName);
        var eventStore = A.Fake<ServiceBusSender>(opt => opt.Strict());
        A.CallTo(() => _sbClient.CreateSender(PlNotifierSettings.TradeEventInfo)).Returns(eventStore);
        var retryQueue = A.Fake<ServiceBusSender>(opt => opt.Strict());
        A.CallTo(() => _sbClient.CreateSender(PlNotifierSettings.DefaultQueueName)).Returns(retryQueue);
        var exception = new Exception("abc");

        var setRetryContext = A.CallTo(() => _retry.SetContext(message, retryQueue));
        var processAsyncCall = A.CallTo(() => _processor.ProcessAsync(
            invocationId.ToString(),
            PlNotifierSettings.DefaultQueueName,
            PlNotifierSettings.PublisherId,
            message,
            actions,
            eventStore,
            (string)null,
            PlNotifierSettings.PublisherId,
            PlNotifierSettings.DefaultQueueName,
            "Update"
        ));
        var loggerStart = LoggerFakeHelper.LoggerCall(_logger, LogLevel.Information, 0, null, "Messages Id : {MessageId} received on {FunctionName}", () => new[] { messageId, functionName });
        var loggerNoGcId = LoggerFakeHelper.LoggerCall(_logger, LogLevel.Warning, 0, null, "The incoming message does not have a GcId");
        var loggerError = LoggerFakeHelper.LoggerCall(_logger, LogLevel.Critical, 0, exception, "abc");

        processAsyncCall.Returns(true);
        loggerNoGcId.DoesNothing();
        loggerStart.Throws(exception);
        loggerError.DoesNothing();
        setRetryContext.DoesNothing();

        // act
        await _sut.RunAsync(message, actions, context);

        // assert
        setRetryContext.MustHaveHappenedOnceExactly();
    }
}
