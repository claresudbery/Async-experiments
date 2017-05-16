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
            await asyncExercises.TrivialExample();
            return View("AsyncExercises");
        }

        public async Task<ActionResult> FlowOfControlExercise1()
        {
            ViewBag.ExerciseType = "Asynchronous exercises - Flow of Control exercise 1.";
            var asyncExercises = new AsyncExercises();
            await asyncExercises.FlowOfControlEx1Part1();
            return View("AsyncExercises");
        }

        public async Task<ActionResult> FlowOfControlExercise2()
        {
            ViewBag.ExerciseType = "Asynchronous exercises - Flow of Control exercise 2.";
            var asyncExercises = new AsyncExercises();
            await asyncExercises.FlowOfControlEx2Part1();
            return View("AsyncExercises");
        }

        public async Task<ActionResult> FlowOfControlExercise3()
        {
            ViewBag.ExerciseType = "Asynchronous exercises - Flow of Control exercise 3.";
            var asyncExercises = new AsyncExercises();
            await asyncExercises.FlowOfControlEx3Part1();
            return View("AsyncExercises");
        }

        // Deadlock exercise:
        // The purpose of this exercise is to change the debug output from just "A." to "A." followed by "B.".
        public ActionResult Deadlock_Exercise()
        {
            ViewBag.ExerciseType = "Asynchronous exercises - Create a deadlock. This action will never complete!";
            var asyncExercises = new AsyncExercises();
            
            asyncExercises.DeadlockExercise();

            Debug.WriteLine("B.");

            return View("AsyncExercises");
        }

        // Exercises:
        // Part 1:
        //    The debug output from this is currently as follows:
        //          available
        //          available
        //          null
        //          null
        //          null
        //          null
        //          null
        //    Can you change the output to be this, instead?
        //          available
        //          available
        //          available
        //          null
        //          null
        //          available
        //          null
        // Part 2:
        //    Can you change things so that a response status code of BadRequest is displayed on the page?
        //    NOTE: You are NOT allowed to do this by manipulating the Controller.Response. 
        //    You must set the response in one of the methods in the AsyncExercises class.
        public async Task<ActionResult> ConfigureAwait_Exercise()
        {
            ViewBag.ExerciseType = "Asynchronous exercises - ConfigureAwait exercise";
            var asyncExercises = new AsyncExercises();
            
            ContextHelper.OutputRequestContext();
            
            await asyncExercises.ConfigureAwait_Exercise().ConfigureAwait(false);

            ContextHelper.OutputRequestContext();

            return View("AsyncExercises");
        }

        // Exercise:
        // When this endpoint is hit, the user should see something like the following:
        //  1. Processed some apples. (33%)
        //  2. Processed some pears. (66%)
        //  3. Processed some cucumbers. (100%)
        //      (Hint 1: look at ExamplesController.ReportProgress)
        //      (Hint 2: look at ProgressHub.NotifyWhatHasBeenProcessed)
        public async Task<ActionResult> Progress_Exercise()
        {
            ViewBag.ExerciseType = "Asynchronous exercises - Progress Exercise.";
            var asyncExercises = new AsyncExercises();

            return View("AsyncExercises");
        }

        // Exercise:
        // When this endpoint is hit, the user should see something like the output below.
        // BUT, when the user clicks the cancel link, they should be able to stop the progress.
        //  1. Processed some apples. (20%)
        //  2. Processed some pears. (40%)
        //  3. Processed some cucumbers. (60%)
        //  4. Processed some cabbages. (80%)
        //  5. Processed some kawasakis. (100%)
        //      (Hint 1: look at ExamplesController.ThisActionCanBeCancelled)
        //      (Hint 2: look at ExamplesController.Cancel)
        public async Task<ActionResult> Cancel_Exercise()
        {
            ViewBag.ExerciseType = "Asynchronous exercises - Cancellation Exercise.";
            var asyncExercises = new AsyncExercises();

            return View("AsyncExercises");
        }

        public async Task<ActionResult> Cancel()
        {
            ViewBag.ExerciseType = "Asynchronous exercises - Async operation was cancelled.";

            return View("AsyncExercises");
        }
    } // End of ExercisesController
}

