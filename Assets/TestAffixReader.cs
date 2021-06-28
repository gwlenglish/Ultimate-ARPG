using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAffixReader : MonoBehaviour
{
    public string Phrase;
    public KeyCode PhraseKey = KeyCode.Space;
    public List<string> Words = new List<string>();
    public KeyCode WordKey = KeyCode.Return;
    public AffixReaderSO AffixReader;
    public bool Preload;
    private void Update()
    {
        if (Input.GetKeyDown(PhraseKey))
        {
            if (Preload)
            {
                AffixReader.GetSortedAffixesFromPreload(Phrase);
            }
            else
            {
                AffixReader.GetSortedAffixes(Phrase);
            }

        }

        if (Input.GetKeyDown(WordKey))
        {
            if (Preload)
            {
                AffixReader.GetSortedAffixesFromPreload(Words);
            }
            else
            {
                AffixReader.GetSortedAffixes(Words);
            }

        }
    }
}
