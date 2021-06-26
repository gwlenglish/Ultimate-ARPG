using GWLPXL.ARPGCore.Items.com;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GWLPXL.ARPGCore.CanvasUI.com
{
    public interface ISocketHolderUIElement
    {
        void SetSockets(Equipment forEquipment);
    }

    public class SocketHolderUIElement : MonoBehaviour, ISocketHolderUIElement
    {
        public GameObject SocketPrefab = default;
        public Transform SocketParent = default;
        List<GameObject> instances = new List<GameObject>();
        Equipment equipment = null;

        public void SetSockets(Equipment forEquipment)
        {
            this.equipment = forEquipment;

            Setup(forEquipment);

        }

        protected virtual void Setup(Equipment forEquipment)
        {
            for (int i = 0; i < instances.Count; i++)
            {
                Destroy(instances[i]);//remove old
            }
            instances.Clear();
            List<Socket> sockets = forEquipment.GetStats().GetSockets();
            for (int i = 0; i < sockets.Count; i++)
            {
                GameObject instance = Instantiate(SocketPrefab, SocketParent);
                ISocketUIElement sock = instance.GetComponent<ISocketUIElement>();
                sock.SetSocket(sockets[i]);
                instances.Add(instance);

            }
        }

       
    }
}