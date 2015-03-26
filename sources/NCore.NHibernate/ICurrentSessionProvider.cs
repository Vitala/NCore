using NHibernate;

namespace NCore.NHibernate
{
    public interface ICurrentSessionProvider
    {
        ISession CurrentSession { get; }
    }
}
