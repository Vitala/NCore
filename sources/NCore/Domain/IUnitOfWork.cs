using System;

namespace NCore.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Факбрика репозиториев
        /// </summary>
        IRepositoryFactory RepositoryFactory { get; }

        /// <summary>
        /// Начало транзакции
        /// </summary>
        /// <param name="closeType">Тип автоматического закрытия транзакции</param>
        /// <returns>Транзакция</returns>
        ITransaction BeginTransaction(TransactionCloseType closeType = TransactionCloseType.Auto);

        /// <summary>
        /// Сохранение внесенных изменений
        /// </summary>
        void SaveChanges();
        
        /// <summary>
        /// Состояние объекта
        /// </summary>
        bool IsDisposed { get; }
    }
}
