using NCore.Domain;
using NCore.EntityFramework.Infrastructure;
using System;

namespace NCore.EntityFramework.Domain
{
    internal static class UnitOfWorkExtensions
    {
        public static IDbContext GetContext(this IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException("unitOfWork");
            }

            if (!(unitOfWork is EfUnitOfWork))
            {
                throw new ArgumentException("Аргумент unitOfWork не является экземпляром типа " + typeof(EfUnitOfWork).FullName, "unitOfWork");
            }

            return (unitOfWork as EfUnitOfWork).DbContext;
        }
    }
}
