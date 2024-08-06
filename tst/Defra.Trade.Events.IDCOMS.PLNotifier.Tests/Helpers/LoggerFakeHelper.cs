// Copyright DEFRA (c). All rights reserved.
// Licensed under the Open Government Licence v3.0.

using System.Linq.Expressions;
using System.Reflection;
using FakeItEasy.Configuration;
using FakeItEasy;
using Microsoft.Extensions.Logging;

namespace Defra.Trade.Events.IDCOMS.PLNotifier.Tests.Helpers;

internal static class LoggerFakeHelper
{
    private static readonly PropertyInfo aStateThat;
    private static readonly MethodInfo aStateThatMatches;
    private static readonly MethodInfo loggerLogMethod;
    private static readonly Delegate messageFormatter;
    private static readonly MethodInfo sequenceEqual;
    private static readonly MemberExpression stateOriginalMessage;
    private static readonly ParameterExpression stateParam;
    private static readonly MemberExpression stateValues;

#pragma warning disable S3963 // "static" fields should be initialized inline
    static LoggerFakeHelper()
#pragma warning restore S3963 // "static" fields should be initialized inline
    {
        Expression<Action<ILogger>> logExpr = l => l.LogInformation(null as string, null!);
        var tLoggerExtensions = ((MethodCallExpression)logExpr.Body).Method.DeclaringType!;
        messageFormatter = (Delegate)tLoggerExtensions.GetField("_messageFormatter", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
        var tFormattedLogValues = messageFormatter.GetType().GetGenericArguments()[0]!;
        loggerLogMethod = typeof(ILogger).GetMethod(nameof(ILogger.Log))!.MakeGenericMethod(tFormattedLogValues);
        stateParam = Expression.Parameter(tFormattedLogValues, "p");
        stateOriginalMessage = Expression.Field(stateParam, tFormattedLogValues.GetField("_originalMessage", BindingFlags.NonPublic | BindingFlags.Instance)!);
        stateValues = Expression.Field(stateParam, tFormattedLogValues.GetField("_values", BindingFlags.NonPublic | BindingFlags.Instance)!);
        sequenceEqual = typeof(Enumerable).GetMethods().Single(m => m.Name == nameof(Enumerable.SequenceEqual) && m.GetParameters().Length == 2).MakeGenericMethod(typeof(object));
        aStateThat = typeof(A<>).MakeGenericType(tFormattedLogValues).GetProperty(nameof(A<object>.That))!;
        aStateThatMatches = typeof(ArgumentConstraintManagerExtensions).GetMethods().Single(m => m.Name == nameof(ArgumentConstraintManagerExtensions.Matches) && m.GetParameters().Length == 2)!.MakeGenericMethod(tFormattedLogValues);
    }

    public static IVoidArgumentValidationConfiguration LoggerCall(ILogger logger, LogLevel logLevel, EventId eventId, Exception? exception, string message, Expression<Func<object?[]>>? messageArgs = null)
    {
        return A.CallTo(Expression.Lambda<Action>(
            Expression.Call(
                Expression.Constant(logger),
                loggerLogMethod,
                    Expression.Constant(logLevel),
                    Expression.Constant(eventId),
                    Expression.Call(
                        aStateThatMatches,
                            Expression.Property(null, aStateThat),
                            Expression.Lambda(
                                Expression.And(
                                    Expression.Equal(stateOriginalMessage, Expression.Constant(message)),
                                    Expression.Call(sequenceEqual, stateValues, messageArgs?.Body ?? Expression.Constant(Array.Empty<object>()))
                                ),
                                stateParam
                            )
                    ),
                    Expression.Constant(exception, typeof(Exception)),
                    Expression.Constant(messageFormatter)
            )
        ));
    }
}
