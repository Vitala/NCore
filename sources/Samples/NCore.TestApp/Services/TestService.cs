﻿using Autofac.Features.OwnedInstances;
using NCore.Domain;
using NCore.NHibernate.Security.Interfaces;
using NCore.NHibernate.Security.Model;
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
        private readonly Func<IAuthorizationRepository> _authRepoFactory;
        private readonly Func<IPermissionsBuilderService> _permBuilderFactory;
        private readonly Func<ITestRepository> _testEntityRepoFactory;
        private readonly Func<IRepository<Permission, int>> _permRepoFactory;

        public TestService(Func<IUnitOfWork> uowFactory,
            Func<IRepository<User, int>> userRepoFactory,
            Func<IAuthorizationService> authService, 
            Func<IAuthorizationRepository> authRepoFactory,
            Func<IPermissionsBuilderService> permBuilderFactory,
            Func<ITestRepository> testEntityRepoFactory,
            Func<IRepository<Permission, int>> permRepoFactory)
        {
            _uowFactory = uowFactory;
            _userRepoFactory = userRepoFactory;
            _authService = authService;
            _authRepoFactory = authRepoFactory;
            _permBuilderFactory = permBuilderFactory;
            _testEntityRepoFactory = testEntityRepoFactory;
            _permRepoFactory = permRepoFactory;
        }

        public void AddTestRecord()
        {
            var user = new User() { Name = "vasya" };
            var ent = new TestEntity() { Name = "test entity", SecurityKey = Guid.NewGuid() };

            using (var uow = _uowFactory())
            {
                uow.BeginTransaction();
                var userRepo = _userRepoFactory();
                var testRepo = _testEntityRepoFactory();
                testRepo.Insert(ent);
                userRepo.Insert(user);
                uow.Commit();
            

            _authRepoFactory().CreateOperation("/TestRootOperation/TestOperation");

            _permBuilderFactory().Allow("/TestRootOperation/TestOperation")
                .For(user)
                .On<TestEntity>(ent)
                .DefaultLevel()
                .Save();

            var testEntities = _testEntityRepoFactory().GetAll();

                var perms = _permRepoFactory().GetAll();

                var queryable = testEntities.Where(x =>
                    perms
                    .Where(y => y.Operation.Name == "/TestRootOperation/TestOperation" && (y.User == user) && y.Allow)
                    .Select(y => y.EntitySecurityKey)
                    .Contains(x.SecurityKey));

                var res = ToSql(queryable);
                /*
                perms.Where(x=>x.Operation.Name == "/TestRootOperation/TestOperation" && (x.User == user)
                    && x.EntitySecurityKey == ent.SecurityKey
                    );

                

            var criteria = _testEntityRepoFactory().GetEntities();
            _authService().AddPermissionsToQuery(user, "/TestRootOperation/TestOperation", criteria);
            var str = criteria.List<TestEntity>(); */
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