// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

using Defra.Trade.Common.Config;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Application.Infrastructure;

public sealed class ServiceBusQueuesSettings : ServiceBusSettings
{
    public string? PlNotifierQueueName { get; set; }
}
