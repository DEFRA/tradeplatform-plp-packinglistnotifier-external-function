// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Defra.Trade.Common.Function.Health.DynamicsClient;

[ExcludeFromCodeCoverage(Justification = "Namespace to be tested within and moved to nuget")]
public sealed class DynamicsClientConfig
{
    /// <summary>
    /// Dynamics App config.
    /// </summary>
    public const string SectionName = "PLP:Dynamics";

    /// <summary>
    /// The unique identifier of the Azure AD instance.
    /// </summary>
    [Required]
    public string Authority { get; set; } = string.Empty;

    /// <summary>
    /// The unique identifier of the registered application we are connecting to in the Azure AD instance.
    /// </summary>
    [Required]
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// The unique secret of the registered application we are connecting to in the Azure AD instance.
    /// </summary>
    [Required]
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// Dynamics subscription key.
    /// </summary>
    [Required]
    public string DynamicsSubscriptionKey { get; set; } = string.Empty;

    /// <summary>
    /// Get transactional data endpoint.
    /// </summary>
    [Required]
    public string Path { get; } = "/api/data/v9.2/trd_exportapplications";

    /// <summary>
    /// Get query string for health check GET requestms
    /// </summary>
    [Required]
    public string Query { get; } = "?$top=1&$select=trd_exportapplicationid";

    /// <summary>
    /// The issuer/domain of the registered application we are connecting to in the Azure AD instance.
    /// </summary>
    [Required]
    public string Resource { get; set; } = string.Empty;
}
