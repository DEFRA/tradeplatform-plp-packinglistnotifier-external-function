// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

using System.Text;
using Defra.Trade.Events.IDCOMS.PLNotifier.Functions;
using FakeItEasy;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Tests.Functions;

public sealed class HealthCheckHttpTriggerFunctionTests
{
    private readonly HealthCheckService _healthCheckService;
    private readonly HealthCheckHttpTriggerFunction _sut;

    public HealthCheckHttpTriggerFunctionTests()
    {
        _healthCheckService = A.Fake<HealthCheckService>();
        _sut = new HealthCheckHttpTriggerFunction(_healthCheckService);
    }

    [Fact]
    public void RunAsync_HasFunctionAttribute()
    {
        // Arrange & Act
        var attribute = FunctionTestHelpers.MethodHasSingleAttribute<HealthCheckFunction, FunctionNameAttribute>(
            nameof(HealthCheckFunction.RunAsync));

        // Assert
        attribute.Name.ShouldBe("HealthCheckFunction");
    }
}
