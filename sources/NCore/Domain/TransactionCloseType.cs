
namespace NCore.Domain
{
    /// <summary>
    /// Тип завершения транзакции
    /// </summary>
    public enum TransactionCloseType
    {
        /// <summary>
        /// Автоматический - транзакция будет подтверждена 
        /// в случае успешного завершения операции и откачена
        /// в случае возникновения исключения
        /// </summary>
        Auto,

        /// <summary>
        /// Подтверждение
        /// </summary>
        Commit,

        /// <summary>
        /// Откат
        /// </summary>
        Rollback
    }
}
