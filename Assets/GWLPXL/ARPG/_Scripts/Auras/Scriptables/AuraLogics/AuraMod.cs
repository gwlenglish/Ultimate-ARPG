using GWLPXL.NoFrills.Modifiers.com;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ARPGCore.Auras.com
{

    [CreateAssetMenu(menuName = "GWLPXL/ARPG/Auras/Logic/NEW Aura Mod")]
    public class AuraMod : AuraLogic
    {
        public List<BaseModHolder> Mods = new List<BaseModHolder>();
        public override bool DoApplyLogic(ITakeAura onUser)
        {
            onUser.GetActorHub().MyStats.GetRuntimeAttributes().AddMods(Mods);
            return true;
        }

        public override bool DoRemoveLogic(ITakeAura fromUser)
        {
            fromUser.GetActorHub().MyStats.GetRuntimeAttributes().RemoveMods(Mods);
            return true;
        }

       
    }
}