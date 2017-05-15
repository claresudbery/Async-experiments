using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Mvc4Async.Hubs;

namespace Mvc4Async.Service
{
    public class AsyncExercises
    {
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