using UnityEngine;

namespace SA
{
    public class M_Profile : Photon.MonoBehaviour
    {
        object[] objs;

        public int sendRate = 1;
        int _sendRate;

        public bool isLocal;
        public m_State m_state;
        public GameObject controller;
        StateManager states;

        public void Init(object[] objs, bool _isLocal)
        {
            isLocal = _isLocal;
            this.objs = objs;

            if (isLocal)
            {
                CreateController();
                m_state = m_State.normal;
                //controller = PhotonNetwork.Instantiate("Controller", Vector3.zero, Quaternion.identity, 0, objs) as GameObject;
            }
            else
            {
                CreateController();
                m_state = m_State.phantoms;
            }

        }
        public void CreateController()
        {
            controller = Instantiate(Resources.Load("PlayerControl"), Vector3.zero, Quaternion.identity) as GameObject;

            ArmorManager arm = controller.GetComponent<ArmorManager>();
            arm.m_chestId = (string)objs[0];
            arm.m_handsId = (string)objs[1];
            arm.m_headId = (string)objs[2];
            arm.m_legsId = (string)objs[3];

            InventoryManager inv = controller.GetComponent<InventoryManager>();
            inv.m_rh_weapons = (string)objs[4];
            inv.m_lh_weapons = (string)objs[5];

            states = controller.GetComponent<StateManager>();
            states.isLocal = isLocal;

            InputHandler inp = controller.GetComponent<InputHandler>();
            inp.Init();


            if (!isLocal)
                arm.ChangeAllToGhotst();
        }

      /*  public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            _sendRate++;
            if (_sendRate < sendRate)
                return;

            _sendRate = 0;

            if (stream.isWriting)
            {
                stream.SendNext(m_state);
            }
            else
            {
                m_state = (m_State)stream.ReceiveNext();
            }

        }*/
        void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            objs = photonView.instantiationData;
            isLocal = photonView.isMine;
            Init(objs, isLocal);
        }
    }
    public enum m_State
    {
        normal, interacting, phantoms
    }
}
