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
        public int m_value;
        public void Button_Create()
        {
            if (PhotonNetwork.connectedAndReady && PhotonNetwork.insideLobby)
            {
                RoomOptions roomOptions = new RoomOptions();
                roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
                roomOptions.CustomRoomProperties.Add("levelName", m_value);
                roomOptions.CustomRoomPropertiesForLobby = new string[2];
                roomOptions.CustomRoomPropertiesForLobby[0] = "levelName";
                roomOptions.CustomRoomPropertiesForLobby[1] = "gameType";
                roomOptions.MaxPlayers = 10;


                PhotonNetwork.CreateRoom(null, roomOptions, TypedLobby.Default);
                M_UIManager.singleton.UpdeatLogger("Creating room...");
                M_UIManager.singleton.CloseUI();
            }
            else
            {
                M_UIManager.singleton.UpdeatLogger("Wait for lobby connection...");
            }
        }
        public void ChangeRoomValue(int v)
        {
            m_value = v;
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
            roomOptions.CustomRoomProperties.Add("levelName", m_value);
            roomOptions.CustomRoomPropertiesForLobby = new string[2];
            roomOptions.CustomRoomPropertiesForLobby[0] = "levelName";
            roomOptions.CustomRoomPropertiesForLobby[1] = "gameType";

            PhotonNetwork.room.SetCustomProperties(roomOptions.CustomRoomProperties);
            Debug.Log("Change room value to " + m_value);
        }
        public void Invade()
        {
            PhotonNetwork.LeaveRoom();
            switchRooms = true;
        }
        bool switchRooms;
        public override void OnLeftRoom()
        {

            Debug.Log("OnLeftRoom");
            //   PhotonNetwork.ConnectUsingSettings(gameVersion);
            PhotonNetwork.JoinLobby();
        }
        public void Button_JoinRoom(RoomInfo r)
        {
            if (!PhotonNetwork.connectedAndReady || !PhotonNetwork.insideLobby)
            {
                M_UIManager.singleton.UpdeatLogger("Wait for lobby connection...");
                return;
            }

            if (r == null || string.IsNullOrEmpty(r.Name))
            {
                M_UIManager.singleton.UpdeatLogger("Room info is invalid.");
                return;
            }

            M_UIManager.singleton.CloseUI();
            M_UIManager.singleton.UpdeatLogger("Joining room " + r.Name);
            PhotonNetwork.JoinRoom(r.Name);
        }

        public override void OnJoinedLobby()
        {
            UIManager.singleton.mainMenu.SetActive(false);
            UIManager.singleton.multiplayerUI.SetActive(true);
            M_UIManager.singleton.OpenUI();
            M_UIManager.singleton.UpdeatLogger("Lobby joined", true);
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
            if (switchRooms)
            {
                switchRooms = false;
                Debug.Log("OnConnectedToMaster switchRooms");

                RoomOptions roomOptions = new RoomOptions();
                roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
                roomOptions.CustomRoomProperties.Add("levelName", m_value);
                roomOptions.CustomRoomPropertiesForLobby = new string[2];
                roomOptions.CustomRoomPropertiesForLobby[0] = "levelName";
                roomOptions.CustomRoomPropertiesForLobby[1] = "gameType";

                M_UIManager.singleton.CloseUI();
                M_UIManager.singleton.UpdeatLogger("Joining room " + m_value);
                PhotonNetwork.JoinRandomRoom(roomOptions.CustomRoomProperties, 10);
            }
        }
        public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
        {
            //  Debug.Log("Join room failed: " + codeAndMsg[1]);
            //  PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 10 }, null);
            M_UIManager.singleton.UpdeatLogger("Join room failed: ");
        }
        public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
        {
            string reason = (codeAndMsg != null && codeAndMsg.Length > 1) ? codeAndMsg[1].ToString() : "unknown";
            M_UIManager.singleton.OpenUI();
            M_UIManager.singleton.UpdeatLogger("Join room failed: " + reason);
        }
        public override void OnConnectedToMaster()
        {
            // PhotonNetwork.JoinRandomRoom();
            conncttedToRoom = true;
            M_UIManager.singleton.OpenUI();
            M_UIManager.singleton.UpdeatLogger("Connected to master, joining lobby...", true);
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