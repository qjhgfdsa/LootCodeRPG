using UnityEngine;
using System.Collections;

namespace SA
{
    public class Launchers : Photon.PunBehaviour
    {
        public PhotonLogLevel loglevel = PhotonLogLevel.Full;
        bool conncttedToRoom = false;
        string gameVersion = "1";
        bool isLoaded;
        bool hasRoom;
        bool uiIsOpen;


        public static Launchers singleton;

        void Awake()
        {
            singleton = this;
            PhotonNetwork.logLevel = loglevel;
            PhotonNetwork.autoJoinLobby = true;
            PhotonNetwork.automaticallySyncScene = true;
        }

        void Start()
        {
            PhotonNetwork.ConnectUsingSettings(gameVersion);
            M_UIManager.singleton.CloseUI();
            M_UIManager.singleton.UpdeatLogger("Connecting to server...");

            uiIsOpen = false;
        }
        void Update()
        {
            if (isLoaded)
                return;

            if (PhotonNetwork.insideLobby)
            {
                if (!uiIsOpen)
                {
                    M_UIManager.singleton.OpenUI();
                    uiIsOpen = true;
                }

                if (!hasRoom)
                {
                    M_UIManager uiManager = M_UIManager.singleton;
                    RoomInfo[] rm = PhotonNetwork.GetRoomList();
                    for (int i = 0; i < rm.Length; i++)
                    {
                        uiManager.AddRoom(rm[i]);
                    }
                    if (rm.Length > 0)
                        hasRoom = true;
                }

            }

        }
        public void Button_Create()
        {
            if (PhotonNetwork.connected)
            {
                PhotonNetwork.CreateRoom(null);
                M_UIManager.singleton.UpdeatLogger("Creating room...");
                M_UIManager.singleton.CloseUI();
            }
            else
            {
                Debug.Log("Not connected to server");
            }
        }
        public void Button_JoinRoom(RoomInfo r)
        {
            M_UIManager.singleton.CloseUI();
            M_UIManager.singleton.UpdeatLogger("Joining room");
            PhotonNetwork.JoinRoom(r.Name);
        }
        public override void OnJoinedLobby()
        {
            UIManager.singleton.mainMenu.SetActive(false);
            UIManager.singleton.multiplayerUI.SetActive(true);
        }
        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
            M_UIManager.singleton.UpdeatLogger("Room created");
            StartCoroutine("CreateRoomProcess");
        }
        IEnumerator CreateRoomProcess()
        {
            yield return new WaitForSeconds(1);
            
            if (PhotonNetwork.isMasterClient)
                LoadLevel();
        }
        public override void OnJoinedRoom()
        {
            M_UIManager.singleton.UpdeatLogger("Joined room", true);
        }
        public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
        {
            Debug.Log("Join room failed: " + codeAndMsg[1]);
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 10 }, null);
        }
        public override void OnConnectedToMaster()
        {
            // PhotonNetwork.JoinRandomRoom();
            conncttedToRoom = true;
            M_UIManager.singleton.OpenUI();
            M_UIManager.singleton.UpdeatLogger("Connected", true);
        }

        public void LoadLevel()
        {
            if (!PhotonNetwork.isMasterClient)
            {

            }
            else
            {
                UIManager.singleton.mainMenu.SetActive(true);
                PhotonNetwork.LoadLevel("main");
                isLoaded = true;
            }
        }


        void OnApplicationQuit()
        {
            Debug.Log("ออกเกม");
            QuitGame();
        }

        public void QuitGame()
        {
            PhotonNetwork.LeaveRoom();
        }
    }
}