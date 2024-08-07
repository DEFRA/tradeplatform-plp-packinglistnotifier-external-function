// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

using System.Text;
using Defra.Trade.Events.IDCOMS.PLNotifier.Functions;
using FakeItEasy;
using Microsoft.Azure.Functions.Worker;
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
    public async Task HttpGet_WithHealthyStatus_ReturnsHealthy()
    {
        // arrange
        var body = new MemoryStream(Encoding.UTF8.GetBytes(string.Empty));
        var request = new FakeHttpRequest(A.Fake<FunctionContext>(), new Uri("https://test/api/message"), body);
        var healthReport = new HealthReport(new Dictionary<string, HealthReportEntry>(), HealthStatus.Healthy, TimeSpan.FromSeconds(1));

        A.CallTo(() => _healthCheckService.CheckHealthAsync(default))
            .ReturnsLazily(() => healthReport);

        // act
        var result = await _sut.RunAsync(request);

        // assert
        result.ShouldNotBeNull();
    }
}
