using NHibernate;

namespace NCore.NHibernate
{
    public interface INhUnitOfWork : IUnitOfWork
    {
        ISession Session { get; }
    }
}