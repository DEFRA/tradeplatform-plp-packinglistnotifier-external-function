// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Tests.Helpers;

public static class FunctionTestHelpers
{
    public static async Task<string> StreamToStringAsync(this Stream stream)
    {
        stream.Position = 0;
        using var reader = new StreamReader(stream);
        var text = await reader.ReadToEndAsync();
        return text;
    }

    public static TAttribute MethodHasSingleAttribute<TClass, TAttribute>(string methodName)
        where TClass : class
        where TAttribute : Attribute
    {
        var attributes = GetMethodAttributes<TClass, TAttribute>(methodName);

        attributes.ShouldNotBeNull();
        attributes.ShouldHaveSingleItem();

        var attribute = attributes![0] as TAttribute;
        attribute.ShouldNotBeNull();

        return attribute!;
    }

    public static object[]? GetMethodAttributes<TClass, TAttribute>(string methodName)
        where TClass : class
        where TAttribute : Attribute
    {
        var methodInfo = typeof(TClass)
            .GetMethod(methodName);

        var attribute = methodInfo!
            .GetCustomAttributes(typeof(TAttribute), false);

        return attribute;
    }

    public static void Function_HasHttpTriggerAttributeWithCorrectValues<TClass>(
        string methodName,
        string expectedRoute,
        string[] expectedMethods,
        AuthorizationLevel expectedAuthLevel = AuthorizationLevel.Function)
        where TClass : class
    {
        var methodInfo = GetMethodInfo<TClass>(methodName)
            ?? throw new InvalidOperationException($"Could not find method {methodName} on class {typeof(TClass).Name}");

        var httpRequestDataParam = methodInfo.GetParameters()
            .Single(x => x.ParameterType == typeof(HttpRequest));

        var httpTriggerAttribute = httpRequestDataParam
            .GetCustomAttributes(typeof(HttpTriggerAttribute), false)
            .Select(x => x as HttpTriggerAttribute)
            .Single()!;

        httpTriggerAttribute.AuthLevel.ShouldBe(expectedAuthLevel);
        httpTriggerAttribute.Route.ShouldBe(expectedRoute);
        httpTriggerAttribute.Methods.ShouldNotBeNull();
        httpTriggerAttribute.Methods!.Length.ShouldBe(expectedMethods.Length);

        var commonEl = expectedMethods.Intersect(httpTriggerAttribute.Methods, StringComparer.InvariantCultureIgnoreCase);
        commonEl.Count().ShouldBe(expectedMethods.Length);
    }

    private static MethodInfo GetMethodInfo<TClass>(string methodName)
    {
        return typeof(TClass).GetMethod(methodName)!;
    }
}
