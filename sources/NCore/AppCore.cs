using Autofac;

namespace NCore
{
    public class AppCore: AppScope
    {
        private readonly ILifetimeScope _rootScope;
        private IContainer _container;
        public static AppCore Instance { get; private set; }

        public AppCore(ILifetimeScope container)
            : base()
        {
            _rootScope = container;
            _container = container as IContainer;

            Scope =
                _rootScope.BeginLifetimeScope(
                    b =>
                    {
                        b.Register(c => this).SingleInstance().AsSelf();
                        b.Register(c => this.BeginScope()).InstancePerDependency().As<IAppScope>().AsSelf();
                    });
            
            Instance = this;
        }

        public void Update(ContainerBuilder cb)
        {
            /*
            if (_container == null)
                throw new ZenCoreException("Ядро Zen.Core не получило IContaner при построении. Функционал не доступен.");
             */
            cb.Update(_container);
        }

        public override void Dispose()
        {
            base.Dispose();
            _rootScope.Dispose();
        }
    }
}