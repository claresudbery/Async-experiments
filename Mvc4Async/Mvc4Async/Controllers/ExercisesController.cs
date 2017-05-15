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
    public class ExercisesController : Controller
    {
        static CancellationTokenSource _cancellationToken = new CancellationTokenSource();

        public async Task<ActionResult> TemplateEndpoint()
        {
            ViewBag.ExerciseType = "Asynchronous exercises - template.";
            var asyncExercises = new AsyncExercises();
            var task = asyncExercises.TrivialExample();
            return View("AsyncExercises", await task);
        }

        public async Task<ActionResult> FlowOfControlExercise1()
        {
            ViewBag.ExerciseType = "Asynchronous exercises - Flow of Control exercise 1.";
            var asyncExercises = new AsyncExercises();
            var task = asyncExercises.FlowOfControlEx1Part1();
            return View("AsyncExercises", await task);
        }

        public async Task<ActionResult> FlowOfControlExercise2()
        {
            ViewBag.ExerciseType = "Asynchronous exercises - Flow of Control exercise 2.";
            var asyncExercises = new AsyncExercises();
            var task = asyncExercises.FlowOfControlEx2Part1();
            return View("AsyncExercises", await task);
        }
    } // End of ExercisesController
}

