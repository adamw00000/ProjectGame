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
            int[] actionPriorities = new int[] { 2, 1, 10, 20, 0, 10, 0, 0 };

            LocalCommunicationServer cs = new LocalCommunicationServer();
            GMLocalConnection gMLocalConnection = new GMLocalConnection(cs);
            GameRules rules = new GameRules(teamSize: 2, baseTimePenalty: 1, goalCount: 1, badPieceProbability: 0, pieceSpawnInterval: 250, boardHeight: 3, boardWidth: 4, goalAreaHeight: 1);
            GameMaster gm = new GameMaster(rules, gMLocalConnection, new GameMasterMessageFactory());

            Task.Run(() => { gm.ListenJoiningAndStart(); });
            //Task gmListener = gm.ListenJoiningAndStart();
            for (int i = 0; i < rules.TeamSize; ++i)
            {
                Console.WriteLine(i);
                Agent exampleRandomAgent = new Agent(2*i + 2, new RandomDecisionModule(actionPriorities), new AgentLocalConnection(cs));
                new Thread(() => exampleRandomAgent.Run(Team.Red)).Start();

                Agent exampleRandomAgent2 = new Agent(2*i + 3, new RandomDecisionModule(actionPriorities), new AgentLocalConnection(cs));
                new Thread(() => exampleRandomAgent2.Run(Team.Blue)).Start();
            }
            Thread.Sleep(10000000);
        }
    }
}