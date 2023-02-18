using BusTour.Common.Services;
using BusTour.Data.Handlers;
using Dapper;
using Infrastructure.Common.DI;
using Infrastructure.Common.Plugins;
using Infrastructure.Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;

namespace BusTour.Test
{
    public abstract class UnitTestBase
    {
        protected static IServiceProvider _services;
        protected IMediator _mediator;

        [TestInitialize]
        public void BeforeTest()
        {
            if (_services == null)
            {
                _services = SimpleStartup.Build(null, ReplaceService);
            }

            _mediator = IoC.GetRequiredService<IMediator>();

            BeforeTestStarted();
        }

        [TestCleanup]
        public void AfterTest()
        {
            AfterTestFinished();
            //_services.Value = null;
        }

        protected virtual void BeforeTestStarted()
        {
        }

        protected virtual void AfterTestFinished()
        {
        }

        protected virtual void ReplaceService(IServiceCollection serviceCollection)
        {
            serviceCollection.Replace(ServiceDescriptor.Singleton<IUserContext, TestUserContext>());
            //SqlMapper.AddTypeHandler(new DictionaryHandler());
        }

        protected void SetContext(int id, string role)
        {
            var context = IoC.GetRequiredService<IUserContext>() as TestUserContext;
            context.UserId = id;
            context.Role = role;
        }
    }
}
