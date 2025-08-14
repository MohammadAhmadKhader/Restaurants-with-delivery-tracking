using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Diagnostics;

namespace Shared.Observability.Telemetry;

public class TelemetryDbInterceptor : DbCommandInterceptor
{
    private static readonly ActivitySource DbActivitySource = new(Activities.DatabaseActivity);
    private static readonly ConcurrentDictionary<Guid, Activity> ActiveCommands = new();

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result,
        CancellationToken cancellationToken = default)
    {
        StartActivity(command, eventData);
        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result,
        CancellationToken cancellationToken = default)
    {
        CompleteActivity(eventData, exception: null);
        return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }

    // NonQuery Operations (INSERT/UPDATE/DELETE)
    public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        StartActivity(command, eventData);
        return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override ValueTask<int> NonQueryExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        CompleteActivity(eventData, exception: null);
        return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
    }

    // Scalar operations such as SELECT COUNT(*)
    public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<object> result,
        CancellationToken cancellationToken = default)
    {
        StartActivity(command, eventData);
        return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override ValueTask<object?> ScalarExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        object? result,
        CancellationToken cancellationToken = default)
    {
        CompleteActivity(eventData, exception: null);
        return base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override void CommandFailed(DbCommand command, CommandErrorEventData eventData)
    {
        CompleteActivity(eventData, eventData.Exception);
        base.CommandFailed(command, eventData);
    }

    public override Task CommandFailedAsync(DbCommand command, CommandErrorEventData eventData, CancellationToken cancellationToken = default)
    {
        CompleteActivity(eventData, eventData.Exception);
        return base.CommandFailedAsync(command, eventData, cancellationToken);
    }


    private static void StartActivity(DbCommand command, CommandEventData eventData)
    {
        var activity = DbActivitySource.StartActivity("SQL Query", ActivityKind.Client);
        
        if (activity != null)
        {
            ActiveCommands.TryAdd(eventData.CommandId, activity);
            
            activity.SetTag("db.statement", command.CommandText);
            activity.SetTag("db.system", TelemetryManager.GetDbSystem(command.Connection?.GetType()));
            activity.SetTag("db.operation", GetDbOperation(command.CommandText));
            activity.SetTag("db.command_id", eventData.CommandId.ToString());
        }
    }


    private static void CompleteActivity(CommandEventData eventData, Exception? exception)
    {
        if (ActiveCommands.TryRemove(eventData.CommandId, out var activity))
        {
            try
            {
                if (eventData is CommandExecutedEventData executedEventData)
                {
                    activity.SetTag("db.duration_ms", executedEventData.Duration.TotalMilliseconds);
                }

                if (exception != null)
                {
                    activity.AddException(exception);
                    activity.SetStatus(ActivityStatusCode.Error, exception.Message);
                }
                else
                {
                    activity.SetStatus(ActivityStatusCode.Ok);
                }
            }
            finally
            {
                activity.Dispose();
            }
        }
    }

    private static string GetDbOperation(string? commandText)
    {
        if (string.IsNullOrEmpty(commandText))
            return "UNKNOWN";

        var trimmed = commandText.Trim().ToUpperInvariant();

        if (trimmed.StartsWith("SELECT")) return "SELECT";
        if (trimmed.StartsWith("INSERT")) return "INSERT";
        if (trimmed.StartsWith("UPDATE")) return "UPDATE";
        if (trimmed.StartsWith("DELETE")) return "DELETE";
        if (trimmed.StartsWith("CREATE")) return "CREATE";
        if (trimmed.StartsWith("DROP")) return "DROP";
        if (trimmed.StartsWith("ALTER")) return "ALTER";

        return "UNKNOWN";
    }
}