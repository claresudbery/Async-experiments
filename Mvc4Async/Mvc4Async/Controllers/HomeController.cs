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
    } // End of HomeController
}

