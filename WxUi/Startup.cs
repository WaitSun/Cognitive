using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WxUi.Startup))]
namespace WxUi
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
