using GWLPXL.ARPGCore.Abilities.Mods.com;
using GWLPXL.ARPGCore.com;

using UnityEngine;

namespace GWLPXL.ARPGCore.Combat.com
{

    /// <summary>
    ///  Mods the weapon to have an Explosive Effect
    /// </summary>
    public class ExplosiveMod : MonoBehaviour, IWeaponModification
    {
        public ExplosionVars Vars;
        bool isactive = false;
        IActorHub user = null;
        public bool DoChange(Transform other)
        {
            return false;
        }

        public void DoModification(IActorHub other)
        {
           
            GameObject instance = Instantiate(Vars.ExplosionPrefab, other.MyTransform.position, Quaternion.identity);
            ActorExplosion explode = instance.GetComponent<ActorExplosion>();
            ExplosionVars newvars = new ExplosionVars(Vars.ActorDamage, user);
            newvars.Delay = Vars.Delay;
            newvars.Duration = Vars.Duration;
            newvars.EndRadius = Vars.EndRadius;
            newvars.ExplosionCurve = Vars.ExplosionCurve;
            newvars.StickToTarget = Vars.StickToTarget;
            explode.Vars = newvars;

            explode.gameObject.transform.position = other.MyTransform.position;
            if (Vars.StickToTarget)
            {
                explode.gameObject.transform.SetParent(other.MyTransform);
            }
            
        }

        public Transform GetTransform() => this.transform;


        public bool IsActive() => isactive;


        public void SetActive(bool isEnabled) => isactive = isEnabled;


        public void SetUser(IActorHub myself) => user = myself;
      

       
    }
}