using System.Diagnostics;
using Shared.Utils;

namespace Shared.Observability.Telemetry;

public static class ActivityUtils
{
    public static TReturn WithEvent<TReturn>(string start, string end, Func<TReturn> func, Activity? activity = null)
    {
        activity ??= Activity.Current;
        GuardUtils.ThrowIfNull(activity);
        activity.AddEvent(new ActivityEvent(start));
        try
        {
            TReturn result = func();
            return result;
        }
        finally
        {
            activity.AddEvent(new ActivityEvent(end));
        }
    }

    public static void WithEvent(string start, string end, Action action, Activity? activity = null)
    {
        activity ??= Activity.Current;
        GuardUtils.ThrowIfNull(activity);
        activity.AddEvent(new ActivityEvent(start));
        try
        {
            action();
        }
        finally
        {
            activity.AddEvent(new ActivityEvent(end));
        }
    }

    public static async Task<TReturn> WithEventAsync<TReturn>(string start, string end, Func<Task<TReturn>> func, Activity? activity = null)
    {
        activity ??= Activity.Current;
        GuardUtils.ThrowIfNull(activity);
        activity.AddEvent(new ActivityEvent(start));
        try
        {
            return await func();
        }
        finally
        {
            activity.AddEvent(new ActivityEvent(end));
        }
    }
    
    public static async Task WithEventAsync(string start, string end, Func<Task> func, Activity? activity = null)
    {
        activity ??= Activity.Current;
        GuardUtils.ThrowIfNull(activity);
        activity.AddEvent(new ActivityEvent(start));
        try
        {
            await func();
        }
        finally
        {
            activity.AddEvent(new ActivityEvent(end));
        }
    }
}