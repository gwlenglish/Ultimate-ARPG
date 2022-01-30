using GWLPXL.ARPGCore.Types.com;

using UnityEngine;
using GWLPXL.ARPGCore.DebugHelpers.com;
using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Statics.com;

namespace GWLPXL.ARPGCore.StatusEffects.com
{
    #region helper

    [System.Serializable]
    public class ModifyResourceVars
    {
        public ResourceType Type = ResourceType.Health;
        public ElementType ElementDamage = ElementType.None;
        public StatusEffectSO[] StatusEffects = new StatusEffectSO[0];
        [Tooltip("Use Negative for damage, positive for regen")]
        public int AmountPerTick = 1;
        [Tooltip("If 0 or less, will tick forever")]
        public float Duration = 1f;
        [Tooltip("When enabled, will clamp the status effect duration to this duration.")]
        public bool ClampStatusEffectToDuration = false;
        public float TickRate = 1f;
        [Tooltip("How many max stacks at once. AmountPerTick * Stack = total.")]
        [Range(1, 5)]
        public int StackAmount = 1;

        public ModifyResourceVars(ResourceType _type, int _amountPerTick, float tickRate, float _duration, int _maxStack)
        {
            Type = _type;
            AmountPerTick = _amountPerTick;
            Duration = _duration;
            TickRate = tickRate;
            StackAmount = _maxStack;

        }
     
    }

    /// <summary>
    /// rethink the system, now we are applying mulitple times
    /// </summary>
    public class ModifyResourceDoTState : IDoT, ITick
    {
        public float CurrentTimer => timer;
        protected ModifyResourceVars vars = null;
        protected IActorHub target = null;
        protected int currentStacks = 0;
        protected bool applied = false;
        protected float timer = 0;
        public ModifyResourceDoTState(IActorHub target, ModifyResourceVars vars)
        {
            this.target = target;
            this.vars = vars;
            AddTicker();
        }

        public virtual void AddTicker()
        {
            TickManager.Instance.AddTicker(this);
        }

        public virtual  void ApplyDoT()
        {
            timer = 0;
            currentStacks += 1;
            if (currentStacks > vars.StackAmount)
            {
                currentStacks = vars.StackAmount;
            }
        
            applied = true;
            StatusEffectHelper.ApplyStatusEffects(target, vars.StatusEffects);
        }

        public virtual void DoTick()
        {
            if (applied == false) return;
            Tick();
        }


      

        public virtual float GetTickDuration() => vars.TickRate;

      

        public virtual void RemoveDoT()
        {
            applied = false;
            RemoveTicker();
            SoTHelper.RemoveDot(target, vars);
            //target.MyStatusEffects.RemoveDot(vars);
            if (vars.ClampStatusEffectToDuration)
            {
                StatusEffectHelper.RemoveStatusEffects(target, vars.StatusEffects);
            }

        }

        public virtual void RemoveTicker()
        {
            TickManager.Instance.RemoveTicker(this);
        }

        public virtual  void Tick()
        {
        
            if (timer >= vars.Duration && vars.Duration > 0)
            {
                RemoveDoT();
                return;
            }
            if (applied == false) return;

            if (vars.AmountPerTick < 0)
            {
                SoTHelper.ReduceResource(target, vars.AmountPerTick * currentStacks, vars.Type, vars.ElementDamage);
                //target.MyStatusEffects.ReduceResource(vars.AmountPerTick * currentStacks, vars.Type, vars.ElementDamage);
            }
            else if (vars.AmountPerTick > 0)
            {
                SoTHelper.RegenResource(target, vars.AmountPerTick * currentStacks, vars.Type, vars.ElementDamage);

                //target.MyStatusEffects.RegenResource(vars.AmountPerTick * currentStacks, vars.Type, vars.ElementDamage);
            }

            timer += vars.TickRate;
             //ARPGDebugger.DebugMessage(ARPGDebugger.GetColorForSOTs("Modify Resource Duration " + timer), null);
              //ARPGDebugger.DebugMessage(ARPGDebugger.GetColorForSOTs("Modify Resource Stack Amount: " + currentStacks), null);
        }
    }

    #endregion
}