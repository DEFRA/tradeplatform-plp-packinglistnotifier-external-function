// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using Defra.Trade.Common.Function.Health.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Functions;

public sealed class HealthCheckHttpTriggerFunction
{
    private readonly HealthCheckService _healthCheckService;

    public HealthCheckHttpTriggerFunction(HealthCheckService healthCheckService)
    {
        ArgumentNullException.ThrowIfNull(healthCheckService);
        _healthCheckService = healthCheckService;
    }

    [FunctionName("HealthCheckHttpTriggerFunction")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Required for function processing")]
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequest request)
    {
        var healthReport = await _healthCheckService.CheckHealthAsync();

        if (healthReport.Status == HealthStatus.Healthy)
        {
            return new JsonResult("Healthy");
        }

        var healthCheckResponse = healthReport.ToResponse();

        return new JsonResult(healthCheckResponse);
    }
}
