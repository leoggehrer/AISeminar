//@CodeCopy
#if EXTERNALGUID_ON

using PulseCheck.Logic.Entities;

namespace PulseCheck.Logic.DataContext
{
    partial class ProjectDbContext
    {
        static partial void BeforeSaveChanges(ChangedEntry changedEntry)
        {
            if (changedEntry.State == EntityState.Added)
            {
                if (changedEntry.Entry.Entity is EntityObject eo && eo.Guid == Guid.Empty)
                {
                    eo.Guid = Guid.NewGuid();
                }
            }
        }
    }
}
#endif
