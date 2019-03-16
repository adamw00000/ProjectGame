using GameLib;
using System;

namespace ProjectGame
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            int[] actionPriorities = new int[] { 2, 1, 10, 20, 5, 10, 2, 2 };
            Agent exampleRandomAgent = new Agent(new RandomDecisionModule(actionPriorities));
            exampleRandomAgent.Run();
        }
    }
}