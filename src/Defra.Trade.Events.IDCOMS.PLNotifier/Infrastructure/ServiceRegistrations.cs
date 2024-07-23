// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

using System;
using System.Diagnostics.CodeAnalysis;
using Defra.Trade.Common.Config;
using Defra.Trade.Common.Functions;
using Defra.Trade.Common.Functions.EventStore;
using Defra.Trade.Common.Functions.Interfaces;
using Defra.Trade.Common.Functions.Models;
using Defra.Trade.Common.Functions.Validation;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Infrastructure;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Mappers;
using Models = Defra.Trade.Events.IDCOMS.PLNotifier.Application.Models;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Services;
using Defra.Trade.Events.IDCOMS.PLNotifier.Application.Validators;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Inbound = Defra.Trade.Events.IDCOMS.PLNotifier.Application.Dtos.Inbound;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Infrastructure;

[ExcludeFromCodeCoverage]
public static class ServiceRegistrations
{
    public static IServiceCollection AddServiceRegistrations(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddValidators()
            .AddEventStore()
            .AddProcessor()
            .AddConfigurations(configuration);
    }

    private static IServiceCollection AddConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ServiceBusQueuesSettings>().Bind(configuration.GetSection(ServiceBusSettings.OptionsName));
        var configName = configuration.GetSection(Models.PlNotifierSettings.PlNotifierSettingsName);
        services.AddOptions<Models.PlNotifierSettings>().Bind(configName);
        services.Configure<ServiceBusSettings>(configuration.GetSection(ServiceBusSettings.OptionsName));

        // TODO
        // DevOps needs to assign appconfig for PLP
        services.AddCrm(configuration.GetSection("SuSRemosSubscriber:Dynamics"));

        return services;
    }

    private static IServiceCollection AddEventStore(this IServiceCollection services)
    {
        return services
            .AddEventStoreConfiguration()
            .AddSingleton<IMessageCollector, EventStoreCollector>();
    }

    private static IServiceCollection AddProcessor(this IServiceCollection services)
    {
        return services
            .AddTransient<IMessageProcessor<Models.Approval, TradeEventMessageHeader>, ApprovalMessageProcessor>()
            .AddTransient<IBaseMessageProcessorService<Inbound.Approval>,
                BaseMessageProcessorService<Inbound.Approval, Models.Approval, Models.Approval, TradeEventMessageHeader>>();
    }

    private static IServiceCollection AddValidators(this IServiceCollection services)
    {
        return services
             .AddSingleton<ICustomValidatorFactory, CustomValidatorFactory>()
             .AddSingleton<AbstractValidator<TradeEventMessageHeader>, MessageHeaderValidator>()
             .AddSingleton<AbstractValidator<Inbound.Approval>, ApprovalValidator>()
             .AddTransient<IInboundMessageValidator<Inbound.Approval, TradeEventMessageHeader>,
                InboundMessageValidator<Inbound.Approval, Models.Approval, TradeEventMessageHeader>>();
    }
}
