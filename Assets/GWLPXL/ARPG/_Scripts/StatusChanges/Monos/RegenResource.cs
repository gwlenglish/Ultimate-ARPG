

using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Statics.com;
using UnityEngine;

namespace GWLPXL.ARPGCore.StatusEffects.com
{
 
    /// <summary>
    /// attach to object with iactorhub to give a regen effect
    /// </summary>
    public class RegenResource : MonoBehaviour
    {
        public ModifyResourceVars Vars;
        protected IActorHub user = null;
        protected virtual void Awake()
        {
            user = GetComponent<IActorHub>();
        }

        protected virtual void Start()
        {
            if (user != null && user.MyStatusEffects != null)
            {
                SoTHelper.AddDoT(user, Vars);
            }
      
        }

    }
}