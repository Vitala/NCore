using NCore.Domain;
using NHibernate;
using System;

namespace NCore.NHibernate.Domain
{
    internal static class UnitOfWorkExtensions
    {
        public static ISession GetSession(this IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
            {
                throw new ArgumentNullException("unitOfWork");
            }

            if (!(unitOfWork is NhUnitOfWork))
            {
                throw new ArgumentException("Аргумент unitOfWork не является экземпляром типа " + typeof(NhUnitOfWork).FullName, "unitOfWork");
            }

            return (unitOfWork as NhUnitOfWork).Session;
        }
    }
}
