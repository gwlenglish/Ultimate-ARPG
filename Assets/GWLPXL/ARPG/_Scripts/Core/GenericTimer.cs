
using GWLPXL.ARPGCore.com;

namespace GWLPXL.ARPGCore.com
{
    public class GenericTimer : ITick
    {
        public System.Action OnComplete;
        float duration;
        public GenericTimer(float duration)
        {
            this.duration = duration;
            AddTicker();
        }

        public void AddTicker()
        {
            TickManager.Instance.AddTicker(this);
        }

        public void DoTick()
        {
            OnComplete?.Invoke();
            RemoveTicker();
        }

        public float GetTickDuration() => duration;
       

        public void RemoveTicker()
        {
            TickManager.Instance.RemoveTicker(this);
        }
    }
}