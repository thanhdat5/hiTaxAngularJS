using System.Web.Http;
using WebActivatorEx;
using hiTaxAngularJS;
using Swashbuckle.Application;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace hiTaxAngularJS
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;

            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", "hiTaxAngularJS");
                    })
                .EnableSwaggerUi();
        }
    }
}
