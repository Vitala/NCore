using NHibernate;
using System;

namespace NCore.NHibernate
{
    public interface ICurrentSessionProvider : IDisposable
    {
        ISession CurrentSession { get; }
        ISession OpenSession();
    }
}
