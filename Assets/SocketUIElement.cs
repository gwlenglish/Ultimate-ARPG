using GWLPXL.ARPGCore.Items.com;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
namespace GWLPXL.ARPGCore.CanvasUI.com
{

    public interface ISocketUIElement
    {
        void SetSocket(Socket socket);
        Socket GetSocket();
    }
    public class SocketUIElement : MonoBehaviour, ISocketUIElement
    {
        public Image Image = default;
        public Sprite EmptySprite = null;
        Socket socket = null;


        public Socket GetSocket()
        {
            return socket;
        }

        public void SetSocket(Socket socket)
        {
            this.socket = socket;
            Setup(socket);
        }

        protected virtual void Setup(Socket socket)
        {
            if (this.socket == null)
            {
                Image.sprite = EmptySprite;

            }
            else
            {
                if (socket.SocketedThing is EquipmentSocketable)
                {
                    EquipmentSocketable sock = socket.SocketedThing as EquipmentSocketable;
                    Image.sprite = sock.GetSprite();
                }

            }
        }
    }
}