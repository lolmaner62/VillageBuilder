using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using VillageBuilder;
using VillageBuilder.VillageBuilder;
namespace VillageBuilder
{
    public abstract class Building
    {
        public BuildingType Type { get; protected set; }
        public Vector2Int Position { get; private set; }

        protected Building(BuildingType type, Vector2Int position)
        {
            Type = type;
            Position = position;
        }
        public abstract void OnPlaced(Tile[,] grid);
        public abstract void Tick();
    }
}
