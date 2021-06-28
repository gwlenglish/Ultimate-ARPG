using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;

[System.Serializable]
public class AffixOrder
{
    public string Label;
    public string EditorDescription = string.Empty;
    public List<string> Words = new List<string>();
}

public struct SortedAffix
{
    public string Affix;
    public int Index;
    public SortedAffix(string affix, int index)
    {
        Affix = affix;
        Index = index;
    }
}


public static class AffixHelper
{
    static StringBuilder sb = new StringBuilder();
    static readonly string space = " ";

    public static bool WordEquals(string affix, string word)
    {
        return string.CompareOrdinal(affix.ToLower(), word.ToLower()) == 0;
    }
    /// <summary>
    /// sorts the affixes based on the order found in the reader. Returns as a phrase.
    /// </summary>
    /// <param name="affixesToSort"></param>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static string GetSortedAffixes(List<string> affixesToSort, AffixReader reader)
    {
        sb.Clear();
        List<AffixOrder> orders = reader.AffixOrders;
        List<SortedAffix> sortedAffixes = new List<SortedAffix>();
        for (int i = 0; i < affixesToSort.Count; i++)
        {
            string affix = affixesToSort[i];
            for (int j = 0; j < orders.Count; j++)
            {
                AffixOrder order = orders[j];
                int index = j;
                for (int k = 0; k < order.Words.Count; k++)
                {
                    string word = order.Words[k];
                    if (WordEquals(affix, word))
                    {
                        sortedAffixes.Add(new SortedAffix(affix, index));
                    }
                  
                }
            }
        }


        sortedAffixes = sortedAffixes.OrderBy(x => x.Index).ToList();
        for (int i = 0; i < sortedAffixes.Count; i++)
        {
            sb.Append(sortedAffixes[i].Affix);
            sb.Append(space);
        }
        sortedAffixes.Clear();
        string stringsorted = sb.ToString();
        UnityEngine.Debug.Log(stringsorted);

        return stringsorted;
    }
    /// <summary>
    /// Splits the phrase, then returns it sorted based on the reader order. You can overload the delimiter
    /// </summary>
    /// <param name="phrase"></param>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static string GetSortedAffixes(string phrase, AffixReader reader, char delimiter = ' ')
    {
        List<string> split = phrase.Split(delimiter).ToList();
        return GetSortedAffixes(split, reader);
    }
    /// <summary>
    /// Returns a sorted version of the affixes, but uses a preloaded dictionary instead of looping. Mostly just an optimized version of the base.
    /// </summary>
    /// <param name="affixesToSort"></param>
    /// <param name="reader"></param>
    /// <param name="preloadDic"></param>
    /// <returns></returns>
    public static string GetSortedAffixesFromPreload(List<string> affixesToSort, AffixReader reader, Dictionary<string, int> preloadDic)
    {
        sb.Clear();

        if (preloadDic.Count == 0)
        {
            LoadReader(reader, preloadDic);
        }

        List<SortedAffix> sortedAffixes = new List<SortedAffix>();
        for (int i = 0; i < affixesToSort.Count; i++)
        {

            string word = affixesToSort[i];
            string key = word.ToLower();

            if (preloadDic.ContainsKey(key))
            {
                sortedAffixes.Add(new SortedAffix(word, preloadDic[key]));
            }
        }

        sortedAffixes = sortedAffixes.OrderBy(x => x.Index).ToList();
        for (int i = 0; i < sortedAffixes.Count; i++)
        {
            sb.Append(sortedAffixes[i].Affix);
            sb.Append(space);
        }


        string sortedaffixes = sb.ToString();

        UnityEngine.Debug.Log(sortedaffixes);
        return sortedaffixes;
    }
    /// <summary>
    /// Splits the phrase then returns a sorted version based on the reader order. You can overload the delimiter
    /// </summary>
    /// <param name="phrase"></param>
    /// <param name="reader"></param>
    /// <param name="preloadDic"></param>
    /// <returns></returns>
    public static string GetSortedAffixesFromPreload(string phrase, AffixReader reader, Dictionary<string, int> preloadDic, char delimiter = ' ')
    {
        List<string> split = phrase.Split(delimiter).ToList();
        return GetSortedAffixesFromPreload(split, reader, preloadDic);
    }
    /// <summary>
    /// Keys a dictionary to word.ToLower() and index in the reader.
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="preloadDic"></param>
    public static void LoadReader(AffixReader reader, Dictionary<string, int> preloadDic)
    {
        for (int i = 0; i < reader.AffixOrders.Count; i++)
        {
            AffixOrder order = reader.AffixOrders[i];
            int index = i;
            for (int j = 0; j < order.Words.Count; j++)
            {

                string word = order.Words[j];

                if (string.IsNullOrEmpty(word)) continue;

                string key = word.ToLower();
                if (preloadDic.ContainsKey(key))
                {
                    UnityEngine.Debug.LogWarning("Duplicate entries in the reader. The word " + word.ToUpper() + " is found at index " + preloadDic[word].ToString() + " and " + index.ToString() + ". Will only use the first entry found of the word");
                    continue;
                }
                else
                {
                    preloadDic[key] = index;
                }
              
            }
        }
    }

}


