using UnityEngine;

namespace Costs
{
    public class CostOfResources : MonoBehaviour
    {
        private static readonly float[] oreCost =
        {
            0f,   // Rock
            0.5f, // Coal
            1.5f, // Iron
            3f,   // Copper
            1f,   // Tin
            2f,   // Boxit
            5f    // Gold
        };

        private static readonly float[] ingotCost =
        {
            30f,   // Steel
            50f,   // Bronze
            40f,  // Aluminum
            70f   // Gold
        };
        
        private static readonly float[] productCost =
        {
            40f,  // Bolt
            65f,  // Pipe
            100f,  // Frame
            160f   // Jewelry
        };

        /// <summary>
        /// ¬озвращает стоимость производства ресурсов: Vector4( 
        /// ресурс 1: индекс, количество, 
        /// русурс 2: индекс, количество.)
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static Vector4 GetCostToProduction(string type, int index)
        {
            // ƒл€ производства слитков
            if (type == "Ingot")
            {
                //           #1 indx  value   #2 indx  value
                Vector4 steelCost = new(1, 10, 2, 10);
                Vector4 bronzeCost = new(3, 10, 4, 10);
                Vector4 alumCost = new(1, 10, 5, 10);
                Vector4 goldCost = new(1, 10, 6, 10);

                return index switch
                {
                    // Steel
                    0 => steelCost,

                    // Bronze
                    1 => bronzeCost,

                    // Aluminum
                    2 => alumCost,

                    // Gold
                    3 => goldCost,

                    _ => Vector4.zero,
                };
            }

            else if (type == "Product")
            {
                // ƒл€ производства товара, первый ресурс всегда камень
                //           #1 indx  value   #2 indx  value
                Vector4 boltCost = new(0, 10, 0, 1);
                Vector4 pipeCost = new(0, 10, 1, 1);
                Vector4 frameCost = new(0, 10, 2, 2);
                Vector4 jevelryCost = new(0, 10, 3, 2);

                return index switch
                {
                    // Steel
                    0 => boltCost,

                    // Bronze
                    1 => pipeCost,

                    // Aluminum
                    2 => frameCost,

                    // Gold
                    3 => jevelryCost,

                    _ => Vector4.zero,
                };
            }

            else return Vector4.zero;
        }


        /// <summary>
        /// ¬озвращает стоимость ресурса в валюте
        /// </summary>
        /// <param name="tag">“ип ресурса (Ore, Ingot, Product)</param>
        /// <param name="index">»ндекс ресурса заданного типа в системе хранени€ ресурсов</param>
        /// <returns></returns>
        public static float GetCostOfResource(string tag, int index)
        {
            return tag switch
            {
                "Ore" => oreCost[index],
                "Ingot" => ingotCost[index],
                "Product" => productCost[index],
                _ => 0f,
            };
        }
    }
}

