// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Application.Extensions;

[ExcludeFromCodeCoverage(Justification = "Extensions cannot be feasibly tested")]
public static partial class LoggerExtensions
{
    [LoggerMessage(EventId = 0, EventName = nameof(MessageReceived), Level = LogLevel.Information, Message = "Message Id : {MessageId} received on {FunctionName}")]
    public static partial void MessageReceived(this ILogger logger, string messageId, string functionName);

    [LoggerMessage(EventId = 1, EventName = nameof(MessageProcessed), Level = LogLevel.Information, Message = "Message Id : {MessageId} completed processing on {FunctionName}")]
    public static partial void MessageProcessed(this ILogger logger, string messageId, string functionName);

    [LoggerMessage(EventId = 5, EventName = nameof(MapToDynamics), Level = LogLevel.Information, Message = "Mapping PL data for application Id : {ApplicationId} to dynamics payload")]
    public static partial void MapToDynamics(this ILogger logger, string applicationId);

    [LoggerMessage(EventId = 6, EventName = nameof(MapToDynamicsSuccess), Level = LogLevel.Information, Message = "Successfully mapped PL data for application Id : {ApplicationId} to dynamics payload")]
    public static partial void MapToDynamicsSuccess(this ILogger logger, string applicationId);

    [LoggerMessage(EventId = 7, EventName = nameof(MapToDynamicsFailure), Level = LogLevel.Error, Message = "Failed to map PL data for application Id : {ApplicationId} to dynamics payload")]
    public static partial void MapToDynamicsFailure(this ILogger logger, Exception ex, string applicationId);

    [LoggerMessage(EventId = 10, EventName = nameof(SendPayload), Level = LogLevel.Information, Message = "Sending PL payload for application Id : {ApplicationId} to dynamics")]
    public static partial void SendPayload(this ILogger logger, string applicationId);

    [LoggerMessage(EventId = 11, EventName = nameof(SendPayloadSuccess), Level = LogLevel.Information, Message = "Successfully sent PL payload for application Id : {ApplicationId} to dynamics")]
    public static partial void SendPayloadSuccess(this ILogger logger, string applicationId);

    [LoggerMessage(EventId = 12, EventName = nameof(SendPayloadFailure), Level = LogLevel.Error, Message = "Failed to send PL payload for application Id : {ApplicationId} to dynamics")]
    public static partial void SendPayloadFailure(this ILogger logger, Exception ex, string applicationId);

    [LoggerMessage(EventId = 15, EventName = nameof(GetExportApplicationId), Level = LogLevel.Information, Message = "Getting export application Id for application Id : {ApplicationId} from dynamics")]
    public static partial void GetExportApplicationId(this ILogger logger, string applicationId);

    [LoggerMessage(EventId = 16, EventName = nameof(GetExportApplicationIdSuccess), Level = LogLevel.Information, Message = "Got export application Id : {ExportApplicationID} for application Id : {ApplicationId} from dynamics")]
    public static partial void GetExportApplicationIdSuccess(this ILogger logger, string applicationId, Guid exportApplicationId);

    [LoggerMessage(EventId = 20, EventName = nameof(ProcessingNotification), Level = LogLevel.Information, Message = "Processing PL notification for application Id : {ApplicationId}")]
    public static partial void ProcessingNotification(this ILogger logger, string applicationId);

    [LoggerMessage(EventId = 21, EventName = nameof(ProcessingNotificationSuccess), Level = LogLevel.Information, Message = "Successfully processed PL notification for application Id : {ApplicationId}")]
    public static partial void ProcessingNotificationSuccess(this ILogger logger, string applicationId);

    [LoggerMessage(EventId = 22, EventName = nameof(ProcessingNotificationFailure), Level = LogLevel.Error, Message = "Failed to process PL notification for application Id : {ApplicationId}")]
    public static partial void ProcessingNotificationFailure(this ILogger logger, Exception ex, string applicationId, int retryCount);

    [LoggerMessage(EventId = 23, EventName = nameof(ProcessingNotificationRetry), Level = LogLevel.Error, Message = "Failed to process PL notification for application Id : {ApplicationId} attempting to retry message with retry count of {RetryCount}")]
    public static partial void ProcessingNotificationRetry(this ILogger logger, Exception ex, string applicationId, int retryCount);
}

