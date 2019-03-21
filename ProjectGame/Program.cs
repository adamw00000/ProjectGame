using Avalonia;
using Avalonia.Logging.Serilog;
using Avalonia.Threading;
using GameLib;
using ProjectGameGUI;
using ProjectGameGUI.ViewModels;
using ProjectGameGUI.Views;
using SharpDX.Direct2D1;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectGame
{
    public static class Program
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private async static Task Main(string[] args)
        {
            logger.Info("Hello world!");
            Console.WriteLine("Hello World!");

            int[] actionPriorities = new int[] { 2, 1, 10, 20, 5, 10, 2, 2 };
            Agent exampleRandomAgent = new Agent(2, new RandomDecisionModule(actionPriorities));
            //Task randomAgentTask = exampleRandomAgent.Run();

            Task inputReaderTask = InteractiveInputProvider.ReadInput();
            Agent interactiveAgent1 = new Agent(0, new InteractiveDecisionModule());
            Agent interactiveAgent2 = new Agent(1, new InteractiveDecisionModule());
            Task interactiveAgentTask1 = interactiveAgent1.Run();
            Task interactiveAgentTask2 = interactiveAgent2.Run();

            var rules = new GameRules(boardWidth: 6, boardHeight: 9, goalAreaHeight: 2, agentStartX: 4, agentStartY: 5);

            MainWindowViewModel mainWindowViewModel = new MainWindowViewModel();

            var app = AppBuilder.Configure<App>().UsePlatformDetect();
            app.Start<MainWindow>();


            //await randomAgentTask;
            await interactiveAgentTask1;
            await interactiveAgentTask2;
            await inputReaderTask;

            
        }
    }
}