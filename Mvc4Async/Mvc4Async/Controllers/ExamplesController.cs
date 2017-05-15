using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Mvc4Async.Models;
using System.Threading.Tasks;
using Mvc4Async.Filters;
using System.Web.UI;
using System.Threading;
using Mvc4Async.Hubs;
using Mvc4Async.Service;

namespace Mvc4Async.Controllers
{
    [UseStopwatch]
    [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    public class ExamplesController : Controller
    {
        static CancellationTokenSource _cancellationToken = new CancellationTokenSource();

        public async Task<ActionResult> TemplateEndpoint()
        {
            ViewBag.ExampleType = "Asynchronous examples - template.";
            var asyncExamples = new AsyncExamples();
            var task = asyncExamples.TrivialExample();
            return View("AsyncExamples", await task);
        }

        public async Task<ActionResult> Dont_await_long_running_code()
        {
            ViewBag.ExampleType = "Asynchronous examples - nested 11-second call which we don't wait for.";
            var asyncExamples = new AsyncExamples();
            var task = asyncExamples.Dont_await_long_running_code();
            return View("AsyncExamples", await task);
        }

        public ActionResult Call_Async_Code_In_NonAsync_Context()
        {
            ViewBag.ExampleType = "Asynchronous examples - Call Async Code In A Non-Async Context.";
            var asyncExamples = new AsyncExamples();

            Debug.WriteLine("Because we are not in an async method, even though we do not await the return, we will still block the thread until it is completed.");
            asyncExamples.MultipleAwaitStatements();
            Debug.WriteLine("We won't get here until all four tasks in MultipleAwaitStatements have completed.");

            return View("AsyncExamples", 10);
        }

        public async Task<ActionResult> FlowOfExecutionExample()
        {
            ViewBag.ExampleType = "Asynchronous examples - flow of execution example.";
            var asyncExamples = new AsyncExamples();

            return View("AsyncExamples", await asyncExamples.FlowOfExecutionExample());
        }

        public async Task<ActionResult> ReportProgress()
        {
            ViewBag.ExampleType = "Asynchronous examples - Reporting Progress.";
            var asyncExamples = new AsyncExamples();

            var progressIndicator = new Progress<ProgressIndicator>(ReportProgress);
            try
            {
                _cancellationToken.Dispose();
                _cancellationToken = new CancellationTokenSource();
                await asyncExamples.MarkedAsyncWithIntegerTaskReturningRandomValueToDifferentContext(
                    _cancellationToken.Token,
                    progressIndicator);
            }
            catch (Exception e)
            {
                ProgressHub.CancelProcessing(e.Message);
            }

            return View("AsyncExamples", 10);
        }

        public async Task<ActionResult> Cancel()
        {
            ViewBag.ExampleType = "Async operation was cancelled.";
            _cancellationToken.Cancel();

            return View("AsyncExamples", 10);
        }

        private void ReportProgress(ProgressIndicator progressIndicator)
        {
            ProgressHub.NotifyHowManyProcessed(progressIndicator.Count, progressIndicator.Total);
        }
    } // End of PlaygroundController
}

