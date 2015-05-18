using System;
using System.Runtime.InteropServices;

namespace NCore.Domain
{
    public abstract class TransactionBase : ITransaction
    {
        protected TransactionCloseType _closeType;

        protected bool _isClosed = false;

        protected TransactionBase(TransactionCloseType closeType)
        {
            _closeType = closeType;
        }

        public abstract void Commit();

        public abstract void Rollback();

        public virtual void Dispose()
        {
            if (_isClosed)
                return;

            switch (_closeType)
            {
                case TransactionCloseType.Commit:
                    Commit();
                    break;
                case TransactionCloseType.Rollback:
                    Rollback();
                    break;
                case TransactionCloseType.Auto:
                    if (Marshal.GetExceptionPointers() != IntPtr.Zero || Marshal.GetExceptionCode() != 0)
                        Rollback();
                    else
                        Commit();
                    break;
                default:
                    Rollback();
                    break;
            }
        }
    }
}
