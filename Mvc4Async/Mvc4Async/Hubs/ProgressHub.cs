using System.Web.UI.WebControls;
using Microsoft.AspNet.SignalR;

namespace Mvc4Async.Hubs
{
    public class ProgressHub : Hub
    {
        public static void NotifyHowManyProcessed(int count, int total)
        {
            var message = "Processed " + count + " out of " + total;
            decimal percentage = ((decimal)count / (decimal)total) * 100;

            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();
            hubContext.Clients.All.sendMessage(string.Format(message), percentage);
        }

        public static void CancelProcessing(string message)
        {
            var hubContext = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();
            hubContext.Clients.All.sendMessage(string.Format("Processing has been cancelled: " + message), 0);
        }
    }
}