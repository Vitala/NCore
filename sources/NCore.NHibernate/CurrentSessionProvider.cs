using NHibernate;
using NCore.NHibernate.Domain;

namespace NCore.NHibernate
{
    public class CurrentSessionProvider : ICurrentSessionProvider
    {
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;

        public ISession CurrentSession
        {
            get
            {
                return _currentUnitOfWorkProvider.Current.GetSession();
            }
        }

        public CurrentSessionProvider(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
        {
            _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
        }
    }
}
