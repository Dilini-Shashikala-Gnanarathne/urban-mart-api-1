public abstract class BaseService<T> where T : class, IAuditableEntity
{
    protected readonly CustomContext _customContext;

    public BaseService(CustomContext customContext)
    {
        _customContext = customContext;
    }

    protected T SetAuditFields(T entity)
    {
        entity.CreatedBy = _customContext.Username;
        entity.CreatedAt = _customContext.CurrentDateTime;
        entity.UpdatedBy = _customContext.Username;
        entity.UpdatedAt = null;
        return entity;
    }

    protected T UpdateAuditFields(T entity)
    {
        entity.UpdatedAt = _customContext.CurrentDateTime;
        entity.UpdatedBy = _customContext.Username;
        return entity;
    }
}
