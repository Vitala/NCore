using NCore.Domain;
using NCore.Kernel;
using System;
using System.Collections.Concurrent;
using System.Runtime.Remoting.Messaging;

namespace NCore
{
    public class CallContextCurrentUnitOfWorkProvider : ICurrentUnitOfWorkProvider
    {
        private const string ContextKey = "NCore.UnitOfWork.Current";

        private static readonly ConcurrentDictionary<string, IUnitOfWork> UnitOfWorkDictionary
            = new ConcurrentDictionary<string, IUnitOfWork>();

        internal static IUnitOfWork StaticUow
        {
            get
            {
                var unitOfWorkKey = CallContext.LogicalGetData(ContextKey) as string;
                if (unitOfWorkKey == null)
                {
                    return null;
                }

                IUnitOfWork unitOfWork;
                if (!UnitOfWorkDictionary.TryGetValue(unitOfWorkKey, out unitOfWork))
                {
                    CallContext.LogicalSetData(ContextKey, null);
                    return null;
                }

                if (unitOfWork.IsDisposed)
                {
                    CallContext.LogicalSetData(ContextKey, null);
                    UnitOfWorkDictionary.TryRemove(unitOfWorkKey, out unitOfWork);
                    return null;
                }

                return unitOfWork;
            }

            set
            {
                var unitOfWorkKey = CallContext.LogicalGetData(ContextKey) as string;
                if (unitOfWorkKey != null)
                {
                    IUnitOfWork unitOfWork;
                    if (UnitOfWorkDictionary.TryGetValue(unitOfWorkKey, out unitOfWork))
                    {
                        if (unitOfWork == value)
                        {
                            return;
                        }

                        UnitOfWorkDictionary.TryRemove(unitOfWorkKey, out unitOfWork);
                    }

                    CallContext.LogicalSetData(ContextKey, null);
                }

                if (value == null)
                {
                    return;
                }

                unitOfWorkKey = Guid.NewGuid().ToString();
                if (!UnitOfWorkDictionary.TryAdd(unitOfWorkKey, value))
                {
                    throw new NCoreException("Невозможно установить UnitOfWork");
                }

                CallContext.LogicalSetData(ContextKey, unitOfWorkKey);
            }
        }

        public IUnitOfWork Current
        {
            get { return StaticUow; }
            set { StaticUow = value; }
        }
    }
}