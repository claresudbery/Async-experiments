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

        public async Task<ActionResult> Flow_Of_Execution_Example()
        {
            ViewBag.ExampleType = "Asynchronous examples - flow of execution example.";
            var asyncExamples = new AsyncExamples();

            return View("AsyncExamples", await asyncExamples.FlowOfExecutionExample());
        }

        public async Task<ActionResult> Marked_Async_With_Empty_Task_And_NOT_Calling_Await()
        {
            ViewBag.ExampleType = "Asynchronous examples - an async method that returns a simple task but doesn't use the await keyword.";
            var asyncExamples = new AsyncExamples();
            await asyncExamples.Marked_Async_With_Empty_Task_And_NOT_Calling_Await();
            return View("AsyncExamples", 98);
        }

        public async Task<ActionResult> Marked_Async_With_Empty_Task_And_Calling_Await()
        {
            ViewBag.ExampleType = "Asynchronous examples - an async method that returns a simple task and DOES use the await keyword.";
            var asyncExamples = new AsyncExamples();
            await asyncExamples.Marked_Async_With_Empty_Task_And_Calling_Await();
            return View("AsyncExamples", 99);
        }

        public async Task<ActionResult> CreateADeadlock()
        {
            ViewBag.ExampleType = "Asynchronous examples - Create a deadlock. This action will never complete!";
            var asyncExamples = new AsyncExamples();

            // It doesn't actually matter what method we call. 
            // As long as it contains an await statement, we will get a deadlock.
            var task = asyncExamples.DeadlockExample();

            // Task.Result hides a Wait statement, which will lock the request context.
            // This means that the code in DeadlockExample() can never complete.
            var result = task.Result;

            System.Diagnostics.Debug.WriteLine("Because of the deadlock, this line of code will never be reached!");

            return View("AsyncExamples", result);
        }

        public async Task<ActionResult> Leave_The_Request_Context_In_Some_Places_But_Not_All()
        {
            ViewBag.ExampleType = "Asynchronous examples - Use ConfigureAwait(false) to leave the request context in some places but not all";
            var asyncExamples = new AsyncExamples();

            // At this point the current http context and sychronization contexts are both populated.
            ContextHelper.OutputRequestAndSynchronizationContexts("Before ConfigureAwait(false): ");

            // In Leave_The_Request_Context_In_Some_Places_But_Not_All, we will stay in the request context
            var result = await asyncExamples.Leave_The_Request_Context_In_Some_Places_But_Not_All().ConfigureAwait(false);

            // We have now left the main request context (because we use ConfigureAwait(false) in the line above).
            // At this point the current http context and sychronization contexts are no longer populated.
            ContextHelper.OutputRequestAndSynchronizationContexts("After ConfigureAwait(false): ");

            return View("AsyncExamples", result);
        }

        public async Task<ActionResult> Leave_The_Request_Context_In_All_Places()
        {
            ViewBag.ExampleType = "Asynchronous examples - Use ConfigureAwait(false) to leave the request context in all places";
            var asyncExamples = new AsyncExamples();

            // At this point the current http context and sychronization contexts are both populated.
            ContextHelper.OutputRequestAndSynchronizationContexts("Before ConfigureAwait(false): ");

            // In Leave_The_Request_Context_In_All_Places, we will also leave the request context
            var result = await asyncExamples.Leave_The_Request_Context_In_All_Places().ConfigureAwait(false);

            // We have now left the main request context (because we use ConfigureAwait(false) in the line above).
            // At this point the current http context and sychronization contexts are no longer populated.
            ContextHelper.OutputRequestAndSynchronizationContexts("After ConfigureAwait(false): ");

            // Note that even though we are no longer in the request context, the base controller class has kept 
            // track of the response in its Response property, so we can still access the response in this class.
            Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return View("AsyncExamples", result);
        }

        public async Task<ActionResult> Fire_and_forget()
        {
            ViewBag.ExampleType = "Asynchronous examples - set off an 11-second call in a different context.";
            var asyncExamples = new AsyncExamples();
            
            // We will leave the request context AND we won't await the result.
            // This means the code will keep running even after we return a response to the browser.
            asyncExamples.Leave_The_Request_Context_In_All_Places();

            return View("AsyncExamples", 10);
        }

        public async Task<ActionResult> ReportProgress()
        {
            ViewBag.ExampleType = "Asynchronous examples - Reporting Progress.";
            var asyncExamples = new AsyncExamples();

            var progressIndicator = new Progress<ProgressIndicator>(ReportProgress);
            await asyncExamples.AsyncMethodWithProgress(progressIndicator);

            return View("AsyncExamples", 10);
        }

        public async Task<ActionResult> ThisActionCanBeCancelled()
        {
            ViewBag.ExampleType = "Asynchronous examples - This Action Can Be Cancelled.";
            var asyncExamples = new AsyncExamples();

            var progressIndicator = new Progress<ProgressIndicator>(ReportProgress);
            try
            {
                _cancellationToken.Dispose();
                _cancellationToken = new CancellationTokenSource();
                await asyncExamples.AsyncMethodWithCancellation(
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
    } // End of ExamplesController
}

