using System;
using System.Diagnostics;
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
            Task<int> task = LongRunningCode();

            return 7;
        }

        public async Task<int> LongRunningCode()
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
            
            return 1;
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

        public async Task<int> MarkedAsyncWithIntegerTaskReturningRandomValueToDifferentContext(
            CancellationToken cancellationToken,
            IProgress<ProgressIndicator> progress = null)
        {
            int totalSeconds = 11;
            System.Diagnostics.Debug.WriteLine($"P. I am at the beginning of a task that will take {totalSeconds} seconds.");
            for (int numSeconds = 1; numSeconds <= totalSeconds; numSeconds++)
            {
                // The use of ConfigureAwait means that after the delay, we will return to a different context
                // (not the request context).
                await Task.Delay(1000).ConfigureAwait(false);
                if (progress != null)
                {
                    progress.Report(new ProgressIndicator { Count = numSeconds, Total = totalSeconds });
                }
                cancellationToken.ThrowIfCancellationRequested();
                System.Diagnostics.Debug.WriteLine($"Q. This task should take {totalSeconds} seconds. Number of seconds so far: {numSeconds}");
            }

            // Note that the return value of 1 will automatically get wrapped in a task,
            // and that task will have nothing to do with the task we are awaiting above??
            // Yes, I guess so - the await keyword above returns control to this point once
            // it has completed, and the new code is executed. The calling code will get the
            // returned int when it uses the await keyword - but not until the above task has returned control.
            return 1;
        }
    }
}