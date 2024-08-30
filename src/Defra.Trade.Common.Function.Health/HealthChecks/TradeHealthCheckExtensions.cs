// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.Common.Function.Health.HealthChecks;

[ExcludeFromCodeCoverage(Justification = "Namespace to be tested within and moved to nuget")]
public static class TradeHealthCheckExtensions
{
    public static IHealthChecksBuilder AddAzureServiceBusCheck(
        this IHealthChecksBuilder builder,
        IConfiguration configuration,
        string serviceBusConnectionConfigPath,
        string queueName)
    {
        string? servicesBusConnectionString = configuration.GetValue<string>(serviceBusConnectionConfigPath);
        string servicesBusQueueName = queueName;

        builder.Add(new HealthCheckRegistration(
           $"ServiceBus:{queueName}",
            sp => new ServiceBusQueueHealthCheck(servicesBusConnectionString!, servicesBusQueueName),
            failureStatus: default,
            tags: default,
            timeout: default));
        return builder;
    }

    public static IHealthChecksBuilder AddDynamicsCheck(
        this IHealthChecksBuilder builder,
        ServiceProvider serviceProvider)
    {
        builder.Add(new HealthCheckRegistration(
            "Dynamics",
            sp => new DynamicsHealthCheck(serviceProvider),
            failureStatus: default,
            tags: default,
            timeout: default));

        return builder;
    }
}
