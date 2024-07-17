// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

using Microsoft.Extensions.Logging;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Application.Extensions;

public static partial class LoggerExtensions
{
    [LoggerMessage(EventId = 0, EventName = nameof(MessageReceived), Level = LogLevel.Information, Message = "Message Id : {MessageId} received on {FunctionName}")]
    public static partial void MessageReceived(this ILogger logger, string messageId, string functionName);
}
