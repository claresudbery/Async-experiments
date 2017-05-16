using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mvc4Async.Hubs;

namespace Mvc4Async.Service
{
    public class AsyncExamples
    {
        public async Task<int> TrivialExample()
        {
            await Task.Delay(100);
            return 1;
        }

        public async Task<int> Dont_await_long_running_code()
        {
            // We are not using the await keyword here.
            // This means that as soon as an await keyword is hit in LongRunningCode,
            // it will return to this method, which will return to its caller...
            // and as soon as the request context is lost, any further await keywords 
            // in the nested method will have no request context to return to.
            Task task = LongRunningCode();

            return 7;
        }

        // Each await statement causes control to return to the caller.
        // But each subsequent await statement will not get excuted until the previous one has completed.
        // The function returns an unrelated (and rather pointless) Task to the caller.
        // The function does not complete until all awaited tasks have been completed.
        public async Task MultipleAwaitStatements()
        {
            Debug.WriteLine("About to do the first await out of four in MultipleAwaitStatements.");
            await Task.Delay(1000);

            Debug.WriteLine("About to do the second await out of four in MultipleAwaitStatements.");
            await Task.Delay(1000);

            Debug.WriteLine("About to do the third await out of four in MultipleAwaitStatements.");
            await Task.Delay(1000);

            Debug.WriteLine("About to do the fourth await out of four in MultipleAwaitStatements.");
            await Task.Delay(1000);

            Debug.WriteLine("Exiting MultipleAwaitStatements.");
        }

        public async Task<int> FlowOfExecutionExample()
        {
            var task = FlowOfExecutionExamplePart2();

            System.Diagnostics.Debug.WriteLine("The async code was kicked off in the line above.");
            System.Diagnostics.Debug.WriteLine("But we will only reach these lines of code if and when an await statement is hit in the async code.");

            var result = await task;

            System.Diagnostics.Debug.WriteLine("This code is after the outer await statement, so will not be reached until all other code has been executed.");

            return result;
        }

        public async Task<int> DeadlockExample()
        {
            await Task.Delay(1000);

            System.Diagnostics.Debug.WriteLine("The await statement above is waiting for the request context, but it will never get it back.");
            System.Diagnostics.Debug.WriteLine("This is because the request context is locked by Task.Result in the calling method.");
            System.Diagnostics.Debug.WriteLine("Therefore these lines of code will never be reached.");

            return 10;
        }

        public async Task<int> FlowOfExecutionExamplePart2()
        {
            System.Diagnostics.Debug.WriteLine("We are now in an async method, but we have not hit an await statement yet.");
            System.Diagnostics.Debug.WriteLine("This means that this code will be executed before any lines after this method was called.");

            await FlowOfExecutionExamplePart3();

            System.Diagnostics.Debug.WriteLine("This code is after the nested await statement, so will not be reached until after the code in the calling method is executed.");

            return 12;
        }

        public async Task<int> FlowOfExecutionExamplePart3()
        {
            await Task.Delay(1000);

            // This return statement is not executed until after the task above has been completed.
            return 12;
        }

        // Here we don't use the await keyword, and don't explicitly return a Task object,
        // but we have marked the return type as Task, AND marked the method async,
        // so we automatically return a Task object.
        // BUT the task that is returned is NOT the task returned by LongRunningCode! 
        // A brand new one will be created! This is because we don't use the await keyword.
        // This means we will not see all the debug strings from LongRunningCode.
        public async Task Marked_Async_With_Empty_Task_And_NOT_Calling_Await()
        {
            LongRunningCode();
        }

        // Here we DO use the await keyword, although we don't explicitly return a Task object.
        // But the return type is Task, AND the method is async, AND we use await,
        // so we will not return until LongRunningCode has completed.
        // So, in this case we will see all the expected debug strings from LongRunningCode.
        public async Task Marked_Async_With_Empty_Task_And_Calling_Await()
        {
            await LongRunningCode();
        }

        public async Task<int> Leave_The_Request_Context_In_Some_Places_But_Not_All()
        {
            // At this point the current http context and sychronization contexts are both populated.
            ContextHelper.OutputRequestAndSynchronizationContexts("Before nested call (not using ConfigureAwait(false)): ");

            await AsyncCodeResumingInADifferentContext();

            // We are now back in the original request context (because we did not use ConfigureAwait(false)).
            // At this point the current http context and sychronization contexts are populated again.
            ContextHelper.OutputRequestAndSynchronizationContexts("After nested call (not using ConfigureAwait(false)): ");

            // We should be able to access the response code and change it.
            System.Web.HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.NotFound;

            return 12;
        }

        public async Task<int> Leave_The_Request_Context_In_All_Places()
        {
            // At this point the current http context and sychronization contexts are both populated.
            ContextHelper.OutputRequestAndSynchronizationContexts("Before ConfigureAwait(false): ");

            await AsyncCodeResumingInADifferentContext().ConfigureAwait(false);

            // We have now left the main request context.
            // At this point the current http context and sychronization contexts are no longer populated.
            // This means we cannot access the response any more to change the response status code.
            ContextHelper.OutputRequestAndSynchronizationContexts("After ConfigureAwait(false): ");

            return 12;
        }

        public async Task<int> AsyncCodeResumingInADifferentContext()
        {
            await Task.Delay(1000).ConfigureAwait(false);

            // Do some more processing
            Debug.WriteLine("All code after this point will execute in a context that is not the main request context.");
            Debug.WriteLine("This means it can continue to execute, even if the main request has terminated.");

            await Task.Delay(1000);
            Debug.WriteLine("First delay out of five.");

            await Task.Delay(1000);
            Debug.WriteLine("Second delay out of five.");

            await Task.Delay(1000);
            Debug.WriteLine("Third delay out of five.");

            await Task.Delay(1000);
            Debug.WriteLine("Fourth delay out of five.");

            await Task.Delay(1000);
            Debug.WriteLine("Fifth delay out of five.");

            return 12;
        }

