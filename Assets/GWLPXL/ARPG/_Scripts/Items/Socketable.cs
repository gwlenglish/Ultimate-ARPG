using GWLPXL.ARPGCore.Types.com;

namespace GWLPXL.ARPGCore.Items.com
{

    public abstract class Socketable : Item
    {
        public SocketableVars Vars;

        public override string GetBaseItemName()
        {
            return Vars.BaseName;
        }

        public override string GetGeneratedItemName()
        {
            return Vars.GeneratedName;
        }

        public override ItemType GetItemType()
        {
            return ItemType.Socketable;
        }

        public override int GetStackingAmount()
        {
            return Vars.StackingAmount;
        }


        public override bool IsStacking()
        {
            return Vars.StackingAmount > 1;
        }

        public override void SetGeneratedItemName(string newName)
        {
            Vars.GeneratedName = newName;
        }
    }

    [System.Serializable]
    public class SocketableVars
    {
        public string BaseName = string.Empty;
        public int StackingAmount = 5;
        public string GeneratedName = string.Empty;

        public SocketableVars(string basename, int stacking = 1)
        {
            BaseName = basename;
            StackingAmount = stacking;
        }
    }
}