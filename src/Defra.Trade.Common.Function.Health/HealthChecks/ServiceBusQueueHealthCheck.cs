using Microsoft.Azure.ServiceBus;
using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.Common.Function.Health.HealthChecks;

/// <summary>
/// Health check for Trade Api,
/// </summary>
[ExcludeFromCodeCoverage]
public class ServiceBusQueueHealthCheck : IHealthCheck
{
    private readonly string _queueName;
    private readonly string _serviceBusConConfig;

    public ServiceBusQueueHealthCheck(string serviceBusConConfig, string queueName)
    {
        _queueName = queueName;
        _serviceBusConConfig = serviceBusConConfig;
    }

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

#pragma warning disable IDE0060 // Remove unused parameter

    protected Task<HealthCheckResult> CheckHealthInternalAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        try
        {
            var name = context.Registration.Name;
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
