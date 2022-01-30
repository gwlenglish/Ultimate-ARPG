using GWLPXL.ARPGCore.Statics.com;
using System.Collections.Generic;
using System;
namespace GWLPXL.ARPGCore.com
{
    public class AttackValues : EventArgs
    {
        public IActorHub Attacker = null;
        public List<IActorHub> Defenders;
        public bool Resolved = false;
        public List<ElementAttackResults> ElementAttacks;
        public List<PhysicalAttackResults> PhysicalAttack;
        public AttackValues(IActorHub attacker, IActorHub defender)
        {
            Defenders = new List<IActorHub>();
            Defenders.Add(defender);
            Attacker = attacker;
            ElementAttacks = new List<ElementAttackResults>();
            PhysicalAttack = new List<PhysicalAttackResults>();
        }

        public virtual void Resolve()
        {
            for (int i = 0; i < Defenders.Count; i++)
            {
                Defenders[i].MyHealth.TakeDamage(this);
            }
            Resolved = true;
        }
    }
}
