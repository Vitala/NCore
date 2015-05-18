using NCore.Domain;
using System;
using System.Data.Entity;

namespace NCore.EntityFramework.Domain
{
    public class EfTransaction : TransactionBase
    {
        private readonly DbContextTransaction _transaction;

        public EfTransaction(DbContextTransaction transaction, TransactionCloseType closeType)
            : base(closeType)
        {
            if (transaction == null)
                throw new ArgumentNullException("transaction");

            _transaction = transaction;
        }

        public override void Commit()
        {
            if (!_isClosed)
                _transaction.Commit();
        }

        public override void Rollback()
        {
            if (!_isClosed)
                _transaction.Rollback();
        }
    }
}
