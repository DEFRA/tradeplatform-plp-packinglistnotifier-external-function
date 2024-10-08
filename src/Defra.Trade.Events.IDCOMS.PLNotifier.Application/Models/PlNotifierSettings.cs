﻿// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Application.Models;

[ExcludeFromCodeCoverage(Justification = "No value in covering startup")]
public sealed class PlNotifierSettings
{
    public const string PlNotifierSettingsName = "PLP";

#if DEBUG

    // In 'Debug' (locally) use connection string
    public const string ConnectionStringConfigurationKey = "ServiceBus:ConnectionString";

#else
    // Assumes that this is 'Release' and uses Managed Identity rather than connection string
    // ie it will actually bind to ServiceBus:FullyQualifiedNamespace !
    public const string ConnectionStringConfigurationKey = "ServiceBus";
#endif

    public const string DefaultQueueName = "defra.trade.plp.parsed";
    public const string PublisherId = PlNotifierHeaderConstants.PublisherId;
    public const string TradeEventInfo = Common.Functions.Constants.QueueName.DefaultEventsInfoQueueName;
    public const string AppConfigSentinelName = "Sentinel";
    public string PlNotifierQueue { get; set; } = DefaultQueueName;

    public static class MessageRetry
    {
        public const int RetryWindowSeconds = 300;
        public const int EnqueueTimeSeconds = 30;
    }
}
