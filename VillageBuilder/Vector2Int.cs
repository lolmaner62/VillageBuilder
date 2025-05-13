using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VillageBuilder
{
    namespace VillageBuilder
    {
        public struct Vector2Int
        {
            public int X;
            public int Y;

            public Vector2Int(int x, int y)
            {
                X = x;
                Y = y;
            }

            public static Vector2Int Zero => new Vector2Int(0, 0);

            public static bool operator ==(Vector2Int a, Vector2Int b) => a.X == b.X && a.Y == b.Y;
            public static bool operator !=(Vector2Int a, Vector2Int b) => !(a == b);

            public override bool Equals(object obj)
            {
                return obj is Vector2Int other && this == other;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(X, Y);
            }

            public override string ToString()
            {
                return $"({X}, {Y})";
            }
        }
    }

}
