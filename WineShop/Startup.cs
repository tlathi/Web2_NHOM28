using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WineShop.Startup))]
namespace WineShop
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
