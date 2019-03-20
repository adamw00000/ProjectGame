using System.Collections.Generic;

namespace GameLib
{
    public class DiscoveryResult
    {
        public List<(int x, int y, int distance)> Fields;
        public DiscoveryResult(IEnumerable<(int x, int y, int distance)> fields)
        {
            Fields = new List<(int x, int y, int distance)>(fields);
        }
    }
}