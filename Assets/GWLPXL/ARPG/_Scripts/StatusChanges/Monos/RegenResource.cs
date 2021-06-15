

using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Statics.com;
using UnityEngine;

namespace GWLPXL.ARPGCore.StatusEffects.com
{
 
    /// <summary>
    /// attach to actor root to give a regen effect
    /// </summary>
    public class RegenResource : MonoBehaviour
    {
        public ModifyResourceVars Vars;
        IActorHub user = null;
        private void Awake()
        {
            user = GetComponent<IActorHub>();
        }

        private void Start()
        {
            if (user != null && user.MyStatusEffects != null)
            {
                SoTHelper.AddDoT(user, Vars);
                //user.MyStatusEffects.AddDoT(Vars);
            }
      
        }

    }
}