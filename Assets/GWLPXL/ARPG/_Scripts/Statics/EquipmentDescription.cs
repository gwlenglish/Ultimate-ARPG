﻿using GWLPXL.ARPGCore.Items.com;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;
using GWLPXL.ARPGCore.DebugHelpers.com;
namespace GWLPXL.ARPGCore.Statics.com
{

    /// <summary>
    /// to do, need to know when it's enchanted or not. need to know if no more sockets or not.
    /// </summary>
    public static class EquipmentDescription
    {
        static StringBuilder sb = new StringBuilder();
        public static void RenameItemWithSocketRemoved(Equipment equipment, EquipmentSocketable removed, RenameType type, AffixOverrides overrides)
        {
            sb.Clear();
           
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
            Debug.Log(sb.ToString());
            switch (type)
            {
                case RenameType.Prefix:

                    //remove from prefix
                    sb = RemoveSocketPrefix(removed, overrides, front);

                    sb.Append(equipment.GetBaseItemName());

                    for (int i = 0; i < back.Count; i++)
                    {
                        sb.Append(" ");
                        sb.Append(back[i]);
                    }
                    break;
                case RenameType.Suffix:
                    for (int i = 0; i < front.Count; i++)
                    {
                        sb.Append(front[i]);
                        sb.Append(" ");
                    }
                    sb.Append(equipment.GetBaseItemName());
                    sb = RemoveSocketSuffix(removed, overrides, back);
                    break;
                case RenameType.Both:
                    sb = RemoveSocketPrefix(removed, overrides, front);
                    sb.Append(equipment.GetBaseItemName());
                    sb = RemoveSocketSuffix(removed, overrides, back);
                    break;
            }

            equipment.SetGeneratedItemName(sb.ToString());
            Debug.Log(sb.ToString());
        }

        private static StringBuilder RemoveSocketSuffix(EquipmentSocketable removed, AffixOverrides overrides, List<string> back)
        {
            List<string> suffixes;
            if (overrides != null && overrides.Suffixes.Count > 0)
            {
                suffixes = overrides.Suffixes;
            }
            else
            {
                suffixes = removed.EquipmentTraitSocketable.GetSuffixes().ToList();
            }

            for (int i = 0; i < back.Count; i++)
            {
                for (int j = 0; j < suffixes.Count; j++)
                {
                    string removekey = suffixes[j].ToLower();
                    if (AffixHelper.WordEquals(back[i].ToLower(), removekey) == false)
                    {
                        sb.Append(back[i]);//reconstruct, skipping any that we no longer have
                    }
                }

            }
            return sb;
        }
        private static StringBuilder RemoveSocketPrefix(EquipmentSocketable removed, AffixOverrides overrides, List<string> front)
        {
            List<string> prefixes;
            if (overrides != null && overrides.Prefixes.Count > 0)
            {
                prefixes = overrides.Prefixes;
            }
            else
            {
                prefixes = removed.EquipmentTraitSocketable.GetPrefixes().ToList();
            }
            List<string> remove = new List<string>();
            for (int i = 0; i < front.Count; i++)
            {
                string currentword = front[i];
                bool add = true;
                for (int j = 0; j < prefixes.Count; j++)
                {
                    string removekey = prefixes[j];
                    Debug.Log(removekey + " , " + currentword);
                    if (AffixHelper.WordEquals(currentword, removekey))
                    {
                        add = false;
                        break;
                    }
                 
                }

                if (add)
                {
                    sb.Append(front[i]);//reconstruct, skipping any that we no longer have
                    sb.Append(" ");
                }

            }
            return sb;
        }

