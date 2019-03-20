using GameLib;
using ConnectionLib;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectGame
{
    public static class Program
    {
        private async static Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            int[] actionPriorities = new int[] { 2, 1, 10, 20, 5, 10, 2, 2 };

            LocalCommunicationServer cs = new LocalCommunicationServer();
            GMLocalConnection gMLocalConnection = new GMLocalConnection(cs);
            GameRules rules = new GameRules(teamSize: 2, baseTimePenalty: 100);
            GameMaster gm = new GameMaster(rules, gMLocalConnection);

            Task inputReaderTask = InteractiveInputProvider.ReadInput();
            Agent interactiveAgent1 = new Agent(0, new InteractiveDecisionModule(), new AgentLocalConnection(cs));
            Agent interactiveAgent2 = new Agent(1, new InteractiveDecisionModule(), new AgentLocalConnection(cs));
            Task interactiveAgentTask1 = interactiveAgent1.Run();
            Task interactiveAgentTask2 = interactiveAgent2.Run();

            Task.Run(() => { gm.ListenJoiningAndStart(); });
            for (int i = 0; i < rules.TeamSize; ++i)
            {
                Agent exampleRandomAgent = new Agent(2*i + 2, new RandomDecisionModule(actionPriorities), new AgentLocalConnection(cs));
                Task.Run(() => exampleRandomAgent.JoinGame(Team.Red));

                Agent exampleRandomAgent2 = new Agent(2*i + 3, new RandomDecisionModule(actionPriorities), new AgentLocalConnection(cs));
                Task.Run(() => exampleRandomAgent2.JoinGame(Team.Blue));
            }
            Thread.Sleep(10000000);
        }
    }
}