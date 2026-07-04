using UnityEngine;

namespace SA
{
    public class M_Launcher : Photon.PunBehaviour
    {
        public PhotonLogLevel loglevel = PhotonLogLevel.ErrorsOnly;
        public static M_Launcher singleton;

        public string gameVersion = "v001";

        M_UIManager m_uiManager;
        bool isLoading;

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
        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            SessionManager.singleton.InitGame();
        }
        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
        }
        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            Debug.Log("Joined room");
          //  SessionManager.singleton.InitGame();
        }
    }
}
