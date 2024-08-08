// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

using Azure.Messaging.ServiceBus;
using Defra.Trade.Common.Functions.Interfaces;
using Defra.Trade.Common.Functions;
using FakeItEasy;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Azure.WebJobs;
using Defra.Trade.Events.IDCOMS.PLNotifier.Functions;
using Microsoft.Extensions.Logging;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Models;
using Defra.Trade.Events.IDCOMS.PLNotifier.Tests.Helpers;
using Approval = Defra.Trade.Events.IDCOMS.PLNotifier.Application.Dtos.Inbound.Approval;
namespace Defra.Trade.Events.IDCOMS.PLNotifier.Tests.Functions;

public class PlNotifierServiceBusTriggerFunctionTests
{
    private readonly IBaseMessageProcessorService<Approval> processor;
    private readonly IMessageRetryService retry;
    private readonly PlNotifierServiceBusTriggerFunction sut;

    public PlNotifierServiceBusTriggerFunctionTests()
    {
        processor = A.Fake<IBaseMessageProcessorService<Approval>>(opt => opt.Strict());
        retry = A.Fake<IMessageRetryService>(opt => opt.Strict());
        sut = new PlNotifierServiceBusTriggerFunction(processor, retry);
    }

    [Fact]
    public async Task RunAsync_CallsTheProcessorWithTheCorrectArguments()
    {
        // arrange
        var messageId = Guid.NewGuid().ToString();
        var invocationId = Guid.NewGuid();
        var functionName = Guid.NewGuid().ToString();
        var message = ServiceBusModelFactory.ServiceBusReceivedMessage(messageId: messageId, body: BinaryData.FromString("{\"GCId\": \"123\"}"));

        var actions = A.Fake<ServiceBusMessageActions>(opt => opt.Strict());
        var context = new ExecutionContext { InvocationId = invocationId, FunctionName = functionName };
        var eventStore = A.Fake<IAsyncCollector<ServiceBusMessage>>(opt => opt.Strict());
        var retryQueue = A.Fake<IAsyncCollector<ServiceBusMessage>>(opt => opt.Strict());
        var logger = A.Fake<ILogger>();

        var setRetryContext = A.CallTo(() => retry.SetContext(message, retryQueue));
        var processAsyncCall = A.CallTo(() => processor.ProcessAsync(
            invocationId.ToString(),
            PlNotifierSettings.DefaultQueueName,
            PlNotifierSettings.PublisherId,
            message,
            actions,
            eventStore,
            null,
            null,
            PlNotifierSettings.PublisherId,
            PlNotifierSettings.DefaultQueueName,
            "Update"
        ));

        var loggerStart = LoggerFakeHelper.LoggerCall(logger, LogLevel.Information, 0, null, "Messages Id : {MessageId} received on {FunctionName}", () => new[] { messageId, functionName });
        var loggerReceived = LoggerFakeHelper.LoggerCall(logger, LogLevel.Information, 0, null, "Message Id : {MessageId} received on {FunctionName}", () => new[] { "123", functionName });
        var loggerEnd = LoggerFakeHelper.LoggerCall(logger, LogLevel.Information, 0, null, "Finished processing Messages Id : {MessageId} received on {FunctionName}", () => new[] { messageId, functionName });

        processAsyncCall.Returns(true);
        loggerStart.DoesNothing();
        loggerReceived.DoesNothing();
        loggerEnd.DoesNothing();
        setRetryContext.DoesNothing();

        // act
        await sut.Run(message, actions, context, eventStore, retryQueue, logger);

        // assert
        setRetryContext.MustHaveHappenedOnceExactly()
            .Then(processAsyncCall.MustHaveHappenedOnceExactly());
    }

    [Fact]
    public async Task RunAsync_LogsWhenTheProcessorThrows()
    {
        // arrange
        var messageId = Guid.NewGuid().ToString();
        var invocationId = Guid.NewGuid();
        var functionName = Guid.NewGuid().ToString();
        var message = ServiceBusModelFactory.ServiceBusReceivedMessage(messageId: messageId);
        var actions = A.Fake<ServiceBusMessageActions>(opt => opt.Strict());
        var context = new ExecutionContext { InvocationId = invocationId, FunctionName = functionName };
        var eventStore = A.Fake<IAsyncCollector<ServiceBusMessage>>(opt => opt.Strict());
        var retryQueue = A.Fake<IAsyncCollector<ServiceBusMessage>>(opt => opt.Strict());
        var logger = A.Fake<ILogger>();
        var exception = new Exception("abc");

        var setRetryContext = A.CallTo(() => retry.SetContext(message, retryQueue));
        var processAsyncCall = A.CallTo(() => processor.ProcessAsync(
            invocationId.ToString(),
            PlNotifierSettings.DefaultQueueName,
            PlNotifierSettings.PublisherId,
            message,
            actions,
            eventStore,
            null,
            null,
            PlNotifierSettings.PublisherId,
            PlNotifierSettings.DefaultQueueName,
            "Update"
        ));
        var loggerStart = LoggerFakeHelper.LoggerCall(logger, LogLevel.Information, 0, null, "Messages Id : {MessageId} received on {FunctionName}", () => new[] { messageId, functionName });
        var loggerNoGcId = LoggerFakeHelper.LoggerCall(logger, LogLevel.Warning, 0, null, "The incoming message does not have a GcId");
        var loggerError = LoggerFakeHelper.LoggerCall(logger, LogLevel.Critical, 0, exception, "abc");

        processAsyncCall.Returns(true);
        loggerNoGcId.DoesNothing();
        loggerStart.Throws(exception);
        loggerError.DoesNothing();
        setRetryContext.DoesNothing();

        // act
        await sut.Run(message, actions, context, eventStore, retryQueue, logger);

        // assert
        setRetryContext.MustHaveHappenedOnceExactly();
    }
}
