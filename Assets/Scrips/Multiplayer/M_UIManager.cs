using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

namespace SA
{
    public class M_UIManager : MonoBehaviour
    {
        public GameObject logger;
        public TextMeshProUGUI logger_Text;
        public Transform roomGrid;
        public GameObject roomTemplate;
        public GameObject createRoom;
        public GameObject roomList;

        List<GameObject> roomsFound = new List<GameObject>();

        public void AddRoom(RoomInfo r)
        {
            GameObject go = Instantiate(roomTemplate) as GameObject;
            M_JoinRoom jr = go.GetComponent<M_JoinRoom>();
            jr.roomInfo = r;
            TextMeshProUGUI t = go.GetComponentInChildren<TextMeshProUGUI>();
            t.text = r.Name + " player count:" + r.PlayerCount;
            go.transform.SetParent(roomGrid);
            roomsFound.Add(go);
            go.SetActive(true);
        }
        public void UpdeatLogger(string str, bool close = false)
        {
            logger.SetActive(true);
            logger_Text.text = str;


            if (close)
            {
                StartCoroutine("CloseLogger");
            }
        }
        public void CloseUI()
        {
            roomGrid.gameObject.SetActive(false);
            createRoom.gameObject.SetActive(false);
            roomList.gameObject.SetActive(false);
        }
        public void OpenUI()
        {
            roomGrid.gameObject.SetActive(true);
            createRoom.gameObject.SetActive(true);
            roomList.gameObject.SetActive(true);
        }

        IEnumerator CloseLogger()
        {
            yield return new WaitForSeconds(1);
            logger.SetActive(false);

        }

        public static M_UIManager singleton;
        void Awake()
        {
            singleton = this;
        }
    }
}
