using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WeixinShare.Startup))]
namespace WeixinShare
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
