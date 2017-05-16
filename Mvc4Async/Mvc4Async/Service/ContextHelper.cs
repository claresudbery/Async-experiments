using System.Diagnostics;
using System.Threading;

namespace Mvc4Async.Service
{
    public static class ContextHelper
    {
        public static void OutputRequestAndSynchronizationContexts(string introMessage)
        {
            string currentHttpContext = System.Web.HttpContext.Current != null
                ? System.Web.HttpContext.Current.GetHashCode().ToString()
                : "null";

            string currentSynchronizationContext = SynchronizationContext.Current != null
                ? SynchronizationContext.Current.GetHashCode().ToString()
                : "null";

            Debug.WriteLine(introMessage);

            Debug.WriteLine(
                $"Current request context: {currentHttpContext}. Current synchronization context: {currentSynchronizationContext}");
        }

        public static void OutputRequestContext()
        {
            string currentHttpContext = System.Web.HttpContext.Current != null
                ? "available"
                : "null";

            Debug.WriteLine(currentHttpContext);
        }
    }
}