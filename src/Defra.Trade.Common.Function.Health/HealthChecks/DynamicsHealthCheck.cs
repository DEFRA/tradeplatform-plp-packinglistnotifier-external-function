// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

using System.Diagnostics.CodeAnalysis;
using Defra.Trade.Common.Function.Health.DynamicsClient;
using Microsoft.Extensions.Options;

namespace Defra.Trade.Common.Function.Health.HealthChecks;

[ExcludeFromCodeCoverage(Justification = "Namespace to be tested within and moved to nuget")]
public class DynamicsHealthCheck : IHealthCheck
{
    private readonly ServiceProvider _serviceProvider;

    public DynamicsHealthCheck(ServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        _serviceProvider = serviceProvider;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        await Task.Delay(5, cancellationToken).ConfigureAwait(false);
        var result = await ExecuteCheckAsync(context, cancellationToken);
        return result;
    }

    protected async Task<HealthCheckResult> CheckHealthInternalAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var name = context.Registration.Name;
            var dynamicsAuthenticator = _serviceProvider.GetRequiredService<IDynamicsClientAuthenticator>();
            var dynamicsApiConfig = _serviceProvider.GetRequiredService<IOptions<DynamicsClientConfig>>();
            var dynamicsEndpoint = $"{dynamicsApiConfig.Value.Resource}{dynamicsApiConfig.Value.Path}{dynamicsApiConfig.Value.Query}";
            var authToken = await dynamicsAuthenticator.GenerateAuthenticationTokenAsync();

            var apiClient = new HttpClient();
            apiClient.DefaultRequestHeaders.Clear();
            apiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {authToken.AccessToken}");
            var dynamicsResponse = await apiClient.GetAsync(dynamicsEndpoint, cancellationToken).ConfigureAwait(false);

            var responseData = new Dictionary<string, object>
            {
                { "url", dynamicsEndpoint},
                { "name", name},
                { "StatusCode", dynamicsResponse.StatusCode}
            };

            return dynamicsResponse.IsSuccessStatusCode
                ? HealthCheckResult.Healthy("Healthy", responseData)
                : HealthCheckResult.Unhealthy($"Exception during dynamics check with status code: {dynamicsResponse.StatusCode}", null, responseData);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"Exception during check: {ex.GetType().FullName}", ex, new Dictionary<string, object> { { "url", "dynamics/IDCOMs" } });
        }
    }

    private async Task<HealthCheckResult> ExecuteCheckAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        HealthCheckResult result;
        try
        {
            result = await CheckHealthInternalAsync(context, cancellationToken).ConfigureAwait(false);
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
}
