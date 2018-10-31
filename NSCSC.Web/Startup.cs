using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NSCSC.Web.Startup))]
namespace NSCSC.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
