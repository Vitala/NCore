using System;
using Autofac;

namespace NCore
{
    public interface IAppScope : IDisposable
    {
        TType Resolve<TType>();
        object Resolve(Type type);
        AppScope BeginScope();
        AppScope BeginScope(Action<ContainerBuilder> confAction);
        AppScope BeginScope(object tag);
        AppScope BeginScope(object tag, Action<ContainerBuilder> confAction);
        void Update(ContainerBuilder cb);
    }

}