using NCore.Domain;
using NCore.Kernel;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace NCore.Emitter
{
    /// <summary>
    /// Фабрика классов реализующих интерфейс через разрешение зависимостей
    /// </summary>
    /// <typeparam name="TInterface">Тип интерфейса</typeparam>
    public class EmitUoWInterfaceImplementor<TInterface> : EmitUoWInterfaceImplementorBase where TInterface : IUnitOfWork
    {
        private readonly IAppScope _scope;
        private readonly ICurrentUnitOfWorkProvider _provider;
        private readonly Type _interfaceType;

        private readonly Type _parentType;
        private readonly PropertyInfo _scopeProperty;

        public EmitUoWInterfaceImplementor(IAppScope scope, ICurrentUnitOfWorkProvider provider)
        {
            _scope = scope;
            _provider = provider;

            _interfaceType = typeof(TInterface);

            using (var innerScope = _scope.BeginScope())
            using (var parentUow = innerScope.Resolve<IUnitOfWorkImplementation>())
            {
                if (parentUow == null)
                    throw new NCoreException("Ошибка определения родительской реализации UoW");

                _parentType = parentUow.GetType();
            }

            _scopeProperty = _parentType.GetProperty("Scope");
        }

        /// <summary>
        /// Получить реализацию интерфейса
        /// </summary>
        /// <param name="scope">Область видимости для IoC</param>
        /// <returns>Экземпляр класса реализующий интерфейс</returns>
        public TInterface ImplementInterface()
        {
            Type type;
            lock (TypeCache)
            {
                if (!TypeCache.TryGetValue(_interfaceType, out type))
                {
                    var typeBuilder = ModuleBuilder.DefineType(GetImplementerTypeName(_interfaceType), DefaultTypeAttributes, _parentType);
                    typeBuilder.AddInterfaceImplementation(_interfaceType);

                    ImplementConstructor(typeBuilder);
                    ImplementInterfaces(_interfaceType, typeBuilder);

                    TypeCache[_interfaceType] = type = typeBuilder.CreateType();
                }
            }

            return (TInterface) (type != null ? Activator.CreateInstance(type, _scope, _provider) : null);
        }

        private void ImplementInterfaces(Type interfaceType, TypeBuilder typeBuilder)
        {
            foreach (var propertyInfo in interfaceType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance))
                ImplementProperty(typeBuilder, propertyInfo);

            foreach (var ancestorInterface in _interfaceType.GetInterfaces().Where(i => i != typeof(IUnitOfWork) && i != typeof(IDisposable)))
                ImplementInterfaces(ancestorInterface, typeBuilder);
        }

        private void ImplementProperty(TypeBuilder typeBuilder, PropertyInfo propertyInfo)
        {
            var propBuilder = typeBuilder.DefineProperty(propertyInfo.Name, PropertyAttributes.None, propertyInfo.PropertyType, new Type[0]);
            var getMethodBuilder = typeBuilder.DefineMethod(GetPropertyGetterName(propertyInfo), DefaultPropertyGetterAttributes, CallingConventions.HasThis, propertyInfo.PropertyType, null);
            
            var ilGen = getMethodBuilder.GetILGenerator();

            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Callvirt, _scopeProperty.GetMethod);
            ilGen.Emit(OpCodes.Callvirt, ResolveMethodInfo.MakeGenericMethod(propertyInfo.PropertyType));
            ilGen.Emit(OpCodes.Ret);

            propBuilder.SetGetMethod(getMethodBuilder);
        }

        private void ImplementConstructor(TypeBuilder typeBuilder)
        {
            var baseConstructor = _parentType.GetConstructor(new[] { typeof(IAppScope), typeof(ICurrentUnitOfWorkProvider) });
            var ctorBuilder = typeBuilder.DefineConstructor(DefaultConstructorAttributes, CallingConventions.Standard, new[] { typeof(IAppScope), typeof(ICurrentUnitOfWorkProvider) });
            var ctorIlGen = ctorBuilder.GetILGenerator();

            ctorIlGen.Emit(OpCodes.Ldarg_0);
            ctorIlGen.Emit(OpCodes.Ldarg_1);
            ctorIlGen.Emit(OpCodes.Ldarg_2);
            ctorIlGen.Emit(OpCodes.Call, baseConstructor);
            ctorIlGen.Emit(OpCodes.Ret);
        }
    }

    /*public interface IUowSample : IUnitOfWork
    {
        Config Config { get; }
    }

    public class UnitOfWorkExample : UnitOfWorkImplementation, IUowSample
    {
        public UnitOfWorkExample(IAppScope scope, ICurrentUnitOfWorkProvider provider)
            : base(scope, provider)
        {
        }

        public Config Config { get { return Scope.Resolve<Config>(); } }
    }*/
}