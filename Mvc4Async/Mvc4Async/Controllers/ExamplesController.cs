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
        static CancellationTokenSource _CancellationToken = new CancellationTokenSource();

        public async Task<ActionResult> ReportProgress()
        {
            ViewBag.ExerciseType = "Asynchronous examples - Reporting Progress.";
            var AsyncExamples = new AsyncExamples();

            var progressIndicator = new Progress<ProgressIndicator>(ReportProgress);
            try
            {
                _CancellationToken.Dispose();
                _CancellationToken = new CancellationTokenSource();
                await AsyncExamples.MarkedAsyncWithIntegerTaskReturningRandomValueToDifferentContext(
                    _CancellationToken.Token,
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
            ViewBag.ExerciseType = "Async operation was cancelled.";
            _CancellationToken.Cancel();

            return View("AsyncExamples", 10);
        }

        private void ReportProgress(ProgressIndicator progressIndicator)
        {
            ProgressHub.NotifyHowManyProcessed(progressIndicator.Count, progressIndicator.Total);
        }
    } // End of PlaygroundController
}

