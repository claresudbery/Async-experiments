using Microsoft.Owin;
using Owin;
using Mvc4Async;

namespace Mvc4Async
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}