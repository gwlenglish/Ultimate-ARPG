using System.Collections;
using UnityEngine;

namespace GWLPXL.InventoryGrid
{


    /// <summary>
    /// holds the pattern class to make scriptables out of them
    /// </summary>
    [CreateAssetMenu(menuName = "Test Pattern")]
    public class PatternHolder : ScriptableObject
    {
        public Pattern Pattern;
    }
}