using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VillageBuilder.VillageBuilder;

namespace VillageBuilder
{
    class House : Building
    {
        public House(Vector2Int position) : base(BuildingType.House, position) { }

        public override void OnPlaced(Tile[,] grid)
        {
        }

        public override void Tick()
        {
        }
    }

    class Farm : Building
    {
        public Farm(Vector2Int position) : base(BuildingType.Farm, position) { }

        public override void OnPlaced(Tile[,] grid)
        {
        }

        public override void Tick()
        {
        }
    }

    class Quarry : Building
    {
        public Quarry(Vector2Int position) : base(BuildingType.Quarry, position) { }

        public override void OnPlaced(Tile[,] grid)
        {
        }

        public override void Tick()
        {
        }
    }

}
