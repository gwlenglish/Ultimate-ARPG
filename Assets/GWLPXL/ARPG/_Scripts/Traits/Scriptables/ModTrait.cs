using GWLPXL.ARPGCore.Attributes.com;
using GWLPXL.ARPGCore.Types.com;
using GWLPXL.NoFrills.Modifiers.com;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GWLPXL.ARPGCore.Traits.com
{

    [CreateAssetMenu(menuName = "GWLPXL/ARPG/Equipment/Traits/NEW_ModTrait")]
    public class ModTrait : EquipmentTrait
    {
        public List<BaseModHolder> Mods = new List<BaseModHolder>();
        public override void ApplyTrait(IAttributeUser toActor)
        {
            toActor.GetRuntimeAttributes().AddMods(Mods);
        }

        public override TraitType GetTraitType()
        {
            return TraitType.ModTrait;
        }

        public override void RemoveTrait(IAttributeUser toActor)
        {
            toActor.GetRuntimeAttributes().RemoveMods(Mods);
        }
    }
}