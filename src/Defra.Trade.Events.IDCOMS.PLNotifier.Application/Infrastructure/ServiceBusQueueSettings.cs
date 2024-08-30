// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Diagnostics.CodeAnalysis;
using Defra.Trade.Common.Config;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Application.Infrastructure;

[ExcludeFromCodeCoverage(Justification = "No value in covering startup")]
public sealed class ServiceBusQueuesSettings : ServiceBusSettings
{
    public string? PlNotifierQueueName { get; set; }
}
