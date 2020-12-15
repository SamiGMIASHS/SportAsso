using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SportAsso.Startup))]
namespace SportAsso
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