        public async Task<int> AsyncMethodWithProgress(IProgress<ProgressIndicator> progress = null)
        {
            int totalSeconds = 11;
            System.Diagnostics.Debug.WriteLine($"I am at the beginning of a task that will take {totalSeconds} seconds.");
            for (int numSeconds = 1; numSeconds <= totalSeconds; numSeconds++)
            {
                await Task.Delay(1000);
                if (progress != null)
                {
                    progress.Report(new ProgressIndicator { Count = numSeconds, Total = totalSeconds });
                }
                System.Diagnostics.Debug.WriteLine($"This task should take {totalSeconds} seconds. Number of seconds so far: {numSeconds}");
            }
            
            return 1;
        }

        public async Task<int> AsyncMethodWithCancellation(
            CancellationToken cancellationToken,
            IProgress<ProgressIndicator> progress = null)
        {
            int totalSeconds = 11;
            System.Diagnostics.Debug.WriteLine($"I am at the beginning of a task that will take {totalSeconds} seconds.");
            for (int numSeconds = 1; numSeconds <= totalSeconds; numSeconds++)
            {
                await Task.Delay(1000);
                if (progress != null)
                {
                    progress.Report(new ProgressIndicator { Count = numSeconds, Total = totalSeconds });
                }
                cancellationToken.ThrowIfCancellationRequested();
                System.Diagnostics.Debug.WriteLine($"This task should take {totalSeconds} seconds. Number of seconds so far: {numSeconds}");
            }
            
            return 1;
        }

        public async Task LongRunningCode()
        {
            System.Diagnostics.Debug.WriteLine("I am at the beginning of a task that will take about 11 seconds.");
            for (int numSeconds = 1; numSeconds <= 11; numSeconds++)
            {
                // This await statement will get executed repeatedly.
                // Every time it is executed, control will return to the caller 
                // - as long as the request context is still available.
                await Task.Delay(1000);
                System.Diagnostics.Debug.WriteLine("Number of seconds so far: " + numSeconds);
            }
        }

        public async Task Parallel_Asynchronous()
        {
            Debug.WriteLine($"About to start parallel asynchronous code. Thread id: {Thread.CurrentThread.ManagedThreadId}");

            // By giving the tasks staggered starts, we increase the chances of them being run on the same thread.
            Task[] parallelAsynchronousTasks = new Task[] {
                Start_After_n_Milliseconds(100),
                Start_After_n_Milliseconds(200),
                Start_After_n_Milliseconds(300),
                Start_After_n_Milliseconds(400),
                Start_After_n_Milliseconds(500),
                Start_After_n_Milliseconds(600),
                Start_After_n_Milliseconds(700),
                Start_After_n_Milliseconds(800)};

            // Because we use the await statement, we free up the main request context.
            await Task.WhenAll(parallelAsynchronousTasks);
        }

        public async Task Start_After_n_Milliseconds(int firstDelay)
        {
            await Task.Delay(firstDelay);
            Debug.WriteLine($"First delay (starting with {firstDelay} milliseconds). Thread id: {Thread.CurrentThread.ManagedThreadId}");
            await Task.Delay(1000);
            Debug.WriteLine($"Second delay (starting with {firstDelay} milliseconds). Thread id: {Thread.CurrentThread.ManagedThreadId}");
            await Task.Delay(1000);
            Debug.WriteLine($"Third delay (starting with {firstDelay} milliseconds). Thread id: {Thread.CurrentThread.ManagedThreadId}");
        }

        public void Parallel_Synchronous()
        { 
            var tasks = new List<Task>();

            Debug.WriteLine($"About to start parallel synchronous code. Thread id: {Thread.CurrentThread.ManagedThreadId}");
            tasks.Add(Task.Factory.StartNew(() => Sleep_After_n_Milliseconds(100)));
            tasks.Add(Task.Factory.StartNew(() => Sleep_After_n_Milliseconds(200)));
            tasks.Add(Task.Factory.StartNew(() => Sleep_After_n_Milliseconds(300)));
            tasks.Add(Task.Factory.StartNew(() => Sleep_After_n_Milliseconds(400)));
            tasks.Add(Task.Factory.StartNew(() => Sleep_After_n_Milliseconds(500)));
            tasks.Add(Task.Factory.StartNew(() => Sleep_After_n_Milliseconds(600)));
            tasks.Add(Task.Factory.StartNew(() => Sleep_After_n_Milliseconds(700)));
            tasks.Add(Task.Factory.StartNew(() => Sleep_After_n_Milliseconds(800)));

            // Because we use Task.WaitAll, the main thread is blocked while it waits for the parallel tasks to complete.
            Task.WaitAll(tasks.ToArray());
        }

        public void Sleep_After_n_Milliseconds(int firstSleep)
        {
            Thread.Sleep(firstSleep);
            Debug.WriteLine($"First sleep (starting with {firstSleep} milliseconds). Thread id: {Thread.CurrentThread.ManagedThreadId}");
            Thread.Sleep(1000);
            Debug.WriteLine($"Second sleep (starting with {firstSleep} milliseconds). Thread id: {Thread.CurrentThread.ManagedThreadId}");
            Thread.Sleep(1000);
            Debug.WriteLine($"Third sleep (starting with {firstSleep} milliseconds). Thread id: {Thread.CurrentThread.ManagedThreadId}");
        }
    }
}