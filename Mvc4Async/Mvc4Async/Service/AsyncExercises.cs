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
        public async Task<int> TrivialExample()
        {
            await Task.Delay(100);
            return 1;
        }

        // EXERCISE 1
        public async Task<int> FlowOfControlEx1Part1()
        {
            Debug.WriteLine("Ex1 A");
            Task<int> task = FlowOfControlEx1Part2();
            Debug.WriteLine("Ex1 B");
            int thing = await task;
            Debug.WriteLine("Ex1 C");
            return 6;
        }

        // EXERCISE 1
        public async Task<int> FlowOfControlEx1Part2()
        {
            Debug.WriteLine("Ex1 D");
            await Task.Delay(1000);
            Debug.WriteLine("Ex1 E");
            return 10;
        }

        // EXERCISE 2
        public async Task<int> FlowOfControlEx2Part1()
        {
            Debug.WriteLine("Ex2 A");
            Task<int> task = FlowOfControlEx2Part2();
            Debug.WriteLine("Ex2 B");
            int thing = await task;
            Debug.WriteLine("Ex2 C");
            return 6;
        }

        // EXERCISE 2
        public async Task<int> FlowOfControlEx2Part2()
        {
            var returnedTask = FlowOfControlEx2Part3();
            Debug.WriteLine("Ex2 D");
            await returnedTask;
            Debug.WriteLine("Ex2 E");
            return 10;
        }

        // EXERCISE 2
        public async Task<int> FlowOfControlEx2Part3()
        {
            Debug.WriteLine("Ex2 F");
            for (int numSeconds = 1; numSeconds <= 4; numSeconds++)
            {
                Debug.WriteLine("Ex2 G" + numSeconds);
                await Task.Delay(1000);
            }
            return 1;
        }

        // EXERCISE 3
        public async Task<int> FlowOfControlEx3Part1()
        {
            Debug.WriteLine("Ex3 A");
            Task<int> task = FlowOfControlEx3Part2();
            Debug.WriteLine("Ex3 B");
            int thing = await task;
            Debug.WriteLine("Ex3 C");
            return 6;
        }

        // EXERCISE 3
        public async Task<int> FlowOfControlEx3Part2()
        {
            FlowOfControlEx3Part3();
            Debug.WriteLine("Ex3 D");
            return 12;
        }

        // EXERCISE 3
        public async Task<int> FlowOfControlEx3Part3()
        {
            Debug.WriteLine("Ex3 E");
            for (int numSeconds = 1; numSeconds <= 4; numSeconds++)
            {
                Debug.WriteLine("Ex3 F" + numSeconds);
                await Task.Delay(1000);
            }
            return 1;
        }
    }
}