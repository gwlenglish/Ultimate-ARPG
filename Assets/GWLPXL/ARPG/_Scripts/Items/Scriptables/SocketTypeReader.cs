using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ARPGCore.Items.com
{
    [System.Serializable]
    public class SocketType
    {
        public GWLPXL.ARPGCore.Types.com.SocketTypes Type = default;
        public Sprite Sprite = default;
        public string Name = default;
        public string Description = default;

    }
    /// <summary>
    /// used to map common information to the socket type
    /// </summary>
    [CreateAssetMenu(menuName ="GWLPXL/ARPG/Socketables/SocketReader")]
    public class SocketTypeReader : ScriptableObject
    {
        public List<SocketType> TypeReaders = new List<SocketType>();
    }
}
