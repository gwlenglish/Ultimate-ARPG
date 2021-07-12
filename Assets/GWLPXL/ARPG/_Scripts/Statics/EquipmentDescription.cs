using GWLPXL.ARPGCore.Items.com;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;

namespace GWLPXL.ARPGCore.Statics.com
{

    /// <summary>
    /// to do, need to know when it's enchanted or not. need to know if no more sockets or not.
    /// </summary>
    public static class EquipmentDescription
    {
        static StringBuilder sb = new StringBuilder();

        public static void RenameItemWithSocket(Equipment equipment, AffixReaderSO AffixReaderSO)
        {
            sb.Clear();
            List<string> socketaffixes = equipment.GetStats().GetAllSocketAffixes();
            string newname = equipment.GetStats().GetDroppedName();
            if (socketaffixes.Count > 0)//we have none
            {
                //get existing
                string key = equipment.GetBaseItemName().ToLower();
                List<string> entire = AffixHelper.SplitPhrase(equipment.GetGeneratedItemName());
                List<string> front = new List<string>();
                List<string> back = new List<string>();
                bool firsthalf = true;
                for (int i = 0; i < entire.Count; i++)
                {
                    if (AffixHelper.WordEquals(entire[i].ToLower(), key))
                    {
                        //found it

                        firsthalf = false;
                        continue;
                    }

                    if (firsthalf)
                    {
                        front.Add(entire[i]);
                    }
                    else
                    {
                        back.Add(entire[i]);
                    }
                }
                //
                List<string> socketprefixes = equipment.GetStats().GetAllSocketPrefixes();
                if (front.Count > 0)
                {

                    front.Concat(socketprefixes);
                    string prefixtoname = AffixReaderSO.GetNameWithAffixesPreLoaded(front, equipment.GetBaseItemName(), 0);
                    sb.Append(prefixtoname);
                }
                else
                {
                    //try create one
                    string prefixtoname = AffixReaderSO.GetNameWithAffixesPreLoaded(socketprefixes, equipment.GetBaseItemName(), 0);
                    sb.Append(prefixtoname);
                }

                List<string> socketpostprefixes = equipment.GetStats().GetAllSocketSuffixes();
                if (back.Count > 0)
                {
                    string postnoun = AffixReaderSO.GetPostNoun(back);

                    back.Concat(socketpostprefixes);
                    if (string.IsNullOrEmpty(postnoun) == false)
                    {
                        string of = " of ";
                        string postnounwithaffixes = AffixReaderSO.GetNameWithAffixesPreLoaded(back, postnoun, 0);
                        sb.Append(of);
                        sb.Append(postnounwithaffixes);
                    }

                }
                else
                {
                    //try create one
                    List<string> nouns = equipment.GetStats().GetAllNounsSockets();
                    string newpostnoun = AffixReaderSO.GetPostNoun(nouns);
                    if (string.IsNullOrEmpty(newpostnoun) == false)
                    {
                        string of = " of ";
                        string postnounwithaffixes = AffixReaderSO.GetNameWithAffixesPreLoaded(socketpostprefixes, newpostnoun, 0);
                        sb.Append(of);
                        sb.Append(postnounwithaffixes);
                    }
                }


                equipment.SetGeneratedItemName(sb.ToString());
            }
            else
            {
                equipment.SetGeneratedItemName(newname);
            }



        }
        public static void RenameItemWithEnchant(Equipment equipment, AffixReaderSO AffixReaderSO)
        {
            sb.Clear();
            List<string> affixes = equipment.GetStats().GetAllTraitAffixes();
            string newname = equipment.GetStats().GetDroppedName();
            if (affixes.Count > 0)//we have none
            {
                //get existing
                string key = equipment.GetBaseItemName().ToLower();
                List<string> entire = AffixHelper.SplitPhrase(equipment.GetGeneratedItemName());
                List<string> front = new List<string>();
                List<string> back = new List<string>();
                bool firsthalf = true;
                for (int i = 0; i < entire.Count; i++)
                {
                    if (AffixHelper.WordEquals(entire[i].ToLower(), key))
                    {
                        //found it

                        firsthalf = false;
                        continue;
                    }

                    if (firsthalf)
                    {
                        front.Add(entire[i]);
                    }
                    else
                    {
                        back.Add(entire[i]);
                    }
                }
                //
                List<string> socketprefixes = equipment.GetStats().GetAllTraitPrefixes();//
                if (front.Count > 0)
                {

                    front.Concat(socketprefixes);
                    string prefixtoname = AffixReaderSO.GetNameWithAffixesPreLoaded(front, equipment.GetBaseItemName(), 0);
                    sb.Append(prefixtoname);
                }
                else
                {
                    //try create one
                    string prefixtoname = AffixReaderSO.GetNameWithAffixesPreLoaded(socketprefixes, equipment.GetBaseItemName(), 0);
                    sb.Append(prefixtoname);
                }

                List<string> socketpostprefixes = equipment.GetStats().GetAllTraitSuffixes();//
                if (back.Count > 0)
                {
                    string postnoun = AffixReaderSO.GetPostNoun(back);

                    back.Concat(socketpostprefixes);
                    if (string.IsNullOrEmpty(postnoun) == false)
                    {
                        string of = " of ";
                        string postnounwithaffixes = AffixReaderSO.GetNameWithAffixesPreLoaded(back, postnoun, 0);
                        sb.Append(of);
                        sb.Append(postnounwithaffixes);
                    }

                }
                else
                {
                    //try create one
                    List<string> nouns = equipment.GetStats().GetAllTraitNouns();//
                    string newpostnoun = AffixReaderSO.GetPostNoun(nouns);
                    if (string.IsNullOrEmpty(newpostnoun) == false)
                    {
                        string of = " of ";
                        string postnounwithaffixes = AffixReaderSO.GetNameWithAffixesPreLoaded(socketpostprefixes, newpostnoun, 0);
                        sb.Append(of);
                        sb.Append(postnounwithaffixes);
                    }
                }


                equipment.SetGeneratedItemName(sb.ToString());
            }
            else
            {
                equipment.SetGeneratedItemName(newname);
            }



        }

        public static void GetEquipmentInfo(Equipment equipment)
        {
        }
    }
}