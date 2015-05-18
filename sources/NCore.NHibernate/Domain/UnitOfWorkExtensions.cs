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
                throw new ArgumentNullException("unitOfWork");

            var nhUnitOfWork = unitOfWork as NhUnitOfWork;
            if (nhUnitOfWork == null)
                throw new ArgumentException("Аргумент unitOfWork не является экземпляром типа " + typeof(NhUnitOfWork).FullName, "unitOfWork");

            return nhUnitOfWork.Session;
        }
    }
}
