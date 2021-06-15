
using GWLPXL.ARPGCore.com;
using GWLPXL.ARPGCore.Items.com;
using GWLPXL.ARPGCore.Leveling.com;
using GWLPXL.ARPGCore.Statics.com;
using UnityEngine;

namespace GWLPXL.ARPGCore.Looting.com
{
    public class EnemyDrop : MonoBehaviour, IDropLoot
    {
        [SerializeField]
        EnemyDropLootEvents dropLootEvents = new EnemyDropLootEvents();
        [SerializeField]
        LootDrops possibleDrops;
        [SerializeField]
        float delay = .25f;
        IScale scale = null;

        private void Awake()
        {
           scale  = GetComponent<IScale>();
        }
        public void DropLoot()
        {
            if (possibleDrops == null || possibleDrops.AllPossibleItems.Count == 0) return;

            int ofLevel = 1;
            if (scale != null)
            {
                ofLevel = scale.GetScaledLevel();
            }
            int level = Formulas.GetILevelMulti(ofLevel);
            Item item = possibleDrops.GetRandomDrop(level);
            ILoot newLoot = LootHandler.DropLoot(item, transform.position, DungeonMaster.Instance.GetLootPrefab(), delay, Random.insideUnitSphere);
            if (dropLootEvents != null)
            {
                dropLootEvents.SceneEvents.OnLootDropped.Invoke(newLoot);
            }
        }

        public void SetLootDrop(LootDrops newDrops)
        {
            possibleDrops = newDrops;
        }
    }
}