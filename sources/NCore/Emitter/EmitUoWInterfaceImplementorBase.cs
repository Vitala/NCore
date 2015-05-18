using NCore.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace NCore.Emitter
{
    /// <summary>
    /// Базовай класс фабрики реализаций
    /// </summary>
    public class EmitUoWInterfaceImplementorBase
    {
        private const String AssemblymName = "NCore.UowEmitCache";
        private const String ModuleName = "EmittedImplementations";

        protected const TypeAttributes DefaultTypeAttributes = TypeAttributes.Class | TypeAttributes.Public 
            | TypeAttributes.AutoLayout | TypeAttributes.AnsiClass | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit;

        protected const MethodAttributes DefaultConstructorAttributes = MethodAttributes.Public | MethodAttributes.HideBySig
            | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;

        protected const MethodAttributes DefaultPropertyGetterAttributes = MethodAttributes.Public | MethodAttributes.HideBySig 
            | MethodAttributes.NewSlot | MethodAttributes.Virtual | MethodAttributes.Final;

        protected static readonly MethodInfo ResolveMethodInfo;
        protected static readonly AssemblyBuilder AssemblyBuilder;
        protected static readonly ModuleBuilder ModuleBuilder;
        protected static readonly Dictionary<Type, Type> TypeCache = new Dictionary<Type, Type>();
        
        static EmitUoWInterfaceImplementorBase()
        {
            ResolveMethodInfo = typeof(AppScope).GetMethods()
                .FirstOrDefault(m => m.Name == "Resolve" && m.IsGenericMethod);

            AssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(AssemblymName), AssemblyBuilderAccess.Run);
            ModuleBuilder = AssemblyBuilder.DefineDynamicModule(ModuleName, AssemblymName);
        }

        protected static String GetImplementerTypeName(Type uowInterface)
        {
            var name = String.Format("{0}Impl", uowInterface.Name);
            return name[0] == 'I'
                ? name.Substring(1)
                : name; 
        }

        protected static String GetPropertyGetterName(PropertyInfo propertyInfo)
        {
            return String.Format("get_{0}", propertyInfo.Name);
        }
    }
}