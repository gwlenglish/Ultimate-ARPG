


using GWLPXL.ARPGCore.Abilities.com;

using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Statics.com;
using GWLPXL.ARPGCore.StatusEffects.com;
using UnityEngine;

namespace GWLPXL.ARPGCore.Abilities.Mods.com
{

    /// <summary>
    /// Instant AOE drain effect, using physics overlapsphere or physics2d circle
    /// </summary>
    [CreateAssetMenu(menuName = "GWLPXL/ARPG/Abilities/Mods/NEW_Drain_AOE")]

    public class InstantDrain_AOE : AbilityLogic
    {
        public AoEVars AoE_Vars;
        public ModifyResourceVars DoT_Vars;



        public override bool CheckLogicPreRequisites(IActorHub forUser)
        {
            return true;
        }

        public override void EndCastLogic(IActorHub skillUser, Ability theSkill)
        {
        }


        public override void StartCastLogic(IActorHub skillUser, Ability theSkill)
        {
            CombatHelper.DoAoEDoT(skillUser, skillUser.MyTransform.position, AoE_Vars, DoT_Vars);

        }
    }
}