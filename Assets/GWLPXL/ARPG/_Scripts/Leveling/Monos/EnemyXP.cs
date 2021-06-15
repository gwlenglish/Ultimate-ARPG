
using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Saving.com;
using GWLPXL.ARPGCore.Statics.com;
using UnityEngine;

namespace GWLPXL.ARPGCore.Leveling.com
{
    public class EnemyXP : MonoBehaviour, IGiveXP
    {
        public int BaseXP = 40;
        public bool UseScaledAmount = true;
        public void GiveXP()
        {
            PlayerSceneReference[] players = DungeonMaster.Instance.GetAllSceneReferences();
            IScale scaler = GetComponent<IScale>();
            for (int i = 0; i < players.Length; i++)
            {
                ILevel leveler = players[i].SceneRef.GetComponent<IActorHub>().Level;
                if (leveler != null)
                {
                    int scaled = BaseXP;
                    if (UseScaledAmount == true)
                    {
                        scaled = Formulas.GetEnemyXP(scaled);
                    }
                    leveler.EarnXP(scaled);

                }
            }
        }
    }
}