        public static void RenameItemWithSocket(Equipment equipment, AffixReaderSO AffixReaderSO, RenameType type, AffixOverrides overrides)
        {
            sb.Clear();
            List<string> socketaffixes = equipment.GetStats().GetAllSocketAffixes();
            if (socketaffixes.Count == 0 && overrides != null)
            {
                socketaffixes.Concat(overrides.Prefixes);
                socketaffixes.Concat(overrides.Suffixes);
            }

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

                switch (type)
                {
                    case RenameType.Prefix:
                        sb = DetermineFrontPrefix(equipment, overrides, AffixReaderSO, front);
                        for (int i = 0; i < back.Count; i++)
                        {
                            sb.Append(" ");
                            sb.Append(back[i]);
                        }
                        break;
                    case RenameType.Suffix:
                        for (int i = 0; i < front.Count; i++)
                        {
                            sb.Append(front[i]);
                            sb.Append(" ");
                        }
                        sb =  DetermineBackSuffix(equipment, overrides, AffixReaderSO, back);
                        break;
                    case RenameType.Both:
                        sb = DetermineFrontPrefix(equipment, overrides, AffixReaderSO, front);
                        sb = DetermineBackSuffix(equipment, overrides, AffixReaderSO, back);
                        break;
                }

            

                equipment.SetGeneratedItemName(sb.ToString());
            }
            else
            {
               // equipment.SetGeneratedItemName(newname);
            }



        }

        private static StringBuilder DetermineBackSuffix(Equipment equipment, AffixOverrides overrides, AffixReaderSO AffixReaderSO, List<string> back)
        {
            List<string> socketpostprefixes;
            if (overrides != null && overrides.Suffixes.Count > 0)
            {
                socketpostprefixes = overrides.Suffixes;
            }
            else
            {
                socketpostprefixes = equipment.GetStats().GetAllSocketSuffixes();

            }
            if (back.Count > 0)
            {
                string postnoun;
                if (overrides != null && overrides.Nouns.Count > 0)
                {
                    postnoun = AffixReaderSO.GetPostNoun(overrides.Nouns);
                }
                else
                {
                    postnoun = AffixReaderSO.GetPostNoun(back);
                }
            

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

                List<string> nouns;
                if (overrides != null && overrides.Nouns.Count > 0)
                {
                    nouns = overrides.Nouns;
                }
                else
                {
                    nouns = equipment.GetStats().GetAllNounsSockets();
                }


                string newpostnoun = AffixReaderSO.GetPostNoun(nouns);
                if (string.IsNullOrEmpty(newpostnoun) == false)
                {
                    string of = " of ";
                    string postnounwithaffixes = AffixReaderSO.GetNameWithAffixesPreLoaded(socketpostprefixes, newpostnoun, 0);
                    sb.Append(of);
                    sb.Append(postnounwithaffixes);
                }
            }
            return sb;
        }

        private static StringBuilder DetermineFrontPrefix(Equipment equipment, AffixOverrides overrides, AffixReaderSO AffixReaderSO, List<string> front)
        {
            List<string> socketprefixes;
            if (overrides != null && overrides.Prefixes.Count > 0)
            {
                socketprefixes = overrides.Prefixes;
            }
            else
            {
                socketprefixes = equipment.GetStats().GetAllSocketPrefixes();
            }

            if (front.Count > 0)
            {

                front.Concat(socketprefixes);
                string prefixtoname = AffixReaderSO.GetNameWithAffixesPreLoaded(front, equipment.GetBaseItemName(), 0);
                sb.Append(prefixtoname);
            }
            else
            {
                //try create new
                string prefixtoname = AffixReaderSO.GetNameWithAffixesPreLoaded(socketprefixes, equipment.GetBaseItemName(), 0);
                sb.Append(prefixtoname);
            }
            return sb;
        }

