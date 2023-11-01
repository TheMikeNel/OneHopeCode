using UnityEngine;

namespace Costs
{
    public class CostOfResources : MonoBehaviour
    {
        private static float[] oreCost =
        {
            0f,   // Rock
            0.5f, // Coal
            1.5f, // Iron
            3f,   // Copper
            1f,   // Tin
            2f,   // Boxit
            5f    // Gold
        };

        private static float[] ingotCost =
        {
            4f,   // Steel
            8f,   // Bronze
            16f   // Gold
        };

        private static float[] productCost =
        {
            10f,  // Bolt
            20f,  // Pipe
            30f,  // Frame
            50f   // Jewelry
        };

        public static float GetCostOfResource(string tag, int index)
        {
            switch (tag)
            {
                case "Ore":
                    return oreCost[index];

                case "Ingot":
                    return ingotCost[index];

                case "Product":
                    return productCost[index];

                default: return 0f;
            }
        }
    }
}

