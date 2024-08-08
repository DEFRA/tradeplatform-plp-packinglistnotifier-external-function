// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Tests.Helpers;

public static class FunctionTestHelpers
{
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

    private static MethodInfo GetMethodInfo<TClass>(string methodName)
    {
        return typeof(TClass).GetMethod(methodName)!;
    }
}
