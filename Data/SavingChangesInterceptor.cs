using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using museum.Models;

namespace museum.Data;

public class SavingChangesInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        SetDateTimeValues(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        SetDateTimeValues(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void SetDateTimeValues(DbContext context)
    {
        foreach (var entry in context.ChangeTracker.Entries())
            if (entry.Entity is IBaseEntity entity)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entity.CreatedAt = DateTime.Now;
                        entity.UpdatedAt = DateTime.Now;
                        break;
                    case EntityState.Modified:
                        entity.UpdatedAt = DateTime.Now;
                        break;
                }
            }
    }
}