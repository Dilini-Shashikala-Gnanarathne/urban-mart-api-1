using System;

namespace DBFirstApproach.Services
{
    public static class AuditHelper
    {
        public static T SetAuditFields<T>(T entity, CustomContext customContext) where T : IAuditableEntity
        {
            entity.CreatedBy = customContext.Username;
            entity.CreatedAt = customContext.CurrentDateTime;
            entity.UpdatedBy = customContext.Username;
            entity.UpdatedAt = null; // Will be updated on subsequent modifications
            return entity;
        }

        public static T UpdateAuditFields<T>(T entity, CustomContext customContext) where T : IAuditableEntity
        {
            entity.UpdatedAt = customContext.CurrentDateTime;
            entity.UpdatedBy = customContext.Username;
            return entity;
        }
    }
}
