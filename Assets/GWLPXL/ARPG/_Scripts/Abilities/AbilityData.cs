using GWLPXL.ARPGCore.Abilities.Mods.com;
using GWLPXL.ARPGCore.Types.com;
using UnityEngine;

namespace GWLPXL.ARPGCore.Abilities.com
{
    [System.Serializable]
    public class AbilityData
    {
        public string Name = string.Empty;
        [TextArea(3, 5)]
        public string Description = string.Empty;
        public float DamageMultiplier = 1;
        public float Range = 1;
        public float ResourceCost = 0;
        public ResourceType ResourceType = ResourceType.Mana;
        public string AnimationTrigger = "Ability";
        public int AnimationIndex = 0;
        public AbilityLogic[] Logics = new AbilityLogic[0];
        public int UniqueID = 0;
        public Sprite Sprite = null;

    }
}