[System.Serializable]
public class AffixReader
{
    public List<AffixOrder> AffixOrders = new List<AffixOrder>();
}

[CreateAssetMenu(menuName ="Test Word Reader")]
public class AffixReaderSO : ScriptableObject
{
    public AffixReader Affixreader;

    [System.NonSerialized]
    Dictionary<string, int> wordIndexDic = new Dictionary<string, int>();

    public virtual void ClearDictionary()
    {
        wordIndexDic.Clear();
    }
    public virtual void LoadReader()
    {
        AffixHelper.LoadReader(Affixreader, wordIndexDic);
        
    }
    public virtual string GetSortedAffixes(string phrase)
    {
        return AffixHelper.GetSortedAffixes(phrase, Affixreader);
    }
    public virtual string GetSortedAffixesFromPreload(string phrase)
    {
        return AffixHelper.GetSortedAffixesFromPreload(phrase, Affixreader, wordIndexDic);
    }
    public virtual string GetSortedAffixesFromPreload(List<string> affixesToSort)
    {
        return AffixHelper.GetSortedAffixesFromPreload(affixesToSort, Affixreader, wordIndexDic);
    }
    public virtual string GetSortedAffixes(List<string> affixesToSort)
    {
            return AffixHelper.GetSortedAffixes(affixesToSort, Affixreader);
    }
}


#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(AffixReaderSO))]
public class AffixReaderSOEditor : UnityEditor.Editor
{
    readonly List<string> labels = new List<string>(8) { "Quantity", "Quality", "Size", "Age", "Shape", "Color", "Proper adjective", "Purpose" };

    public override void OnInspectorGUI()
    {
        AffixReaderSO so = (AffixReaderSO)target;
        List<AffixOrder> orders = so.Affixreader.AffixOrders;


        while (orders.Count < labels.Count)
        {
            orders.Add(new AffixOrder());
        }

        for (int i = 0; i < orders.Count; i++)
        {
            orders[i].Label = labels[i];
        }


        //a way to save out the affix orders to json to edit elsewhere, and then to reimport into this.


        base.OnInspectorGUI();


        if (GUILayout.Button("Save to Json"))
        {
           string path = UnityEditor.EditorUtility.SaveFilePanelInProject("Save File", "Name", "json", "Choose where to save.");
            if (string.IsNullOrEmpty(path) == false)
            {
                string json = JsonUtility.ToJson(so.Affixreader, true);
                using (System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Create))
                {
                    using (System.IO.StreamWriter writer = new System.IO.StreamWriter(fs))
                    {
                        writer.Write(json);
                    }
                }
            }

            UnityEditor.AssetDatabase.Refresh();
        }

        if (GUILayout.Button("Load from Json"))
        {
            string path = UnityEditor.EditorUtility.OpenFilePanel("Open", "Assets", "json");
            if (string.IsNullOrEmpty(path) == false)
            {
                string json = string.Empty;
                using (System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open))
                {
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(fs))
                    {
                        json = reader.ReadToEnd();
                    }
                }

                so.Affixreader = JsonUtility.FromJson<AffixReader>(json);
            }
        }
    }
}

#endif
