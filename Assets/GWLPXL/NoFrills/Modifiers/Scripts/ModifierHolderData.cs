using UnityEngine;

namespace GWLPXL.NoFrills.Modifiers.com
{
    /// <summary>
    /// basic info
    /// </summary>
    [System.Serializable]
    public class ModifierHolderData
    {
        public bool AutoName = false;
        public bool AutoAssignUniqueID = false;
        public string Name = string.Empty;
        [TextArea(3, 5)]
        public string Description = string.Empty;
        public int UniqueID = 0;
    }
}