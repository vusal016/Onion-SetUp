namespace OnionSetUp.Persistence.Interceptors
{
    public class Interceptor:SaveChangesInterceptor
    {
        public override async ValueTask <InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            if (eventData.Context is null) return await base.SavingChangesAsync(eventData, result,cancellationToken);
            var entitiesDel = eventData.Context.ChangeTracker.Entries();
            foreach (var entry in entitiesDel)
            {
                if (entry.Entity is ISoftDeletable && entry.State == EntityState.Deleted)
                {
                entry.State = EntityState.Modified;
                var entryDeleted = (ISoftDeletable)entry.Entity;
                entryDeleted.IsDeleted = true;
                entryDeleted.DeletedAt = DateTime.UtcNow;
                }
                if (entry.Entity is AuditEntity audit)
                {
                    if (entry.State == EntityState.Added)
                        audit.CreatedAt = DateTime.UtcNow;
                    if (entry.State == EntityState.Modified)
                        audit.ModifiedAt = DateTime.UtcNow;
                }
            }
            return await base.SavingChangesAsync(eventData, result,cancellationToken);
        }
    }
}