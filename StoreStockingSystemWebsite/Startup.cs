using Microsoft.Owin;
using Owin;
using StoreStockingSystem.Models;

[assembly: OwinStartupAttribute(typeof(compare.Startup))]
namespace compare
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //TODO: Move this to context constructor
            //StoreStockingSystem.Services.Test.Seed(new StoreStockingContext());
            ConfigureAuth(app);
        }
    }
}
