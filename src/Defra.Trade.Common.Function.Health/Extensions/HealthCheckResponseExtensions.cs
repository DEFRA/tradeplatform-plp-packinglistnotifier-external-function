using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure.Management.ContainerService.Fluent.Models;

namespace Defra.Trade.Common.Function.Health.Extensions;

/// <summary>
/// Extensions to help in dealing with Health Checks.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Namespace to be tested within and moved to nuget")]
public static class HealthCheckResponseExtensions
{
    /// <summary>
    /// Map the <see cref="HealthReport"/> received to the <see cref="HealthCheckResponse"/> to return.
    /// </summary>
    /// <param name="healthReport">The health data.</param>
    /// <returns>The mapped response.</returns>
    public static HealthCheckResponse ToResponse(this HealthReport healthReport)
    {
        return new HealthCheckResponse
        {
            Status = healthReport.Status.ToString(),
            TotalDurationSeconds = healthReport.TotalDuration.TotalSeconds.ToString("F3"),
            Results = healthReport.Entries.Select(e =>
                new HealthCheckResponseEntry
                {
                    Name = e.Key,
                    Description = e.Value.Description!,
                    DurationSeconds = e.Value.Duration.TotalSeconds.ToString("F3"),
                    Status = e.Value.Status.ToString(),
                    Exception = e.Value.Exception?.Message!,
                    Data = e.Value.Data
                })
        };
    }
}
