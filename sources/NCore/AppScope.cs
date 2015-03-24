using System;
using Autofac;

namespace NCore
{
    public class AppScope : IAppScope
    {
        public ILifetimeScope Scope { get; protected set; }

        public AppScope(ILifetimeScope scope)
        {
            if (scope == null) throw new ArgumentNullException("scope");
            Scope = scope;
        }

        protected AppScope()
        {
        }
        
        public TType Resolve<TType>()
        {
            return Scope.Resolve<TType>();
        }

        public object Resolve(Type type)
        {
            if (!Scope.IsRegistered(type)) return null;
            return Scope.Resolve(type);
        }

        public AppScope BeginScope()
        {
            var scope = new AppScope();
            var lscope = Scope.BeginLifetimeScope(b => b.Register(c => scope)
                .As<IAppScope>()
                .AsSelf()
                .InstancePerLifetimeScope());
            scope.Scope = lscope;
            return scope;
        }

        public AppScope BeginScope(Action<ContainerBuilder> confAction)
        {
            var scope = new AppScope();
            var lscope = Scope.BeginLifetimeScope(scope, b =>
            {
                b.Register(c => scope)
                 .As<IAppScope>()
                 .AsSelf()
                 .InstancePerLifetimeScope();
                confAction(b);
            });
            scope.Scope = lscope;
            return scope;
        }

        public AppScope BeginScope(object tag)
        {
            var scope = new AppScope();
            var lscope = Scope.BeginLifetimeScope(tag, b => b.Register(c => scope)
                                                            .As<IAppScope>()
                                                            .AsSelf()
                                                            .InstancePerLifetimeScope());
            scope.Scope = lscope;
            return scope;
        }

        public AppScope BeginScope(object tag, Action<ContainerBuilder> confAction)
        {
            var scope = new AppScope();
            var lscope = Scope.BeginLifetimeScope(tag, b =>
            {
                b.Register(c => scope)
                 .As<IAppScope>()
                 .AsSelf()
                 .InstancePerLifetimeScope();
                confAction(b);
            });
            scope.Scope = lscope;
            return scope;
        }

        public virtual void Update(ContainerBuilder cb)
        {
            cb.Update(Scope.ComponentRegistry);
        }

        public virtual void Dispose()
        {
            if (Scope != null)
            {
                Scope.Dispose();
                Scope = null;
            }
        }
    }
}