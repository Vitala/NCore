using NCore.NHibernate.Security.Model;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using System;

namespace NCore.NHibernate.Security.Services
{
    internal class SecurityCriterions
    {
        public static DetachedCriteria DirectUsersGroups(User user)
        {
            return DetachedCriteria.For<UsersGroup>()
                .CreateAlias("Users", "user")
                .Add(Expression.Eq("user.id", user.SecurityInfo.Identifier));
        }

        public static DetachedCriteria DirectEntitiesGroups<TEntity>(TEntity entity) where TEntity : class
        {
            Guid key = SecurityCore.Instance.ExtractKey(entity);
            return DetachedCriteria.For<EntitiesGroup>()
                .CreateAlias("Entities", "e")
                .Add(Expression.Eq("e.EntitySecurityKey", key));
        }

        public static DetachedCriteria AllGroups(User user)
        {
            DetachedCriteria directGroupsCriteria = DirectUsersGroups(user)
                .SetProjection(Projections.Id());

            DetachedCriteria criteria = DetachedCriteria.For<UsersGroup>()
                                                        .CreateAlias("Users", "user", JoinType.LeftOuterJoin)
                                                        .CreateAlias("AllChildren", "child", JoinType.LeftOuterJoin)
                                                        .Add(
                                                             Subqueries.PropertyIn("child.id", directGroupsCriteria) ||
                                                             Expression.Eq("user.id", user.SecurityInfo.Identifier))
                                        .SetProjection(Projections.Id());

            return DetachedCriteria.For<UsersGroup>()
                                    .Add(Subqueries.PropertyIn("Id", criteria));
        }

        public static DetachedCriteria AllGroups<TEntity>(TEntity entity) where TEntity : class
        {
            var key = SecurityCore.Instance.ExtractKey(entity);
            var directGroupsCriteria = DirectEntitiesGroups(entity)
                .SetProjection(Projections.Id());

            var criteria = DetachedCriteria.For<EntitiesGroup>()
                                                        .CreateAlias("Entities", "entity", JoinType.LeftOuterJoin)
                                                        .CreateAlias("AllChildren", "child", JoinType.LeftOuterJoin)
                                                        .Add(
                                                            Subqueries.PropertyIn("child.id", directGroupsCriteria) ||
                                                            Expression.Eq("entity.EntitySecurityKey", key))
                                        .SetProjection(Projections.Id());

            return DetachedCriteria.For<EntitiesGroup>()
                                    .Add(Subqueries.PropertyIn("Id", criteria));
        }

    }
}
