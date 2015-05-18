using Autofac.Features.OwnedInstances;
using NCore.Domain;
using NCore.Security.Model;
using NCore.Security.NHibernate.Interfaces;
using NCore.Security.NHibernate.Model;
using NCore.TestApp.Entities;
using NHibernate;
using NHibernate.Hql.Ast.ANTLR;
using NHibernate.Impl;
using NHibernate.Linq;
using NHibernate.Loader.Criteria;
using NHibernate.Persister.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NCore.TestApp.Services
{
    public class TestService : ITestService
    {
        private readonly Func<IAuthorizationService> _authService;
        private readonly Func<IUnitOfWork> _uowFactory;
        private readonly Func<IRepository<User, int>> _userRepoFactory;
        private readonly Func<IAuthorizationRepository> _authRepo;
        private readonly Func<IPermissionsBuilderService> _permBuilderFactory;
        private readonly Func<ITestRepository> _testEntityRepoFactory;

        public TestService(Func<IUnitOfWork> uowFactory,
            Func<IRepository<User, int>> userRepoFactory,
            Func<IAuthorizationService> authService,
            Func<IAuthorizationRepository> authRepo,
            Func<IPermissionsBuilderService> permBuilderFactory,
            Func<ITestRepository> testEntityRepoFactory)
        {
            _uowFactory = uowFactory;
            _userRepoFactory = userRepoFactory;
            _authService = authService;
            _authRepo = authRepo;
            _permBuilderFactory = permBuilderFactory;
            _testEntityRepoFactory = testEntityRepoFactory;
        }

        public void AddTestRecord()
        {
            var user = new User() { Name = "vasya" };
            var user2 = new User() { Name = "petya" };
            var ent = new TestEntity() { Name = "test entity", SecurityKey = Guid.NewGuid() };
            var ent2 = new TestEntity() { Name = "test entity2", SecurityKey = Guid.NewGuid() };
            var ent3 = new TestEntity() { Name = "test entity3", SecurityKey = Guid.NewGuid() };

            using (var uow = _uowFactory())
            {
                uow.BeginTransaction();
                var userRepo = _userRepoFactory();
                var testRepo = _testEntityRepoFactory();
                var authRepo = _authRepo();

                var eg = authRepo.CreateEntitiesGroup("test entities");
                using (uow.BeginTransaction(TransactionCloseType.Auto))
                {
                    testRepo.Insert(ent); testRepo.Insert(ent2); testRepo.Insert(ent3);
                    userRepo.Insert(user); userRepo.Insert(user2);

                    authRepo.CreateOperation("/TestRootOperation/TestOperation");

                    authRepo.AssociateEntityWith<TestEntity>(ent, eg);
                    authRepo.AssociateEntityWith<TestEntity>(ent2, eg);
                    authRepo.CreateChildEntityGroupOf("test entities", "test ent child");

                    uow.SaveChanges();
                }

                _permBuilderFactory().Deny("/TestRootOperation/TestOperation")
                    .For(user)
                    .On(eg)
                    .DefaultLevel()
                    .Save();

                _permBuilderFactory().Allow("/TestRootOperation/TestOperation")
               .For(user2)
               .On(eg)
               .DefaultLevel()
               .Save();



                var testEntities = _testEntityRepoFactory().GetAll();

                testEntities = _authService().AddPermissionsToQuery<TestEntity>(user2, "/TestRootOperation/TestOperation", testEntities);

                var res = testEntities.ToList();
            }
        }
        
        public String ToSql(System.Linq.IQueryable queryable)
        {
            var sessionProperty = typeof(DefaultQueryProvider).GetProperty("Session", BindingFlags.NonPublic | BindingFlags.Instance);
            var session = sessionProperty.GetValue(queryable.Provider, null) as ISession;
            var sessionImpl = session.GetSessionImplementation();
            var factory = sessionImpl.Factory;
            var nhLinqExpression = new NhLinqExpression(queryable.Expression, factory);
            var translatorFactory = new ASTQueryTranslatorFactory();
            var translator = translatorFactory.CreateQueryTranslators(nhLinqExpression, null, false, sessionImpl.EnabledFilters, factory).First();
            //in case you want the parameters as well
            //var parameters = nhLinqExpression.ParameterValuesByName.ToDictionary(x => x.Key, x => x.Value.Item1);

            return translator.SQLString;
        }

        public String GetGeneratedSql(ICriteria criteria)
        {
            var criteriaImpl = (CriteriaImpl)criteria;
            var sessionImpl = (SessionImpl)criteriaImpl.Session;
            var factory = (SessionFactoryImpl)sessionImpl.SessionFactory;
            var implementors = factory.GetImplementors(criteriaImpl.EntityOrClassName);
            var loader = new CriteriaLoader((IOuterJoinLoadable)factory.GetEntityPersister(implementors[0]), factory, criteriaImpl, implementors[0], sessionImpl.EnabledFilters);

            return loader.SqlString.ToString();
        }
        
    }
}
