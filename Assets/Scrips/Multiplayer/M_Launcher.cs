using UnityEngine;
using System.Collections.Generic;

namespace SA
{
    public class M_Launcher : Photon.PunBehaviour
    {
        public PhotonLogLevel loglevel = PhotonLogLevel.ErrorsOnly;
        public static M_Launcher singleton;

        public string gameVersion = "v001";
        public string curRoom;

        M_UIManager m_uiManager;
        bool isLoading;

        [HideInInspector]
        public StateManager localState;

        void Awake()
        {
            singleton = this;
            PhotonNetwork.autoJoinLobby = false;
            PhotonNetwork.automaticallySyncScene = false;
        }
        void Start()
        {
            m_uiManager = M_UIManager.singleton;
            PhotonNetwork.ConnectUsingSettings(gameVersion);
            m_uiManager.CloseUI();
            m_uiManager.UpdeatLogger("Connecting to server...");
        }

        void Update()
        {
            if (isLoading)
                return;

            /*    if (PhotonNetwork.insideLobby)
                {
                    RoomOptions roomOptions = new RoomOptions();
                    Debug.Log("Connecting");

                    PhotonNetwork.JoinOrCreateRoom("area1", roomOptions, TypedLobby.Default);
                    isLoading = true;
                }*/
        }

        bool isCreated;
        public override void OnConnectedToMaster()
        {
            if (!isCreated)
            {
                base.OnConnectedToMaster();
                MySceneManager.singleton.PressStartGame();
                //SessionManager.singleton.InitGame();
                Debug.Log("Connected to master");
                isCreated = true;
            }
        }
        public void JoinRoom(string roomName, StateManager local)
        {
            curRoom = roomName;
            localState = local;
            RoomOptions roomOptions = new RoomOptions();
            PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
        }
        public void LeaveRoom(StateManager local)
        {
            localState = local;
            PhotonNetwork.LeaveRoom();
        }
        public override void OnFailedToConnectToPhoton(DisconnectCause cause)
        {
            base.OnFailedToConnectToPhoton(cause);
            MySceneManager.singleton.PressStartGame();
            Debug.Log("Failed to connect to photon");
        }
        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
        }
        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            Debug.Log("Joined room");
            SessionManager.singleton.MultiplayerFootprint();
            if (localState)
                localState.isInRoom = true;
            //  SessionManager.singleton.InitGame();
        }
        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            Debug.Log("Left room");
            if (localState)
                localState.isInRoom = false;

            ClearPlayerList();
        }
        public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
        {
            CurrentPlayers cp = null;
            for (int i = 0; i < playerList.Count; i++)
            {
                if (playerList[i].id == otherPlayer.ID)
                {
                    cp = playerList[i];
                    break;
                }
            }
            if (cp != null && cp.states)
            {
                if (cp.states.inventoryManager && cp.states.inventoryManager.referenceParent)
                    Destroy(cp.states.inventoryManager.referenceParent);
                Destroy(cp.states.gameObject);
                playerList.Remove(cp);
            }
        }
        void ClearPlayerList()
        {
            for (int i = 0; i < playerList.Count; i++)
            {
                if (playerList[i].states != localState)
                {
                    Destroy(playerList[i].states.inventoryManager.referenceParent);
                    Destroy(playerList[i].states.gameObject);
                }
            }
            playerList.Clear();
        }
        public List<CurrentPlayers> playerList = new List<CurrentPlayers>();
        public void AddPlayer(M_Listener listener, StateManager states, int id)
        {
            CurrentPlayers p = new CurrentPlayers();
            p.listener = listener;
            p.states = states;
            p.id = id;
            listener.SetPlayerContainer(p);
            playerList.Add(p);
        }
        public CurrentPlayers GetPlayer(int id)
        {
            for (int i = 0; i < playerList.Count; i++)
            {
                if (playerList[i].id == id)
                {
                    return playerList[i];
                }
            }
            return null;
        }
    }
    [System.Serializable]
    public class CurrentPlayers
    {
        public int id;
        public StateManager states;
        public M_Listener listener;
    }
}
