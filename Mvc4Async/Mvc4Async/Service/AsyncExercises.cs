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

        public async Task<int> FlowOfControlEx1Part1()
        {
            Debug.WriteLine("Ex1 A");
            Task<int> task = FlowOfControlEx1Part2();
            Debug.WriteLine("Ex1 B");
            int thing = await task;
            Debug.WriteLine("Ex1 C");
            return 6;
        }

        public async Task<int> FlowOfControlEx1Part2()
        {
            var returnedTask = FlowOfControlEx1Part3();
            Debug.WriteLine("Ex1 D");
            await returnedTask;
            Debug.WriteLine("Ex1 E");
            return 10;
        }

        public async Task<int> FlowOfControlEx1Part3()
        {
            Debug.WriteLine("Ex1 F");
            for (int numSeconds = 1; numSeconds <= 4; numSeconds++)
            {
                Debug.WriteLine("Ex1 G" + numSeconds);
                await Task.Delay(1000);
            }
            return 1;
        }

        public async Task<int> FlowOfControlEx2Part1()
        {
            Debug.WriteLine("Ex2 A");
            Task<int> task = FlowOfControlEx2Part2();
            Debug.WriteLine("Ex2 B");
            int thing = await task;
            Debug.WriteLine("Ex2 C");
            return 6;
        }

        public async Task<int> FlowOfControlEx2Part2()
        {
            FlowOfControlEx2Part3();
            Debug.WriteLine("Ex2 D");
            return 12;
        }

        public async Task<int> FlowOfControlEx2Part3()
        {
            var returnedTask = FlowOfControlEx2Part4();
            Debug.WriteLine("Ex2 E");
            await returnedTask;
            Debug.WriteLine("Ex2 F");
            return 10;
        }

        public async Task<int> FlowOfControlEx2Part4()
        {
            Debug.WriteLine("Ex2 G");
            for (int numSeconds = 1; numSeconds <= 4; numSeconds++)
            {
                Debug.WriteLine("Ex2 H" + numSeconds);
                await Task.Delay(1000);
            }
            return 1;
        }
    }
}