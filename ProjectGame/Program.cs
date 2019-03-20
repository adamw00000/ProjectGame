using GameLib;
using System;
using System.Threading.Tasks;

namespace ProjectGame
{
    public static class Program
    {
        private async static Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            int[] actionPriorities = new int[] { 2, 1, 10, 20, 5, 10, 2, 2 };
            Agent exampleRandomAgent = new Agent(2, new RandomDecisionModule(actionPriorities));
            //Task randomAgentTask = exampleRandomAgent.Run();

            Task inputReaderTask = InteractiveInputProvider.ReadInput();
            Agent interactiveAgent1 = new Agent(0, new InteractiveDecisionModule());
            Agent interactiveAgent2 = new Agent(1, new InteractiveDecisionModule());
            Task interactiveAgentTask1 = interactiveAgent1.Run();
            Task interactiveAgentTask2 = interactiveAgent2.Run();

            //await randomAgentTask;
            await interactiveAgentTask1;
            await interactiveAgentTask2;
            await inputReaderTask;
        }
    }
}