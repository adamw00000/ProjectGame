using GameLib;
using ConnectionLib;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectGame
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            int[] actionPriorities = new int[] { 0, 0, 0, 20, 0, 0, 0, 0 };

            LocalCommunicationServer cs = new LocalCommunicationServer();
            GMLocalConnection gMLocalConnection = new GMLocalConnection(cs);
            GameRules rules = new GameRules(teamSize: 2, baseTimePenalty:100);
            GameMaster gm = new GameMaster(rules, gMLocalConnection);
            Task.Run(() => { gm.ListenJoiningAndStart(); });
            for(int i = 0; i < rules.TeamSize; ++i)
            {
                Agent exampleRandomAgent = new Agent(new RandomDecisionModule(actionPriorities), new AgentLocalConnection(cs));
                Task.Run(() => exampleRandomAgent.JoinGame(Team.Red));

                Agent exampleRandomAgent2 = new Agent(new RandomDecisionModule(actionPriorities), new AgentLocalConnection(cs));
                Task.Run(() => exampleRandomAgent2.JoinGame(Team.Blue));
            }
            Thread.Sleep(10000000);
        }
    }
}