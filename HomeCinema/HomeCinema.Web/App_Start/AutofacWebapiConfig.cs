using Autofac;
using Autofac.Integration.WebApi;
using HomeCinema.Data.Configurations;
using HomeCinema.Data.Infrastructure;
using HomeCinema.Data.Repositories;
using HomeCinema.Services;
using HomeCinema.Services.Abstract;
using System.Data.Entity;
using System.Reflection;
using System.Web.Http;

namespace HomeCinema.Web.App_Start
{
    public class AutofacWebapiConfig
    {

        public static IContainer Container;

        public static void Initialize(HttpConfiguration config)
        {
            Intialize(config, RegisterServices(new ContainerBuilder()));
        }

        private static IContainer RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // EF Home cinema context
            builder.RegisterType<HomeCinemaContext>().As<DbContext>().InstancePerRequest();
            builder.RegisterType<DbFactory>().As<IDbFactory>().InstancePerRequest();
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerRequest();
            builder.RegisterGeneric(typeof(EntityBaseRepository<>)).As(typeof(IEntityBaseRepository<>)).InstancePerRequest();

            // Services 
            builder.RegisterType<EncryptionService>().As<IEncryptionService>().InstancePerRequest();
            builder.RegisterType<MembershipService>().As<IMembershipService>().InstancePerRequest();
            Container = builder.Build();

            return Container;
        }

        private static void Intialize(HttpConfiguration config, IContainer container)
        {
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}