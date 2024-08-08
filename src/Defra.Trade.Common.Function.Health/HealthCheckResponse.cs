using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Defra.Trade.Common.Function.Health;

/// <summary>
/// Information about the health of a WebAPI.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Namespace to be tested within and moved to nuget")]
public class HealthCheckResponse
{
    /// <summary>
    /// The overall status of the health check.
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Amount of time that the response took to execute.
    /// </summary>
    public string TotalDurationSeconds { get; set; } = string.Empty;

    /// <summary>
    /// Health check details.
    /// </summary>
    public IEnumerable<HealthCheckResponseEntry> Results { get; set; } = new List<HealthCheckResponseEntry>();
}
