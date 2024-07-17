// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Functions;

public static class HealthCheckHttpTriggerFunction
{
    [FunctionName("HealthCheckHttpTriggerFunction")]
    public static IActionResult Health(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
    {
        return new StatusCodeResult(StatusCodes.Status200OK);
    }
}
