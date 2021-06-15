using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Types.com;
using UnityEngine;

namespace GWLPXL.ARPGCore.Auras.com
{
    /// <summary>
    /// Defines what can take an aura.
    ///use this to link your auras to your other systems, extend according to your own system.
    /// </summary>
    public interface ITakeAura
    {
        #region required, these shouldnt be removed or changed
        GameObject GetGameObjectInstance();
        AuraTargetGroup[] GetAuraGroups();
        void SetActorHub(IActorHub newHub);
        #endregion

        #region modify and extend, these can be removed/changed 

        void AuraModifyMaxResource(int resourceType, int byAmount);
        void AuraModifyCurrentResource(int resourceType, int byAmount);
        void AuraBuffSat(int statType, int byAmount);
        #endregion
    }
}