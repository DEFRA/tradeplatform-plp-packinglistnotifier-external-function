// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

namespace Defra.Trade.Common.Function.Health.DynamicsClient;

/// <summary>
/// Http client authenticator.
/// </summary>
public interface IDynamicsClientAuthenticator
{
    Task<DynamicsClientJwtTokenOptions> GenerateAuthenticationTokenAsync();
}
