using System;

namespace NCore.Domain
{
    /// <summary>
    /// Транзакция
    /// </summary>
    public interface ITransaction : IDisposable
    {
        /// <summary>
        /// Фиксация изменений
        /// </summary>
        void Commit();

        /// <summary>
        /// Откат изменений
        /// </summary>
        void Rollback();
    }
}
