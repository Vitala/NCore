using NHibernate;
using NHibernate.Context;
using System;

namespace NCore.NHibernate
{
    public class CurrentSessionProvider : ICurrentSessionProvider
    {
        private ISessionFactory _sessionFactory;

        public CurrentSessionProvider(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        public ISession OpenSession()
        {
            var session = _sessionFactory.OpenSession();
            CurrentSessionContext.Bind(session);
            return session;
        }

        public void Dispose()
        {
            CurrentSessionContext.Unbind(_sessionFactory);
        }

        public ISession CurrentSession
        {
            get
            {
                if (!CurrentSessionContext.HasBind(_sessionFactory))
                {
                    return null;
                }
                var contextualSession = _sessionFactory.GetCurrentSession();
                return contextualSession;
            }
        }
    }
}
