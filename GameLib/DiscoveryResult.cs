using System.Collections.Generic;

namespace GameLib
{
    public class DiscoveryResult
    {
        public List<(int x, int y, int dist)> Fields;
        public DiscoveryResult(IEnumerable<(int x, int y, int dist)> fields)
        {
            Fields = new List<(int x, int y, int dist)>(fields);
        }
    }
}