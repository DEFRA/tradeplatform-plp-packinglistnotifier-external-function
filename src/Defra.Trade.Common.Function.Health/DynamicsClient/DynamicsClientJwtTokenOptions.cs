// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Defra.Trade.Common.Function.Health.DynamicsClient;

[ExcludeFromCodeCoverage(Justification = "Namespace to be tested within and moved to nuget")]
public sealed class DynamicsClientJwtTokenOptions
{
    /// <summary>
    /// Bearer token.
    /// </summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    /// <summary>
    /// Token type.
    /// </summary>
    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;
}
