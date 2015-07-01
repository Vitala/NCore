using System.Linq;

namespace NCore.Domain
{
    public interface IRepository
    {
    }

    /// <summary>
    /// Репозиторий
    /// </summary>
    public interface IRepository<TEntity, TPrimaryKey> : IRepository where TEntity : Entity<TPrimaryKey>
    {
        /// <summary>
        /// Получение всех элементов репозитория
        /// </summary>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// Получение элемента репозитория по ключу
        /// </summary>
        TEntity Get(TPrimaryKey key);

        /// <summary>
        /// Добавление элемента в репозиторий
        /// </summary>
        void Insert(TEntity entity);

        /// <summary>
        /// Обновление элемента в репозитории
        /// </summary>
        void Update(TEntity entity);

        /// <summary>
        /// Удаление элемента из репозитория по ключу
        /// </summary>
        /// <param name="id"></param>
        void Delete(TPrimaryKey id);

        /// <summary>
        /// Удаление элемента из репозитория
        /// </summary>
        /// <param name="entity"></param>
        void Delete(TEntity entity);
    }
}
