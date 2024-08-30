// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.Common.Function.Health;

/// <summary>
/// Specific health check details.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Namespace to be tested within and moved to nuget")]
public class HealthCheckResponseEntry
{
    /// <summary>
    /// The name of the service.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the service.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Time that the health check took to execute.
    /// </summary>
    public string DurationSeconds { get; set; } = string.Empty;

    /// <summary>
    /// Status of the service.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Exception details if raised.
    /// </summary>
    public string Exception { get; set; } = string.Empty;

    /// <summary>
    /// Further untyped data.
    /// </summary>
    public IReadOnlyDictionary<string, object>? Data { get; set; }
}
