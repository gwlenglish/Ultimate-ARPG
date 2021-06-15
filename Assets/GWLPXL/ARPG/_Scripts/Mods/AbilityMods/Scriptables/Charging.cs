using GWLPXL.ARPGCore.Abilities.com;
using GWLPXL.ARPGCore.Abilities.Mods.com;
using GWLPXL.ARPGCore.com;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GWLPXL.ARPGCore.Abilities.Mods.com
{

    /// <summary>
    /// performs charging logic for the next ability to lead into
    /// </summary>
    [CreateAssetMenu(menuName = "GWLPXL/ARPG/Abilities/Mods/New_Charging")]

    public class Charging : AbilityLogic
    {

        public Ability EndChargeAbility;
        [System.NonSerialized]
        Dictionary<IActorHub, ChargeTimer> chargedic = new Dictionary<IActorHub, ChargeTimer>();
        [System.NonSerialized]
        Dictionary<Ability, IActorHub> nextdic = new Dictionary<Ability, IActorHub>();
        public override bool CheckLogicPreRequisites(IActorHub forUser)
        {
            return true;
        }

        public override void EndCastLogic(IActorHub skillUser, Ability theSkill)
        {
            if (EndChargeAbility == null) return;
            skillUser.MyAbilities.SetChargedAbility(EndChargeAbility);//tells the user that the ability uses the charge.
        }

        void Next(IActorHub hub, Ability ability)//working on the timing
        {
            if (nextdic.ContainsKey(ability))
            {
                AbilityDurationTimer duration = hub.MyAbilities.GetRuntimeController().GetTimer(ability);
                if (duration != null)
                {
                    duration.RemoveTicker();
                }
                if (EndChargeAbility != null)
                {
                    bool success = hub.MyAbilities.TryCastAbility(EndChargeAbility);
                    Debug.Log("Charge success: " + success);//returning false, probably happening to fast.
                }
                chargedic.Remove(hub);
                nextdic.Remove(ability);
                hub.MyAbilities.GetRuntimeController().OnAbilityUserEnd -= Next;
            }
        }


        public override void StartCastLogic(IActorHub skillUser, Ability theSkill)
        {
            if (chargedic.ContainsKey(skillUser) == false)
            {
                Debug.Log("Cast Success");
                theSkill.CoolDownRate = 0;//charging should not have any cooldown rates.
                AbilityDurationTimer duration = skillUser.MyAbilities.GetRuntimeController().GetTimer(theSkill);
                Debug.Log("TIMER " + duration);
                ChargeTimer timer = new ChargeTimer(skillUser, duration, theSkill);
                chargedic.Add(skillUser, timer);
                nextdic.Add(theSkill, skillUser);
                skillUser.MyAbilities.GetRuntimeController().OnAbilityUserEnd += Next;

            }
        }


    }
}
public class ChargeTimer : ITick
{
    IActorHub user;
    AbilityDurationTimer timer;
    Ability ability;
    float chargetimer = 0;
    public ChargeTimer(IActorHub user, AbilityDurationTimer duration, Ability ability)
    {
        this.ability = ability;
        this.user = user;
        this.timer = duration;
        timer.Cooldown.Pause = true;
        user.MyAbilities.GetRuntimeController().SetChargeAmount(0);
        AddTicker();
    }

    public void AddTicker()
    {
        TickManager.Instance.AddTicker(this);
    }

    public void DoTick()
    {
        bool held = user.InputHub.AbilityInputs.GetAbilityInput(ability);
        Debug.Log("Holding " + held);
        chargetimer += GetTickDuration();
        float percent = chargetimer / ability.Duration;//using duration as the max charge length
        percent = Mathf.Clamp01(percent);//clamping to max, so can't over charge. May want to reconsider this later
        user.MyAbilities.GetRuntimeController().SetChargeAmount(percent);
        if (held == ability)
        {

            //holding
            timer.Cooldown.Pause = true;
           
        }
        else
        {


            RemoveTicker();
        }
    }

    public float GetTickDuration()
    {
        return Time.deltaTime;
    }

    public void RemoveTicker()
    {
        timer.Cooldown.Pause = false;
        user.MyAbilities.GetRuntimeController().InterruptAbility(ability);
        TickManager.Instance.RemoveTicker(this);
    }
}
