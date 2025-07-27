using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Shared.Data.Patterns.AuditTimestamp;
public static class UpdatedAtHandler
{
    public static void HandleUpdatedAt(ChangeTracker tracker)
    {
        var entries = tracker.Entries<IUpdatedAt>();
        foreach (var e in entries)
        {
            if (e.State == EntityState.Modified)
            {
                e.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}