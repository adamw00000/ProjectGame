using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GameLib
{
    public static class InteractiveInputProvider
    {
        private readonly static HashSet<ConsoleKey> validKeys = new HashSet<ConsoleKey>() {
            ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.LeftArrow, ConsoleKey.RightArrow,
            ConsoleKey.Q, ConsoleKey.W, ConsoleKey.E, ConsoleKey.R,
            ConsoleKey.A, ConsoleKey.S, ConsoleKey.D
        };

        public static object RegisterInteractiveAgentLock = new object();
        private readonly static Dictionary<int, char> registeredAgents = new Dictionary<int, char>();
        private readonly static HashSet<char> activeAgents = new HashSet<char>();

        private static int agentCount = 0;

        private readonly static Dictionary<int, BlockingCollection<ConsoleKey>> queues = new Dictionary<int, BlockingCollection<ConsoleKey>>();

        public static void Register(int id)
        {
            lock(RegisterInteractiveAgentLock)
            {
                if (agentCount > 9)
                    throw new InteractiveModuleException("You can have at most 10 Interactive Agent instances at the same time!");

                char agentChar = (char)(agentCount + '0');
                registeredAgents.Add(id, agentChar);
                queues.Add(id, new BlockingCollection<ConsoleKey>());
                agentCount++;
            }
        }

        public static async Task<ConsoleKey> GetKey(int id)
        {
            ConsoleKey c = await Task.Run(() => queues[id].Take());

            return c;
        }

        public static async Task ReadInput()
        {
            while (true)
            {
                var input = await Task.Run(() => Console.ReadKey());
                ConsoleKey consoleKey = input.Key;
                char c = input.KeyChar;

                if (c >= '0' && c < (char)(agentCount + '0'))
                {
                    if (activeAgents.Contains(c))
                    {
                        activeAgents.Remove(c);
                    }
                    else
                    {
                        activeAgents.Add(c);
                    }
                    continue;
                }

                if (!validKeys.Contains(consoleKey))
                    continue;

                foreach (var (id, agentChar) in registeredAgents)
                {
                    if (activeAgents.Contains(agentChar))
                    {
                        queues[id].Add(consoleKey);
                    }
                }
            }

        }
    }
}
