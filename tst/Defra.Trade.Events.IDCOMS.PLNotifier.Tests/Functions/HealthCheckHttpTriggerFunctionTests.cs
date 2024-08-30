// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Text;
using Defra.Trade.Common.Function.Health;
using Defra.Trade.Events.IDCOMS.PLNotifier.Functions;
using Defra.Trade.Events.IDCOMS.PLNotifier.Tests.Helpers;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
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
        var attribute = FunctionTestHelpers.MethodHasSingleAttribute<HealthCheckHttpTriggerFunction, FunctionNameAttribute>(
            nameof(HealthCheckHttpTriggerFunction.RunAsync));

        // Assert
        attribute.Name.ShouldBe("HealthCheckHttpTriggerFunction");
    }

    [Fact]
    public void RunAsync_HasHttpTriggerAttributeWithCorrectValues()
    {
        // Arrange & Act & Assert
        FunctionTestHelpers.Function_HasHttpTriggerAttributeWithCorrectValues<HealthCheckHttpTriggerFunction>(
            nameof(HealthCheckHttpTriggerFunction.RunAsync),
            "health",
            ["GET"],
            AuthorizationLevel.Anonymous);
    }

    [Fact]
    public async Task RunAsync_ValidHealthCheck_ReturnsOkResponse()
    {
        // Arrange
        var body = new MemoryStream(Encoding.UTF8.GetBytes(string.Empty));
        var req = new FakeHttpRequest(A.Fake<FunctionContext>(), new Uri("https://test/api/message"), body);
        var healthReport = new HealthReport(new Dictionary<string, HealthReportEntry>(), HealthStatus.Healthy, TimeSpan.FromSeconds(1));

        A.CallTo(() => _healthCheckService.CheckHealthAsync(null, CancellationToken.None))
            .ReturnsLazily(() => healthReport);

        // Act
        var result = await _sut.RunAsync(req);

        // Assert
        result.ShouldNotBeNull();
        var bodyText = result as JsonResult;
        bodyText.ShouldNotBeNull();
        bodyText.Value.ShouldBe("Healthy");
    }

    [Fact]
    public async Task RunAsync_InvalidHealthCheck_ReturnsInternalServerErrorResponse()
    {
        // Arrange
        var body = new MemoryStream(Encoding.UTF8.GetBytes(string.Empty));
        var req = new FakeHttpRequest(A.Fake<FunctionContext>(), new Uri("https://test/api/message"), body);
        var healthReport = new HealthReport(new Dictionary<string, HealthReportEntry>(), HealthStatus.Unhealthy, TimeSpan.FromSeconds(1));

        A.CallTo(() => _healthCheckService.CheckHealthAsync(null, CancellationToken.None))
            .ReturnsLazily(() => healthReport);

        // Act
        var result = await _sut.RunAsync(req);

        // Assert
        result.ShouldNotBeNull();
        var bodyText = result as JsonResult;
        bodyText.ShouldNotBeNull();
        bodyText.Value.ShouldNotBeNull();
        var errors = bodyText.Value as HealthCheckResponse;
        errors.ShouldNotBeNull();
        errors.Status.ShouldBe("Unhealthy");
    }
}