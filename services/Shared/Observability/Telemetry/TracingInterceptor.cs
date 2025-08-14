using System.Collections;
using System.Diagnostics;
using Castle.DynamicProxy;

namespace Shared.Observability.Telemetry;

public class TracingInterceptor : IAsyncInterceptor
{
    private static readonly ActivitySource ActivitySource = new(Activities.ServicesActivity);

    public void InterceptSynchronous(IInvocation invocation)
    {
        var activity = HandleTelemtryAnReturnActivity(invocation);
        try
        {
            invocation.Proceed();
        }
        catch (Exception ex)
        {
            HandleException(activity, ex);
            throw;
        }
        finally
        {
            activity.Dispose();
        }
    }

    public void InterceptAsynchronous(IInvocation invocation)
        => invocation.ReturnValue = InternalInterceptAsync(invocation);

    public void InterceptAsynchronous<TResult>(IInvocation invocation)
        => invocation.ReturnValue = InternalInterceptAsync<TResult>(invocation);

    private static async Task InternalInterceptAsync(IInvocation invocation)
    {
        var activity = HandleTelemtryAnReturnActivity(invocation);
        try
        {
            invocation.Proceed();
            await (Task)invocation.ReturnValue!;
        }
        catch (Exception ex)
        {
            HandleException(activity, ex);
            throw;
        }
        finally
        {
            activity.Dispose();
        }
    }

    private static async Task<TResult> InternalInterceptAsync<TResult>(IInvocation invocation)
    {
        var activity = HandleTelemtryAnReturnActivity(invocation);
        try
        {
            invocation.Proceed();
            var result = await (Task<TResult>)invocation.ReturnValue!;
            return result;
        }
        catch (Exception ex)
        {
            HandleException(activity, ex);
            throw;
        }
        finally
        {
            activity.Dispose();
        }
    }

    private static Activity HandleTelemtryAnReturnActivity(IInvocation invocation)
    {
        var method = invocation.MethodInvocationTarget ?? invocation.Method;
        var activityName = TelemetryManager.GetActivityName(method, invocation.TargetType!);

        var activity = ActivitySource.StartActivity(activityName);
        var parametersNames = TelemetryManager.GetParametersNames(method);

        activity?.SetTag("method.name", invocation.Method.Name);
        activity?.SetTag("method.class", invocation.TargetType!.Name);
        activity?.SetTag("method.parameters", string.Join(", ", parametersNames.Select(p => p)));
        activity?.SetTag("method.arguments.count", parametersNames.Length);

        for (int i = 0; i < parametersNames.Length; i++)
        {
            var paramName = parametersNames[i];
            var argValue = invocation.Arguments[i];

            if (TelemetryManager.IsParamMasked(method, paramName))
            {
                argValue = TelemetryManager.MaskValue;
            }
            // (argValue is not IEnumerable) disallow strings bcs they are IEnumerable
            else if (argValue != null && TelemetryManager.IsClass(method, i, argValue) && argValue is not IEnumerable)
            {
                argValue = SerializeWithMasking(argValue);
            }
            else if (argValue is IEnumerable value && argValue is not string)
            {
                argValue = HandleEnumerable(value);
            }

            activity?.SetTag($"arg.{paramName}", argValue?.ToString() ?? "null");
        }

        return activity!;
    }

    private static void HandleException(Activity activity, Exception ex)
    {
        activity?.AddException(ex);
        activity?.SetStatus(ActivityStatusCode.Error);
    }
    
    private static string SerializeWithMasking(object obj)
    {
        if (obj == null) return "null";

        var props = obj.GetType().GetProperties()
            .Select(p =>
            {
                var name = p.Name;
                var value = p.GetValue(obj);

                if (TelemetryManager.IsPropertyMasked(p))
                {
                    value = TelemetryManager.MaskValue;
                }

                return $"{name}={value}";
            });

        return "{" + string.Join(", ", props) + "}";
    }

    private static string HandleEnumerable(IEnumerable enumerable)
        => $"[{string.Join(", ", enumerable.Cast<object>())}]";

}