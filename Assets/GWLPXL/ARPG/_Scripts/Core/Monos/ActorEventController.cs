
using UnityEngine;

namespace GWLPXL.ARPGCore.com
{
    /// <summary>
    /// used to control the order of subscribe and unsubscribe
    /// </summary>
    public class ActorEventController : MonoBehaviour, IEventController
    {
        ISubscribeEvents[] eventSubbers = new ISubscribeEvents[0];
        public void SubEvents()
        {
            eventSubbers = GetComponentsInChildren<ISubscribeEvents>();
            for (int i = 0; i < eventSubbers.Length; i++)
            {
                eventSubbers[i].SubscribeEvents();
            }
        }

        public void UnSubEvents()
        {
            for (int i = 0; i < eventSubbers.Length; i++)
            {
                if (eventSubbers[i] == null) continue;
                eventSubbers[i].UnSubscribeEvents();
            }
        }

        public void TestEvent()
        {
            Debug.Log("Worked");
        }
    }
}