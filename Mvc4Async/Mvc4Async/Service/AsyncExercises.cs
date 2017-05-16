using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mvc4Async.Hubs;

namespace Mvc4Async.Service
{
    public class AsyncExercises
    {
        public async Task<int> TrivialExample()
        {
            Debug.WriteLine("A.");
            await Task.Delay(100);
            return 10;
        }

        // EXERCISE 1:
        // What will the debug output be? What order will the letters appear?
        public async Task FlowOfControlEx1Part1()
        {
            Debug.WriteLine("Ex1 A");
            Task task = FlowOfControlEx1Part2();
            Debug.WriteLine("Ex1 B");
            await task;
            Debug.WriteLine("Ex1 C");
        }

        // EXERCISE 1
        public async Task FlowOfControlEx1Part2()
        {
            Debug.WriteLine("Ex1 D");
            await Task.Delay(1000);
            Debug.WriteLine("Ex1 E");
        }

        // EXERCISE 2
        // What will the debug output be? What order will the letters appear?
        public async Task FlowOfControlEx2Part1()
        {
            Debug.WriteLine("Ex2 A");
            Task task = FlowOfControlEx2Part2();
            Debug.WriteLine("Ex2 B");
            await task;
            Debug.WriteLine("Ex2 C");
        }

        // EXERCISE 2
        public async Task FlowOfControlEx2Part2()
        {
            var returnedTask = FlowOfControlEx2Part3();
            Debug.WriteLine("Ex2 D");
            await returnedTask;
            Debug.WriteLine("Ex2 E");
        }

        // EXERCISE 2
        public async Task FlowOfControlEx2Part3()
        {
            Debug.WriteLine("Ex2 F");
            for (int numSeconds = 1; numSeconds <= 4; numSeconds++)
            {
                Debug.WriteLine("Ex2 G" + numSeconds);
                await Task.Delay(1000);
            }
        }

        // EXERCISE 3
        // What will the debug output be? What order will the letters appear?
        public async Task FlowOfControlEx3Part1()
        {
            Debug.WriteLine("Ex3 A");
            Task task = FlowOfControlEx3Part2();
            Debug.WriteLine("Ex3 B");
            await task;
            Debug.WriteLine("Ex3 C");
        }

        // EXERCISE 3
        public async Task FlowOfControlEx3Part2()
        {
            FlowOfControlEx3Part3();
            Debug.WriteLine("Ex3 D");
        }

        // EXERCISE 3
        public async Task FlowOfControlEx3Part3()
        {
            Debug.WriteLine("Ex3 E");
            for (int numSeconds = 1; numSeconds <= 4; numSeconds++)
            {
                Debug.WriteLine("Ex3 F" + numSeconds);
                await Task.Delay(1000);
            }
        }

        public int DeadlockExercise()
        {
            var task = TrivialExample();
            return task.Result;
        }

        public async Task ConfigureAwait_Exercise()
        {
            ContextHelper.OutputRequestContext();

            await ConfigureAwait_Exercise_Nested_Code().ConfigureAwait(false);

            ContextHelper.OutputRequestContext();

            //System.Web.HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.NotFound;
        }

        public async Task ConfigureAwait_Exercise_Nested_Code()
        {
            await Task.Delay(1000).ConfigureAwait(false);
            ContextHelper.OutputRequestContext();

            await Task.Delay(1000);
            ContextHelper.OutputRequestContext();

            await Task.Delay(1000);
            ContextHelper.OutputRequestContext();
        }
    }
}