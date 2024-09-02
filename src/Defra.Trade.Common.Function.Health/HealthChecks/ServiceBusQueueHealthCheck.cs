// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure.ServiceBus;

namespace Defra.Trade.Common.Function.Health.HealthChecks;

/// <summary>
/// Health check for Trade Api,
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Namespace to be tested within and moved to nuget")]
public class ServiceBusQueueHealthCheck(string serviceBusConConfig, string queueName) : IHealthCheck
{
    private readonly string _queueName = queueName;
    private readonly string _serviceBusConConfig = serviceBusConConfig;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        await Task.Delay(5, cancellationToken).ConfigureAwait(false);
        var result = await ExecuteCheckAsync(context, cancellationToken);
        return result;
    }

    private async Task<HealthCheckResult> ExecuteCheckAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        HealthCheckResult result;
        try
        {
            result = await CheckHealthInternalAsync(context, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            result = HealthCheckResult.Unhealthy("The health check operation timed out");
        }
        catch (Exception ex)
        {
            result = HealthCheckResult.Unhealthy($"Exception during check: {ex.GetType().FullName}", ex);
        }

        return result;
    }

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Not yet used")]
    protected Task<HealthCheckResult> CheckHealthInternalAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            string name = context.Registration.Name;
            var client = CreateQueueClient(_serviceBusConConfig);
            _ = client.ServiceBusConnection.Endpoint;
            return Task.FromResult(HealthCheckResult.Healthy($"{name} Service bus connection successful."));
        }
        catch (Exception ex)
        {
            var data = new Dictionary<string, object> { { "url", _queueName + "/health" } };
            return Task.FromResult(HealthCheckResult.Unhealthy($"Exception during check: {ex.GetType().FullName}", ex, data));
        }
    }

    public IQueueClient CreateQueueClient(string connectionString)
    {
        return new QueueClient(connectionString, _queueName, ReceiveMode.PeekLock, RetryExponential.Default);
    }
}
