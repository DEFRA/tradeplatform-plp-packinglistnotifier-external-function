// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Defra.Trade.Common.Function.Health.DynamicsClient;

/// <inheritdoc />
[ExcludeFromCodeCoverage(Justification = "Namespace to be tested within and moved to nuget")]
public sealed class DynamicsClientAuthenticator : IDynamicsClientAuthenticator
{
    private readonly DynamicsClientConfig _dynamicsClientOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="DynamicsClientAuthenticator"/> class.
    /// </summary>
    /// <param name="dynamicsApiConfig"></param>
    public DynamicsClientAuthenticator(IOptions<DynamicsClientConfig> dynamicsApiConfig)
    {
        ArgumentNullException.ThrowIfNull(dynamicsApiConfig);
        _dynamicsClientOptions = dynamicsApiConfig.Value;
    }

    /// <inheritdoc />
    public async Task<DynamicsClientJwtTokenOptions> GenerateAuthenticationTokenAsync()
    {
        using var client = new HttpClient();

        var baseAddress = $"{_dynamicsClientOptions.Authority}/oauth2/token";
        const string grantType = "client_credentials";
        var form = new Dictionary<string, string>
        {
            { "grant_type", grantType },
            { "client_id", _dynamicsClientOptions.ClientId },
            { "client_secret", _dynamicsClientOptions.ClientSecret },
            { "resource", _dynamicsClientOptions.Resource }
        };

        using var uriContext = new FormUrlEncodedContent(form);
        var tokenResponse = await client.PostAsync(baseAddress, uriContext);
        var jsonContent = await tokenResponse.Content.ReadAsStringAsync();

        tokenResponse.EnsureSuccessStatusCode();

        var jwtToken = JsonSerializer.Deserialize<DynamicsClientJwtTokenOptions>(jsonContent);

        return jwtToken ?? throw new InvalidOperationException("Unable to get token from App reg");
    }
}
