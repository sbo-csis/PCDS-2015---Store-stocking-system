using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProjectWebsite.Startup))]
namespace ProjectWebsite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
