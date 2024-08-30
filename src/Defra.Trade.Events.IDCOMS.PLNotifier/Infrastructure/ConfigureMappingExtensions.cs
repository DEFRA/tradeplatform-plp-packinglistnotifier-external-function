// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government License v3.0.

using System.Diagnostics.CodeAnalysis;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Mappers;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Infrastructure;

[ExcludeFromCodeCoverage(Justification = "No value in covering startup")]
public static class ConfigureMappingExtensions
{
    public static void ConfigureMapper(this IFunctionsHostBuilder hostBuilder)
    {
        var assembly = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.FullName is string fullName && fullName.Contains("Defra"))
            .OrderBy(a => a.FullName)
            .ToList();
        hostBuilder.Services.AddAutoMapper(assembly);
        hostBuilder.Services.AddAutoMapper(typeof(ApprovalProfile).Assembly);
    }
}
