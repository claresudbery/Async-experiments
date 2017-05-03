using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mvc4Async.Service
{
    public class AsyncExperiments
    {
        public async Task<int> PlaceATaskInADifferentContext(int result)
        {
            System.Diagnostics.Debug.WriteLine("A1. I'm at the top level of nesting.");
            System.Diagnostics.Debug.WriteLine("A2. I'm going to await a task in a context other than the request context.");

            // The call to ConfigureAwait means that when the task completes, it will come back to a different context.
            // Therefore NOT the request context, which means that the ASP.Net request will probably have terminated.
            await MarkedAsyncWithIntegerTaskReturningRandomValueToDifferentContext().ConfigureAwait(false);

            System.Diagnostics.Debug.WriteLine("B. The task in the different context has now completed.");

            return result;
        }

        public async Task<int> NestedAsyncExperimentsNumberSix()
        {
            System.Diagnostics.Debug.WriteLine("C1. I'm at the top level of nesting.");
            System.Diagnostics.Debug.WriteLine("C2. I'm going to call the highest level of nesting so far.");

            Task<int> task = NestedAsyncExperimentsNumberFive();

            System.Diagnostics.Debug.WriteLine("D. I'm just going to write this before I await the latest call (I'm at the top level now).");
            
            int thing = await task;

            System.Diagnostics.Debug.WriteLine("E. These are the final lines of text which should appear (I think). This is after the very last await call.");

            return 6;
        }

        public async Task<int> NestedAsyncExperimentsNumberSeven()
        {
            System.Diagnostics.Debug.WriteLine("F1. I'm at the top level of nesting.");
            System.Diagnostics.Debug.WriteLine("F2. I'm going to call the highest level of nesting so far.");

            //await Task.Delay(10);
            Task<int> task = NestedAsyncExperimentsNumberFive();

            System.Diagnostics.Debug.WriteLine("G. I'm not going to await the latest call. So what will happen to control when the 11-second stuff finishes executing??");

            return 7;
        }

        public async Task<int> NestedAsyncExperimentsNumberFive()
        {
            //await Task.Delay(10);
            var task = NestedAsyncExperimentsNumberFour();

            System.Diagnostics.Debug.WriteLine("H. I just called the latest method in the nested chain for the 11-second task. Next I'm going to await its results.");

            await task;

            System.Diagnostics.Debug.WriteLine("I. I just awaited the results for a chain of calls which call the 11-second task.");

            return 12;
        }

        public async Task<int> NestedAsyncExperimentsNumberFour()
        {
            //await Task.Delay(10);
            NestedAsyncExperimentsNumberThree();

            System.Diagnostics.Debug.WriteLine("J. I just called the method which calls the method which calls the method which starts the 11-second task! I'm in an async method this time, but I'm not awaiting anything.");

            return 12;
        }

        public void NestedAsyncExperimentsNumberThree()
        {
            NestedAsyncExperimentsNumberTwo();

            System.Diagnostics.Debug.WriteLine("K. I just called the method which calls the method which starts the 11-second task! I'm not even in an async method!");
        }

        public Task<int> NestedAsyncExperimentsNumberTwo()
        {
            NestedAsyncExperimentsNumberOne();

            System.Diagnostics.Debug.WriteLine("L. I just called the method which starts the 11-second task. I did not await its results.");

            return new Task<int>(() => 10);
        }

        public async Task<int> NestedAsyncExperimentsNumberOne()
        {
            //await Task.Delay(10);
            var returnedTask = MarkedAsyncWithIntegerTaskReturningRandomValue();

            // Do some more processing.
            System.Diagnostics.Debug.WriteLine("M1. I just started a task that will take 11 seconds. I did not await its results.");
            System.Diagnostics.Debug.WriteLine("M2. I started an 11-second task and still haven't called await.");
            System.Diagnostics.Debug.WriteLine("M3. I started an 11-second task and now I'm going to call await on it.");

            // NOW await the result, which means that control should return to the calling method while we wait.
            await returnedTask;
            
            // Do some more processing.
            System.Diagnostics.Debug.WriteLine("N. This code is after the await statement for the 11-second task, so we shouldn't get here until after the 11 seconds are up.");

            return 10;
        }

        public Task<int> ReturnTaskWithoutBeingAsync()
        {
            HttpClient client = new HttpClient();

            var returnedTask = MarkedAsyncWithIntegerTaskReturningRandomValue();

            // Do some more processing.
            System.Diagnostics.Debug.WriteLine("O. I just started a task that will take 11 seconds. I did not await its results.");

            return returnedTask;
        }

        public async Task<int> MarkedAsyncWithIntegerTaskReturningRandomValue()
        {
            System.Diagnostics.Debug.WriteLine("P. I am at the beginnong of a task that will take 11 seconds.");
            for (int numSeconds = 1; numSeconds <= 11; numSeconds++)
            {
                await Task.Delay(1000);
                System.Diagnostics.Debug.WriteLine("Q. This task should take 11 seconds. Number of seconds so far: " + numSeconds);
            }

            // Note that the return value of 1 will automatically get wrapped in a task,
            // and that task will have nothing to do with the task we are awaiting above??
            // Yes, I guess so - the await keyword above returns control to this point once
            // it has completed, and the new code is executed. The calling code will get the
            // returned int when it uses the await keyword - but not until the above task has returned control.
            return 1;
        }

        public async Task<int> MarkedAsyncWithIntegerTaskReturningRandomValueToDifferentContext()
        {
            System.Diagnostics.Debug.WriteLine("P. I am at the beginnong of a task that will take 11 seconds.");
            for (int numSeconds = 1; numSeconds <= 11; numSeconds++)
            {
                // The use of ConfigureAwait means that after the delay, we will return to a different context
                // (not the request context).
                await Task.Delay(1000).ConfigureAwait(false);
                System.Diagnostics.Debug.WriteLine("Q. This task should take 11 seconds. Number of seconds so far: " + numSeconds);
            }

            // Note that the return value of 1 will automatically get wrapped in a task,
            // and that task will have nothing to do with the task we are awaiting above??
            // Yes, I guess so - the await keyword above returns control to this point once
            // it has completed, and the new code is executed. The calling code will get the
            // returned int when it uses the await keyword - but not until the above task has returned control.
            return 1;
        }

        public async Task<int> MarkedAsyncWithIntegerTaskReturnsAwaitedTask()
        {
            var integerTask = await NotMarkedAsyncButIntegerTaskReturned();

            // There are two statements here for clarity 
            // - in fact this could have been done all in one return statement.
            return integerTask;
        }

        public async Task<int> MarkedAsyncWithIntegerTaskAndNotCallingAwait()
        {
            return 1;
        }

        // Note that you can't explicitly return a task object 
        // unless you have prefixed it with the await keyword.
        // The following method is commented out because it will not compile.
        //public async Task<int> MarkedAsyncWithIntegerTaskAndNotCallingAwaitButReturningTaskObject()
        //{
        //    return NotMarkedAsyncButIntegerTaskReturned();
        //}

        // Note that although we haven't used the await keyword,
        // and we are not explicitly returning a Task object,
        // the fact that we have marked the return type as Task,
        // AND we marked the method async,
        // means that we automatically return a Task object.
        // Otherwise we have to explicitly return a task (see NotMarkedAsyncButEmptyTaskReturned below)
        public async Task MarkedAsyncWithEmptyTaskAndNotCallingAwait()
        {
            Task.Delay(1);
        }

        private async Task ProofThatThePreviousMethodReturnsATask()
        {
            // Even though we did not explicitly return a task object, 
            // the fact we can await it proves it is in fact returning a Task.
            await MarkedAsyncWithEmptyTaskAndNotCallingAwait();
        }

        public async Task MarkedAsyncWithEmptyTaskButNotPerformingTask()
        {
            // Just perform any random action.
            var thing = 1;
        }

        private async Task ProofThatTheLatestMethodReturnsATask()
        {
            // Even though we did not explicitly return a task object, 
            // the fact we can await it proves it is in fact returning a Task.
            await MarkedAsyncWithEmptyTaskButNotPerformingTask();
        }

        private void DoNothing()
        {
            var thing = 1;
        }

        // This method is commented out because it won't actually compile!
        //private async Task ProofThatYouCannotAwaitSomethingThatDoesNotReturnATask()
        //{
        //    // Even though we did not explicitly return a task object, 
        //    // the fact we can await it proves it is in fact returning a Task.
        //    await DoNothing();
        //}

        // Note that in order for this method to return a task,
        // we do not have to explicitly return a Task object.
        // The fact that we have used the await keyword means that a task is created and automatically returned.
        // !! What if we put code after await? what if we have two awaits?
        public async Task MarkedAsyncWithEmptyTaskAndCallingAwait()
        {
            await NotMarkedAsyncButEmptyTaskReturned();
        }

        // We can mark the return type as Task and still put other code after the task.
        // The method will still return a task relating to the await keyword.
        public async Task ReturnATaskEvenThoughThereIsOtherCodeAfterTheTask()
        {
            await NotMarkedAsyncButEmptyTaskReturned();

            var thing = 2;
            var thing2 = 3;
            var thing3 = thing + thing2;

            DoNothing();
        }

        // We can mark the return type as Task and have multiple await statements.
        // The method will still return a task.
        // Which of the await statements will the returned task encapsulate??
        // I guess it doesn't matter - each await statement returns control once it has completed,
        // so in fact the multiple await statements will effectively be invoked synchronously
        // - one after the other.
        public async Task ReturnATaskEvenThoughWeHaveUsedTheAwaitKeywordTwice()
        {
            await NotMarkedAsyncButEmptyTaskReturned();

            await ReturnATaskEvenThoughThereIsOtherCodeAfterTheTask();

            await MarkedAsyncWithEmptyTaskAndCallingAwait();
        }

        public Task NotMarkedAsyncButEmptyTaskReturned()
        {
            Task task = Task.Delay(1);

            return task;
        }

        public Task<int> NotMarkedAsyncButIntegerTaskReturned()
        {
            return new Task<int>(() => 256);
        }

        public async Task<int> FlowOfExecutionExample()
        {
            var task = FlowOfExecutionExamplePart2();
            
            System.Diagnostics.Debug.WriteLine("S1. The async code was kicked off in the line above.");
            System.Diagnostics.Debug.WriteLine("S2. But we will only reach these lines of code if and when an await statement is hit in the async code.");

            var result = await task;

            System.Diagnostics.Debug.WriteLine("T. This code is after the outer await statement, so will not be reached until all other code has been executed.");

            return result;
        }

        public async Task<int> FlowOfExecutionExamplePart2()
        {
            System.Diagnostics.Debug.WriteLine("U1. We are now in an async method, but we have not hit an await statement yet.");
            System.Diagnostics.Debug.WriteLine("U2. This means that this code will be executed before any lines after this method was called.");

            await FlowOfExecutionExamplePart3();

            System.Diagnostics.Debug.WriteLine("V. This code is after the nested await statement, so will not be reached until after the code in the calling method is executed.");

            return 12;
        }

        public async Task<int> FlowOfExecutionExamplePart3()
        {
            await Task.Delay(1000);
            return 12;
        }
    }
}