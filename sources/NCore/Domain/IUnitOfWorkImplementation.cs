using NCore.Kernel;

namespace NCore.Domain
{
    public interface IUnitOfWorkImplementation : IUnitOfWork
    {
        IAppScope Scope { get; }
    }
}
