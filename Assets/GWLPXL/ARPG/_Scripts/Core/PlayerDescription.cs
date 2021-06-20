
using GWLPXL.ARPGCore.Abilities.com;
using GWLPXL.ARPGCore.Attributes.com;
using GWLPXL.ARPGCore.Items.com;
using GWLPXL.ARPGCore.Traits.com;
using GWLPXL.ARPGCore.Types.com;
using System.Text;
using UnityEngine;

namespace GWLPXL.ARPGCore.Statics.com
{
    /// <summary>
    /// used for consistent naming of elements for the player, to do turn into scriptable for easy override, make functions virtual
    /// </summary>
    //
    public static class PlayerDescription
    {
        const string enter = "\n";
        #region old code
        //words
        const string resist = " resist ";
        const string attack = " attack ";
        const string percent = "%";
        const string positive = "+";
        const string negative = "-";
        const string increase = "Increase ";
        const string decrease = "Decrease ";
        const string by = " by ";
        const string space = " ";

        //element descriptions
        public const string Cold = "Cold";
        public const string Fire = "Fire";
        public const string Lightning = "Lightning";

        //resource descriptions
        public const string Health = "Health";
        public const string Mana = "Mana";

        //stat descriptions
        public const string Dexterity = "Dexterity";
        public const string Strength = "Strength";
        public const string Vitality = "Vitality";
        public const string Intelligence = "Intelligence";

        //equipment
        public const string Weapon = "Weapon: ";
        public const string Accessory = "Accessory: ";
        public const string Armor = "Armor: : ";

        //natives go before the base name, so Icey Cutlass of X
        #region moved to SO's, this is old backups
        public static string[] ColdPrefix = new string[] { "Icey", "Frozen", "Chilling" };
        public static string[] FirePrefix = new string[] { "Fiery", "Blazing" };
        public static string[] LightningPrefix = new string[] { "Electric", "Zapping" };

        public static string[] ColdSuffix = new string[] { "of Iciness", "of Ice", "of the Tundra" };
        public static string[] FireSuffix = new string[] { "of Flame", "of Fire", "of Hellfire" };
        public static string[] LightningSuffix = new string[] { "of Lightning", "of Spark" };

        public static string[] HealthPrefix = new string[] { "Healthy", "Vital", "Invigorating" };
        public static string[] ManaPrefix = new string[] { "Magical", "Refreshing", "Mystical" };

        public static string[] HealthSuffix = new string[] { "of Vitality", "of Invigoration", "of Nourishment" };
        public static string[] ManaSuffix = new string[] { "of Mysticism", "of Mana" };

        public static string[] StrengthPrefix = new string[] { "Strong", "Poewrful", "Solid" };
        public static string[] DexterityPrefix = new string[] { "Agile", "Nimble" };
        public static string[] VitalityPrefix = new string[] { "Healthy", "Vital", "Invigorating" };
        public static string[] IntelligencePrefix = new string[] { "Brilliant", "Clever" };

        public static string[] StrengthSuffix = new string[] { "of Strength", "of Power" };
        public static string[] DexteritySuffix = new string[] { "of Dexterity", "of Nimbleness" };
        public static string[] VitalitySuffix = new string[] { "of Vitality", "of Health" };
        public static string[] IntelligenceSuffix = new string[] { "of Intelligence", "of Brilliance" };
        #endregion
        #endregion

        static StringBuilder sb = new StringBuilder("");
        public static string GetRandomTraitDescriptor(string[] possibleNames)
        {
            int rando = Random.Range(0, possibleNames.Length);
            return possibleNames[rando];
        }
        public static string GetGeneratedName(Equipment equipment)//could refactor out to put the names on the SO's themselves...
        {
            sb.Clear();


            EquipmentTrait[] NativeTraits = equipment.GetStats().GetNativeTraits();
            if (NativeTraits.Length > 0)
            {
                sb.Append(NativeTraits[0].GetTraitPrefix());
                //we use the first native one for the prefix
                sb.Append(space);
            }


            sb.Append(equipment.GetStats().GetBaseName());


            EquipmentTrait[] randomTraits = equipment.GetStats().GetRandomTraits();
            if (randomTraits.Length > 0)
            {
                sb.Append(space);
                sb.Append(randomTraits[0].GetTraitSuffix());
                //we use the first random one for the suffix
            }

            //ARPGDebugger.DebugMessage(sb.ToString());
            equipment.SetGeneratedItemName(sb.ToString());
            return sb.ToString();

        }

        //convert to SB
        public static string GetCharacterInfoDescription(IAttributeUser stats, IInventoryUser inv, IAbilityUser abilities)
        {


            sb.Clear();

            //overall attack and armor
            sb.Append("Attack Value: " + CombatStats.GetTotalPlayerAttackDamage(stats, inv, abilities).ToString() + enter);
            sb.Append("Armor Value: " + CombatStats.GetPlayerArmorAmount(stats, inv).ToString() + enter);

            //stat values
            Attribute[] attackElements = stats.GetRuntimeAttributes().GetAttributes(AttributeType.Stat);
            for (int i = 0; i < attackElements.Length; i++)
            {
                Stat stat = (Stat)attackElements[i];
                sb.Append(stat.GetFullDescription() + enter);
            }

            Attribute[] resources = stats.GetRuntimeAttributes().GetAttributes(AttributeType.Resource);
            for (int i = 0; i < resources.Length; i++)
            {
                Resource resource = (Resource)resources[i];
                sb.Append(resource.GetFullDescription() + enter);
            }

            Attribute[] eleAttacks = stats.GetRuntimeAttributes().GetAttributes(AttributeType.ElementAttack);
            for (int i = 0; i < eleAttacks.Length; i++)
            {
                ElementAttack eleAttack = (ElementAttack)eleAttacks[i];
                if (eleAttack.Type == ElementType.None) continue;//skip it so it doesn't add itself to the description
                sb.Append(eleAttack.GetFullDescription() + enter);//for some reason not formatting the same as resist

            }

            Attribute[] eleResist = stats.GetRuntimeAttributes().GetAttributes(AttributeType.ElementResist);
            for (int i = 0; i < eleResist.Length; i++)
            {
                ElementResist resist = (ElementResist)eleResist[i];
                if (resist.Type == ElementType.None) continue;//skip it so it doesn't add itself to the description
                sb.Append(resist.GetFullDescription() + enter);


            }

            //format the other stuff
            Attribute[] other = stats.GetRuntimeAttributes().GetAttributes(AttributeType.Other);
            for (int i = 0; i < other.Length; i++)
            {
                Other otheratt = (Other)other[i];
                sb.Append(otheratt.GetFullDescription() + enter);
            }


            return sb.ToString();
        }












    }
}