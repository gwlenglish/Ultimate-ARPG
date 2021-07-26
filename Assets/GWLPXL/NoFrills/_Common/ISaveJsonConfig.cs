using UnityEngine;

namespace GWLPXL.NoFrills.Common.com
{
    public interface ISaveJsonConfig
    {
        void SetTextAsset(TextAsset textAsset);
        TextAsset GetTextAsset();
        UnityEngine.Object GetObject();

    }
}