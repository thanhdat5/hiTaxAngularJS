using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(hiTaxAngularJS.Startup))]
namespace hiTaxAngularJS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
