using Autofac;
using NCore.Domain;
using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace NCore.Emitter
{
    /// <summary>
    /// Класс помошник регистрации интерфесов для EmmitUoWInterfaceImplementer
    /// </summary>
    public static class EmitUoWRegistrationHelper
    {
        private static readonly ConcurrentDictionary<Type, Func<object, object>> Transformers = new ConcurrentDictionary<Type, Func<object, object>>();
        
        /// <summary>
        /// Зарегистрировать несколько интерфейсов для фабрики интерфейсов
        /// </summary>
        /// <param name="builder">Построитьель контейнера</param>
        /// <param name="types">Интерфейсы для регистрации</param>
        public static void RegisterInterfacesForEmit(this ContainerBuilder builder, params Type[] types)
        {
            for (var i = 0; i < types.Length; ++i)
                builder.RegisterInterfaceForEmit(types[i]);
        }        

        /// <summary>
        /// Зарегистрировать интерфейс для фабрики интерфейсов
        /// </summary>
        /// <param name="builder">Построитель контейнера</param>
        /// <param name="type">Интерфейс</param>
        public static void RegisterInterfaceForEmit(this ContainerBuilder builder, Type type)
        {
            var factoryType = type.IsAssignableTo<IUnitOfWork>() 
                ? typeof(EmitUoWInterfaceImplementor<>).MakeGenericType(type)
                : typeof(EmitRawUoWInterfaceImplementor<>).MakeGenericType(type);

            var function = Transformers.GetOrAdd(type, (Func<Type, Func<object, object>>) (t =>
            {
                var parameter = Expression.Parameter(typeof (object), "interfaceType");
                var methodInfo = factoryType.GetMethod("ImplementInterface");
                var expression = Expression.Lambda<Func<object, object>>(Expression.Call(Expression.Convert(parameter, factoryType), methodInfo), new[] { parameter });
                
                return expression.Compile();
            }));
           
            builder.Register(c => function(c.Resolve(factoryType))).As(type);
        }

        /// <summary>
        /// Зарегистрировать интерфейс для фабрики интерфейсов
        /// </summary>
        /// <typeparam name="T">Интерфейс</typeparam>
        /// <param name="builder">Построитель контейнера</param>
        public static void RegisterInterfaceForEmit<T>(this ContainerBuilder builder)
        {
            builder.RegisterInterfaceForEmit(typeof(T));
        }     
    }
}