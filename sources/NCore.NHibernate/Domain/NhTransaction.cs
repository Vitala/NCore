using NCore.Domain;
using System;

namespace NCore.NHibernate.Domain
{
    using ITransaction = global::NHibernate.ITransaction;

    public class NhTransaction : TransactionBase
    {
        private readonly ITransaction _transaction;

        public NhTransaction(ITransaction transaction, TransactionCloseType closeType)
            : base(closeType)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");

            _transaction = transaction;
        }
        public override void Commit()
        {
            if (_transaction.IsActive)
                _transaction.Commit();
        }

        public override void Rollback()
        {
            if (_transaction.IsActive)
                _transaction.Rollback();
        }

        public override void Dispose()
        {
            base.Dispose();

            _transaction.Dispose();
        }
    }
}
