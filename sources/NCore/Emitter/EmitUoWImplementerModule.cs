using Autofac;

namespace NCore.Emitter
{
    /// <summary>
    /// Модуль обеспечивающий работу EmitInterfaceImplementor
    /// </summary>
    public class EmitUoWImplementerModule:Module
    {
        protected override void Load(ContainerBuilder b)
        {
            b.RegisterGeneric(typeof(EmitUoWInterfaceImplementor<>))
                .AsSelf()
                .InstancePerLifetimeScope();
            
            b.RegisterGeneric(typeof(EmitRawUoWInterfaceImplementor<>))
                .AsSelf()
                .InstancePerLifetimeScope();
        }        
    }
}