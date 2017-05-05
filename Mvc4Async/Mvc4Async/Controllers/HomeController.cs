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
    public class HomeController : Controller
    {
        static CancellationTokenSource _CancellationToken = new CancellationTokenSource();

        public async Task<ActionResult> PWGasync()
        {
            ViewBag.SyncType = "Asynchronous";
            var widgetService = new WidgetService();
            var prodService = new ProductService();
            var gizmoService = new GizmoService();

            var widgetTask = widgetService.GetWidgetsAsync();
            var prodTask = prodService.GetProductsAsync();
            var gizmoTask = gizmoService.GetGizmosAsync();

            await Task.WhenAll(widgetTask, prodTask, gizmoTask);

            var pwgVM = new ProdGizWidgetVM(
               widgetTask.Result,
               prodTask.Result,
               gizmoTask.Result
               );

            ViewBag.numGizmos = pwgVM.gizmoList.Count();
            ViewBag.numWidgets = pwgVM.widgetList.Count();
            ViewBag.numProducts = pwgVM.prodList.Count();

            return View("PWG", pwgVM);
        }

        [AsyncTimeout(50)]
        [HandleError(ExceptionType = typeof(TimeoutException), View = "TimeoutError")]
        public async Task<ActionResult> PWGtimeOut(CancellationToken cancellationToken)
        {
            ViewBag.SyncType = "Asynchronous with CancellationToken";

            var widgetService = new WidgetService();
            var prodService = new ProductService();
            var gizmoService = new GizmoService();

            var widgetTask = widgetService.GetWidgetsAsync(cancellationToken);
            var prodTask = prodService.GetProductsAsync(cancellationToken);
            var gizmoTask = gizmoService.GetGizmosAsync(cancellationToken);

            await Task.WhenAll(widgetTask, prodTask, gizmoTask);

            var pwgVM = new ProdGizWidgetVM(
               widgetTask.Result,
               prodTask.Result,
               gizmoTask.Result
               );

            ViewBag.numGizmos = pwgVM.gizmoList.Count();
            ViewBag.numWidgets = pwgVM.widgetList.Count();
            ViewBag.numProducts = pwgVM.prodList.Count();

            return View("PWG", pwgVM);
        }

        public ActionResult PWG()
        {
            ViewBag.SyncType = "Synchronous";
            var widgetService = new WidgetService();
            var prodService = new ProductService();
            var gizmoService = new GizmoService();

            var pwgVM = new ProdGizWidgetVM(
                widgetService.GetWidgets(),
                prodService.GetProducts(),
                gizmoService.GetGizmos()
               );

            ViewBag.numGizmos = pwgVM.gizmoList.Count();
            ViewBag.numWidgets = pwgVM.widgetList.Count();
            ViewBag.numProducts = pwgVM.prodList.Count();

            return View("PWG", pwgVM);
        }

        public async Task<ActionResult> WidgetsAsync()
        {
            System.Diagnostics.Debug.WriteLine("Entering WidgetsAsync. Context: " + SynchronizationContext.Current.GetHashCode());
            ViewBag.SyncOrAsync = "Asynchronous";
            var widgetService = new WidgetService();
            var widgets = await widgetService.GetWidgetsAsync();
            ViewBag.numWidgets = widgets.Count();
            System.Diagnostics.Debug.WriteLine("Leaving WidgetsAsync");

            return View("Widgets", widgets);
        }

        public ActionResult Widgets()
        {
            ViewBag.SyncOrAsync = "Synchronous";
            var widgetService = new WidgetService();
            var widgets = widgetService.GetWidgets();
            ViewBag.numWidgets = widgets.Count();

            return View("Widgets", widgets);
        }

        [AsyncTimeout(150)]
        [HandleError(ExceptionType = typeof(TimeoutException),
                                            View = "TimeoutError")]
        public async Task<ActionResult> GizmosCancelAsync(
                               CancellationToken cancellationToken)
        {
            ViewBag.SyncOrAsync = "Asynchronous";
            var gizmoService = new GizmoService();
            var gizmos = await gizmoService.GetGizmosAsync(cancellationToken);
            ViewBag.numGizmos = gizmos.Count();

            return View("Gizmos", gizmos);
        }

        public async Task<ActionResult> GizmosAsync()
        {
            System.Diagnostics.Debug.WriteLine("Entering GizmosAsync. Context: " +
                                               SynchronizationContext.Current.GetHashCode());
            ViewBag.SyncOrAsync = "Asynchronous";
            var gizmoService = new GizmoService();
            var gizmos = await gizmoService.GetGizmosAsync();
            ViewBag.numGizmos = gizmos.Count();
            System.Diagnostics.Debug.WriteLine("Leaving GizmosAsync");

            return View("Gizmos", gizmos);
        }

        public ActionResult Gizmos()
        {
            ViewBag.SyncOrAsync = "Synchronous";
            var gizmoService = new GizmoService();
            var gizmos = gizmoService.GetGizmos();
            ViewBag.numGizmos = gizmos.Count();

            return View("Gizmos", gizmos);
        }

        public async Task<ActionResult> ProductsAsync()
        {
            System.Diagnostics.Debug.WriteLine("Entering ProductsAsync. Context: " + SynchronizationContext.Current.GetHashCode());
            ViewBag.SyncOrAsync = "Asynchronous";
            var productsService = new ProductService();
            var products = await productsService.GetProductsAsync();
            ViewBag.numProducts = products.Count();
            System.Diagnostics.Debug.WriteLine("Leaving ProductsAsync");

            return View("Products", products);
        }

        public ActionResult Products()
        {
            ViewBag.SyncOrAsync = "Synchronous";
            var prodService = new ProductService();
            var products = prodService.GetProducts();
            ViewBag.numProducts = products.Count();

            return View("Products", products);
        }

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

        public async Task<ActionResult> CreateADeadlock()
        {
            ViewBag.ExperimentType = "Asynchronous experiments - deadlock example. Warning! This will hang!!";
            var asyncExperiments = new AsyncExperiments();

            var task = asyncExperiments.FlowOfExecutionExample();
            var result = task.Result;

            System.Diagnostics.Debug.WriteLine("Because of the deadlock, this line of code should never be reached!");

            return View("AsyncExperiments", result);
        }

        public async Task<ActionResult> EachAsyncMethodHasItsOwnContext()
        {
            ViewBag.ExperimentType = "Asynchronous experiments - Each Async Method Has Its Own Context.";
            var asyncExperiments = new AsyncExperiments();

            return View("AsyncExperiments", await asyncExperiments.EachAsyncMethodHasItsOwnContext());
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
                _CancellationToken.Dispose();
                _CancellationToken = new CancellationTokenSource();
                await asyncExperiments.MarkedAsyncWithIntegerTaskReturningRandomValueToDifferentContext(
                    _CancellationToken.Token,
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
            _CancellationToken.Cancel();

            return View("AsyncExperiments", 10);
        }

        private void ReportProgress(ProgressIndicator progressIndicator)
        {
            ProgressHub.NotifyHowManyProcessed(progressIndicator.Count, progressIndicator.Total);
        }
    } // End of HomeController
}

