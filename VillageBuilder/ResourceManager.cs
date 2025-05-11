using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace VillageBuilder
{
    static class ResourceManager
    {
        private static Dictionary<ResourceType, int> resources = new();
        public static void Init()
        {
            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
            {
                resources[type] = 100;
            }
        }
        public static bool HasEnough(ResourceType type, int amount) => resources[type] >= amount;
        public static void Spend(ResourceType type, int amount)
        {
            if (HasEnough(type, amount))
            {
                resources[type] -= amount;
            }
        }
        public static int Get(ResourceType type) => resources[type];
        public static void Add(ResourceType type, int amount)
        {
            resources[type] += amount;
        }


    }
}