        public static void RenameItemWithEnchant(Equipment equipment, EquipmentEnchant enchant, AffixReaderSO AffixReaderSO, RenameType type)
        {
            if (equipment == null || AffixReaderSO == null)
            {
                Debug.LogWarning("Can't rename item becuase either equipment or AffixReader is null");
                return;
            }
            sb.Clear();
            List<string> affixes = equipment.GetStats().GetAllTraitAffixes(true);
            if (affixes.Count == 0)
            {
                affixes.Concat(enchant.AffixOverrides.Prefixes);
                affixes.Concat(enchant.AffixOverrides.Suffixes);//try to see if overrides are set.
            }


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

                Debug.Log("Front " + front.Count);
                Debug.Log("Back " + back.Count);

                switch (type)
                {
                    case RenameType.Prefix:
                        sb = DetermineTraitrPrefix(equipment, enchant, AffixReaderSO, front);
                        for (int i = 0; i < back.Count; i++)
                        {
                            sb.Append(" ");
                            sb.Append(back[i]);
                        }
                       // sb.Append(" ");
                       // sb.Append(equipment.GetBaseItemName());
                        break;
                    case RenameType.Suffix:
                        for (int i = 0; i < front.Count; i++)
                        {
                            sb.Append(front[i]);
                            sb.Append(" ");
                        }
                        sb.Append(equipment.GetBaseItemName());
                        sb = DetermineTraitSuffix(equipment, enchant, AffixReaderSO, back);
                        break;
                    case RenameType.Both:
                        sb = DetermineTraitrPrefix(equipment, enchant, AffixReaderSO, front);
                        sb = DetermineTraitSuffix(equipment, enchant, AffixReaderSO, back);
                        break;
                }
                //
               

                


                equipment.SetGeneratedItemName(sb.ToString());
            }
           



        }

        private static StringBuilder DetermineTraitSuffix(Equipment equipment, EquipmentEnchant enchant, AffixReaderSO AffixReaderSO, List<string> back)
        {

            List<string> traitsuffixes;
            if (enchant.AffixOverrides.Suffixes.Count > 0)
            {
                traitsuffixes = enchant.AffixOverrides.Suffixes;
            }
            else
            {
                traitsuffixes = equipment.GetStats().GetAllTraitSuffixes(true);//
            }

            if (back.Count > 0)
            {
                
                string postnoun = "NULL";
                if (enchant.AffixOverrides.Nouns.Count > 0)
                {
                    postnoun = AffixReaderSO.GetPostNoun(enchant.AffixOverrides.Nouns);
                }
                else
                {
                    postnoun = AffixReaderSO.GetPostNoun(back);
                }

                back.Concat(traitsuffixes);
                if (string.IsNullOrEmpty(postnoun) == false)
                {
                  //  string basename = equipment.GetStats().GetBaseName();
                  //  sb.Append(basename);
                    string of = " of ";
                    string postnounwithaffixes = AffixReaderSO.GetNameWithAffixesPreLoaded(back, postnoun, 0);
           
                    sb.Append(of);
                    sb.Append(postnounwithaffixes);
                }

            }
            else
            {
                //try create one
                List<string> nouns = new List<string>();
                if (enchant.AffixOverrides.Nouns.Count > 0)
                {
                    nouns = enchant.AffixOverrides.Nouns;
                }
                else
                {
                    nouns = equipment.GetStats().GetAllTraitNouns();//
                }

                string newpostnoun = AffixReaderSO.GetPostNoun(nouns);
                if (string.IsNullOrEmpty(newpostnoun) == false)
                {
                  //  string basename = equipment.GetStats().GetBaseName();
                  //  sb.Append(basename);
                    string of = " of ";
                    string postnounwithaffixes = AffixReaderSO.GetNameWithAffixesPreLoaded(traitsuffixes, newpostnoun, 0);
            
                    sb.Append(of);
                    sb.Append(postnounwithaffixes);
                }
            }
            return sb;
        }

        private static StringBuilder DetermineTraitrPrefix(Equipment equipment, EquipmentEnchant enchant, AffixReaderSO AffixReaderSO, List<string> front)
        {
            List<string> socketprefixes;
            if (enchant.AffixOverrides.Prefixes.Count > 0)
            {
                socketprefixes = enchant.AffixOverrides.Prefixes;
            }
            else
            {
                socketprefixes = equipment.GetStats().GetAllTraitPrefixes(true);//
            }

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
            return sb;
        }

        public static void GetEquipmentInfo(Equipment equipment)
        {
        }
    }
}