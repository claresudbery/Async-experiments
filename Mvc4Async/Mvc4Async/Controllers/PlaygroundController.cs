using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Mvc4Async.Models;
using System.Threading.Tasks;
using Mvc4Async.Filters;
using System.Web.UI;
using System.Threading;
using System.Web;
using System.Web.Routing;
using Mvc4Async.Hubs;
using Mvc4Async.Service;

namespace Mvc4Async.Controllers
{
    [UseStopwatch]
    [OutputCache(Location = OutputCacheLocation.None, NoStore = true)]
    public class PlaygroundController : Controller
    {
        static CancellationTokenSource _cancellationToken = new CancellationTokenSource();

        public async Task<ActionResult> NestedAsyncCallsNoWait()
        {
            ViewBag.ExperimentType = "Asynchronous experiments - nested 11-second call which we don't wait for.";
            var asyncExperiments = new AsyncExperiments();
            var task = asyncExperiments.NestedAsyncExperimentsNumberSeven();
            await Task.Delay(1000);
            return View("AsyncExperiments", await task);
            //await Task.Delay(10);
            //return View("AsyncExperiments", 1);
        }

        public async Task<ActionResult> NestedAsyncCallsWithWait()
        {
            ViewBag.ExperimentType = "Asynchronous experiments - nested 11-second call which we wait for.";
            var asyncExperiments = new AsyncExperiments();
            return View("AsyncExperiments", await asyncExperiments.NestedAsyncExperimentsNumberSix());
        }

        public async Task<ActionResult> PlaceATaskInADifferentContext()
        {
            ViewBag.ExperimentType = "Asynchronous experiments - placing a task in a different context.";
            var asyncExperiments = new AsyncExperiments();

            asyncExperiments.PlaceATaskInADifferentContext(101);

            System.Diagnostics.Debug.WriteLine("R1. The task in the different context has been kicked off.");
            System.Diagnostics.Debug.WriteLine("R2. We are NOT awaiting its results.");

            return View("AsyncExperiments", 101);
        }

        public async Task<ActionResult> PlaceATaskInADifferentContextAndWaitForIt()
        {
            ViewBag.ExperimentType = "Asynchronous experiments - awaiting a task in a different context.";
            var asyncExperiments = new AsyncExperiments();

            var task = asyncExperiments.PlaceATaskInADifferentContext(102);

            System.Diagnostics.Debug.WriteLine("R1. The task in the different context has been kicked off.");
            System.Diagnostics.Debug.WriteLine("R2. We ARE awaiting its results.");

            return View("AsyncExperiments", await task);
        }

        public async Task<ActionResult> FlowOfExecutionExample()
        {
            ViewBag.ExperimentType = "Asynchronous experiments - flow of execution example.";
            var asyncExperiments = new AsyncExperiments();

            return View("AsyncExperiments", await asyncExperiments.FlowOfExecutionExample());
        }

        public async Task<string> ConfigureAwaitExample()
        {
            string fileName = "";

            var fileContents = await DownloadFileContentsAsync(fileName).ConfigureAwait(false);

            return fileContents;
        }

        private async Task<string> DownloadFileContentsAsync(string fileName)
        {
            throw new NotImplementedException();
        }

        public async Task<ActionResult> CreateADeadlock()
        {
            ViewBag.ExperimentType = "Asynchronous experiments - deadlock example. Warning! This will hang!!";
            var asyncExperiments = new AsyncExperiments();

            var task = asyncExperiments.FlowOfExecutionExample();
            var result = task.Result;

            System.Diagnostics.Debug.WriteLine("Because of the deadlock, this line of code will never be reached!");

            return View("AsyncExperiments", result);
        }

        public async Task<ActionResult> EachAsyncMethodHasItsOwnContext()
        {
            ViewBag.ExperimentType = "Asynchronous experiments - Each Async Method Has Its Own Context.";
            var asyncExperiments = new AsyncExperiments();

            // At this point the current http context and sychronization contexts are both populated.
            ContextHelper.OutputRequestAndSynchronizationContexts("Before ConfigureAwait(false): ");

            var result = await asyncExperiments.EachAsyncMethodHasItsOwnContext().ConfigureAwait(false);

            // At this point the current http context and sychronization contexts are no longer populated.
            ContextHelper.OutputRequestAndSynchronizationContexts("After ConfigureAwait(false): ");

            // Note that even though we are no longer in the request context, the base controller class has kept 
            // track of the response in its Response property, so we can still access the response in this class.
            Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return View("AsyncExperiments", result);
        }

        public async Task<ActionResult> CallingCodeWhichContainsMultipleAwaits()
        {
            ViewBag.ExperimentType = "Asynchronous experiments - Calling Code Which Contains Multiple Awaits.";
            var asyncExperiments = new AsyncExperiments();

            await asyncExperiments.CallingCodeWhichContainsMultipleAwaits();

            return View("AsyncExperiments", 10);
        }

        public ActionResult CallAsyncCodeInANonAsyncContext()
        {
            ViewBag.ExperimentType = "Asynchronous experiments - Call Async Code In A Non-Async Context.";
            var asyncExperiments = new AsyncExperiments();

            Debug.WriteLine("Because we are not in an async method, even though we do not await the return, we will still block the thread until it is completed.");
            asyncExperiments.ReturnATaskEvenThoughWeHaveUsedTheAwaitKeywordMoreThanOnce();
            Debug.WriteLine("We won't get here until the async function is completed.");

            return View("AsyncExperiments", 10);
        }

        public async Task<ActionResult> ReportProgress()
        {
            ViewBag.ExperimentType = "Asynchronous experiments - Reporting Progress.";
            var asyncExperiments = new AsyncExperiments();

            var progressIndicator = new Progress<ProgressIndicator>(ReportProgress);
            try
            {
                _cancellationToken.Dispose();
                _cancellationToken = new CancellationTokenSource();
                await asyncExperiments.MarkedAsyncWithIntegerTaskReturningRandomValueToDifferentContext(
                    _cancellationToken.Token,
                    progressIndicator);
            }
            catch (Exception e)
            {
                ProgressHub.CancelProcessing(e.Message);
            }

            return View("AsyncExperiments", 10);
        }

        public async Task<ActionResult> Cancel()
        {
            _cancellationToken.Cancel();

            return View("AsyncExperiments", 10);
        }

        private void ReportProgress(ProgressIndicator progressIndicator)
        {
            ProgressHub.NotifyHowManyProcessed(progressIndicator.Count, progressIndicator.Total);
        }
    } // End of PlaygroundController
}