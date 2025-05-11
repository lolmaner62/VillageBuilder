using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VillageBuilder
{
    static public class TileHelper
    {
        public static List<(int, int)> GetNeighbors(int x, int y)
        {
            return new List<(int, int)>
            {
            (x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1),
            };
        }

    }
}
