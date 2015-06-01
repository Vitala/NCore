using Autofac;
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
    public class EmitRawUoWInterfaceImplementor<TInterface> : EmitUoWInterfaceImplementorBase
    {
        private const String AppScopeFieldName = "_appScope";
        
        private static readonly MethodInfo BeginScopeMethodInfo;
        private static readonly MethodInfo DisposeMethodInfo;

        private readonly IAppScope _scope;
        private readonly Type _interfaceType;
        
        private readonly bool _isDisposable;

        private FieldInfo _scopeField;

        static EmitRawUoWInterfaceImplementor()
        {
            BeginScopeMethodInfo = typeof(IAppScope).GetMethod("BeginScope");
            DisposeMethodInfo = typeof(IDisposable).GetMethod("Dispose");
        }

        public EmitRawUoWInterfaceImplementor(IAppScope scope)
        {
            _scope = scope;

            _interfaceType = typeof(TInterface);

            _isDisposable = _interfaceType.IsAssignableTo<IDisposable>();
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
                    var typeBuilder = ModuleBuilder.DefineType(GetImplementerTypeName(_interfaceType), DefaultTypeAttributes);
                    typeBuilder.AddInterfaceImplementation(_interfaceType);

                    if (_isDisposable)
                        typeBuilder.AddInterfaceImplementation(typeof(IDisposable));

                    _scopeField = DefineAppScopeField(typeBuilder);

                    ImplementConstructor(typeBuilder);
                    ImplementInterfaces(_interfaceType, typeBuilder);

                    if (_isDisposable)
                        ImplementDisposable(typeBuilder);

                    TypeCache[_interfaceType] = type = typeBuilder.CreateType();
                }
            }

            return (TInterface) (type != null ? Activator.CreateInstance(type, _scope) : null);
        }

        private FieldInfo DefineAppScopeField(TypeBuilder typeBuilder)
        {
            return typeBuilder.DefineField(AppScopeFieldName, typeof(IAppScope), FieldAttributes.Private);
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
            ilGen.Emit(OpCodes.Ldfld, _scopeField);
            ilGen.Emit(OpCodes.Callvirt, ResolveMethodInfo.MakeGenericMethod(propertyInfo.PropertyType));
            ilGen.Emit(OpCodes.Ret);

            propBuilder.SetGetMethod(getMethodBuilder);
        }

        private void ImplementConstructor(TypeBuilder typeBuilder)
        {
            var ctorBuilder = typeBuilder.DefineConstructor(DefaultConstructorAttributes, CallingConventions.Standard, new[] { typeof(IAppScope) });
            var ctorIlGen = ctorBuilder.GetILGenerator();

            ctorIlGen.Emit(OpCodes.Ldarg_0);
            ctorIlGen.Emit(OpCodes.Ldarg_1);

            if (_isDisposable)
                ctorIlGen.Emit(OpCodes.Callvirt, BeginScopeMethodInfo);

            ctorIlGen.Emit(OpCodes.Stfld, _scopeField);
            ctorIlGen.Emit(OpCodes.Ret);
        }

        private void ImplementDisposable(TypeBuilder typeBuilder)
        {
            var disposeBuilder = typeBuilder.DefineMethod(DisposeMethodInfo.Name, DefaultPropertyGetterAttributes, CallingConventions.HasThis);
            var ilGen = disposeBuilder.GetILGenerator();
            
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Ldfld, _scopeField);
            ilGen.Emit(OpCodes.Callvirt, DisposeMethodInfo);
            ilGen.Emit(OpCodes.Ret);
        }
    }

    /*public interface IUowSample: IDisposable
    {
        Config Config { get; }
    }

    public class UnitOfWorkExample: IUowSample
    {
        private IAppScope _scope;

        public UnitOfWorkExample(IAppScope scope)
        {
            _scope = scope.BeginScope();
        }

        public void Dispose()
        {
            _scope.Dispose();
        }

        public Config Config { get { return _scope.Resolve<Config>(); } }
    }*/
}