// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

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
    public async Task<IActionResult> RunAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequest request)
    {
        var healthReport = await _healthCheckService.CheckHealthAsync();

        return new OkObjectResult(Enum.GetName(typeof(HealthStatus), healthReport.Status));
    }
}
