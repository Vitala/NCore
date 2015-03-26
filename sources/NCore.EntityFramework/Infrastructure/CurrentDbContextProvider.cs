using NCore.EntityFramework.Domain;

namespace NCore.EntityFramework.Infrastructure
{
    public class CurrentDbContextProvider : ICurrentDbContextProvider
    {
        private readonly ICurrentUnitOfWorkProvider _currentUnitOfWorkProvider;

        public CurrentDbContextProvider(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
        {
           _currentUnitOfWorkProvider = currentUnitOfWorkProvider;
        }

        public IDbContext CurrentContext
        {
            get
            {
                return _currentUnitOfWorkProvider.Current.GetContext();
            }
        }
    }
}
