using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Castle.DynamicLinqQueryBuilder.Samples.Startup))]
namespace Castle.DynamicLinqQueryBuilder.Samples
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
