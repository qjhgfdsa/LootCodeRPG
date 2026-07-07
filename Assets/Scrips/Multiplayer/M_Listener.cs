using UnityEngine;


namespace SA
{
    public class M_Listener : Photon.MonoBehaviour
    {
        public StateManager states;

        public int myId;
        public int targetEnemyId;
        public bool invadeBotton;
        bool invade;
        CurrentPlayers playerContainer;
        public void SetPlayerContainer(CurrentPlayers p)
        {
            playerContainer = p;
        }
        void Start()
        {

        }

        void Update()
        {
            if (invadeBotton)
            {
                invadeBotton = false;
                TryToInvade();
            }

            if (!states.isLocal)
                states.NetworkTick();

        }
       public object[] instantiationData;
        void TryToInvade()
        {
            Debug.Log("Trying to invade");
            invade = true;
            M_Launcher m = M_Launcher.singleton;
            CurrentPlayers p = m.GetPlayer(targetEnemyId);
            GameObject controller = Instantiate(Resources.Load("PlayerControl")) as GameObject;
            controller.transform.position = p.states.transform.position;
            controller.transform.rotation = p.states.transform.rotation;
            Destroy(p.states.inventoryManager.referenceParent);
            Destroy(p.states.gameObject);
            p.states = controller.GetComponent<StateManager>();
            LoadInstantiationData(p.listener.instantiationData, controller.transform);
            p.states.Init();
            p.listener.states = p.states;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            myId = PhotonNetwork.player.ID;

            if (states.anim == null)
                return;

            if (stream.isWriting)
            {
                stream.SendNext(states.transform.position);
                stream.SendNext(states.transform.rotation);

                stream.SendNext(states.anim.GetFloat(StaticStrings.Horizontal_Axis));
                stream.SendNext(states.anim.GetFloat(StaticStrings.Vertical_Axis));
                stream.SendNext(states.anim.GetBool(StaticStrings.lockon));

                stream.SendNext(states.sendAnim);
                if (states.sendAnim)
                {
                    bool isMirrored = states.anim.GetBool(StaticStrings.mirror);
                    stream.SendNext(isMirrored);
                    stream.SendNext(states.sendTargetAnim);
                    states.sendAnim = false;
                }

                stream.SendNext(states.sendEquipment);
                if (states.sendEquipment)
                {
                    ArmorSnapshot snap = states.inventoryManager.armorManager.GetSnapshot();
                    stream.SendNext(snap.m_chestId);
                    stream.SendNext(snap.m_handsId);
                    stream.SendNext(snap.m_headId);
                    stream.SendNext(snap.m_legsId);
                    states.sendEquipment = false;
                }

                stream.SendNext(states.sendWeapons);
                if (states.sendWeapons)
                {
                    stream.SendNext(states.inventoryManager.m_lh_weapons);
                    stream.SendNext(states.inventoryManager.m_rh_weapons);
                    states.sendWeapons = false;
                }

                stream.SendNext(states.sendSecAnim);
                if (states.sendSecAnim)
                {
                    stream.SendNext(states.secAnims.Count);
                    for (int i = 0; i < states.secAnims.Count; i++)
                    {
                        stream.SendNext(states.secAnims[i]);
                    }
                    states.secAnims.Clear();
                    states.sendSecAnim = false;
                }

                stream.SendNext(states.isTwoHanded);
                stream.SendNext(invade);
                stream.SendNext(targetEnemyId);
            }
            else
            {
                Vector3 p = (Vector3)stream.ReceiveNext();
                Quaternion r = (Quaternion)stream.ReceiveNext();
                ReceivePositionRotation(p, r);

                float h = (float)stream.ReceiveNext();
                float v = (float)stream.ReceiveNext();
                bool lockon = (bool)stream.ReceiveNext();

                states.horizontal = h;
                states.vertical = v;
                states.anim.SetBool(StaticStrings.lockon, lockon);

                bool playAnim = (bool)stream.ReceiveNext();
                if (playAnim)
                {
                    bool mirror = (bool)stream.ReceiveNext();
                    states.anim.SetBool(StaticStrings.mirror, mirror);
                    string tAnim = (string)stream.ReceiveNext();
                    states.anim.Play(tAnim);
                }

                bool eqipment = (bool)stream.ReceiveNext();
                if (eqipment)
                {
                    ArmorSnapshot arm = new ArmorSnapshot();
                    arm.m_chestId = (string)stream.ReceiveNext();
                    arm.m_handsId = (string)stream.ReceiveNext();
                    arm.m_headId = (string)stream.ReceiveNext();
                    arm.m_legsId = (string)stream.ReceiveNext();
                    states.inventoryManager.armorManager.LoadSnapshot(arm);
                    states.inventoryManager.armorManager.EquipAllMultiplayer();
                }

                bool weapons = (bool)stream.ReceiveNext();
                if (weapons)
                {
                    states.inventoryManager.m_lh_weapons = (string)stream.ReceiveNext();
                    states.inventoryManager.m_rh_weapons = (string)stream.ReceiveNext();
                    states.inventoryManager.LoadInventory();
                }

                bool sAnim = (bool)stream.ReceiveNext();
                if (sAnim)
                {
                    int count = (int)stream.ReceiveNext();
                    for (int i = 0; i < count; i++)
                    {
                        string a = (string)stream.ReceiveNext();
                        states.anim.CrossFade(a, 0.2f);
                    }
                }

                bool isTwoHand = (bool)stream.ReceiveNext();
                states.isTwoHanded = isTwoHand;
                if (isTwoHand)
                {
                    if (states.inventoryManager.leftHandWeapon.weaponModel)
                        states.inventoryManager.leftHandWeapon.weaponModel.SetActive(false);
                }
                else
                {
                    if (states.inventoryManager.rightHandWeapon.weaponModel)
                        states.inventoryManager.rightHandWeapon.weaponModel.SetActive(true);
                }

                bool invaded = (bool)stream.ReceiveNext();
                int target = (int)stream.ReceiveNext();
                if (invaded)
                {
                    if (myId == target)
                    {
                        if (states.inventoryManager.armorManager.isGhost)
                        {
                            Destroy(states.gameObject);
                            GameObject controller = Instantiate(Resources.Load("PlayerControl"), p, r) as GameObject;
                            states = controller.GetComponent<StateManager>();
                            LoadInstantiationData(photonView.instantiationData, controller.transform);
                            states.Init();
                            playerContainer.states = states;
                        }
                    }
                }
            }
        }
        public float movementThreshold = 0.1f;
        public float angleThreshold = 5f;
        void ReceivePositionRotation(Vector3 pos, Quaternion rot)
        {
            states.lastDirection = pos - states.lastPosition;
            states.lastDirection /= PhotonNetwork.sendRate;

            if (states.lastDirection.magnitude > movementThreshold)
                states.lastDirection = Vector3.zero;

            Vector3 lastEuler = states.lastRotation.eulerAngles;
            Vector3 newEuler = rot.eulerAngles;

            if (Quaternion.Angle(states.lastRotation, rot) < angleThreshold)
            {
                states.lastRotation = Quaternion.Euler((newEuler - lastEuler) / PhotonNetwork.sendRate);
            }
            else
            {
                states.lastRotation = Quaternion.identity;
            }
            states.lastPosition = pos;
            states.lastRotation = rot;
        }
        void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            instantiationData = photonView.instantiationData;

            if (photonView.isMine)
            {
                states = M_Launcher.singleton.localState;
            }
            else
            {
                GameObject controller = Instantiate(Resources.Load("PlayerControl"), Vector3.zero, Quaternion.identity) as GameObject;
                states = controller.GetComponent<StateManager>();
                LoadInstantiationData(instantiationData, controller.transform);

                states.Init();
                states.inventoryManager.armorManager.ChangeAllToGhotst();
                states.rigid.isKinematic = true;
                states.controllerCollider.enabled = false;
            }

            M_Launcher.singleton.AddPlayer(this, states, info.sender.ID);
        }
        void LoadInstantiationData(object[] objs, Transform go)
        {
            ArmorManager arm = go.GetComponent<ArmorManager>();
            arm.m_chestId = (string)objs[0];
            arm.m_handsId = (string)objs[1];
            arm.m_headId = (string)objs[2];
            arm.m_legsId = (string)objs[3];

            InventoryManager inv = go.GetComponent<InventoryManager>();
            inv.m_rh_weapons = (string)objs[4];
            inv.m_lh_weapons = (string)objs[5];


        }
    }
}
