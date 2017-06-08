using System.Web.Http;

namespace HomeCinema.Web.App_Start
{
    public class Bootstrapper
    {
        public static void Run()
        {
            // configure autofac
            AutofacWebapiConfig.Initialize(GlobalConfiguration.Configuration);

            // configure automapper
            //AutoMapperConfiguration.Configure();
        }
    }
}