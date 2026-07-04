using UnityEngine;

namespace SA
{
    public class ArmorManager : MonoBehaviour
    {
        public string m_chestId;
        public string m_legsId;
        public string m_headId;
        public string m_handsId;

        public int chestId;
        public int legsId;
        public int handsId;
        public int headId;

        public SkinnedMeshRenderer chestPiece;
        public SkinnedMeshRenderer legsPiece;
        public SkinnedMeshRenderer handsPiece;
        public SkinnedMeshRenderer headPiece;

        public SkinnedMeshRenderer a_chestPiece;
        public SkinnedMeshRenderer a_legsPiece;
        public SkinnedMeshRenderer a_handsPiece;
        public SkinnedMeshRenderer a_headPiece;

        StateManager states;

        public Material ghost;

        public void Init(StateManager st)
        {
            states = st;

            if (states.isLocal)
                EquipAll();
            else
                EquipAllMultiplayer();
        }
        void EquipAll()
        {
            LoadArmor(chestId, ArmorType.chest);
            LoadArmor(legsId, ArmorType.legs);
            LoadArmor(handsId, ArmorType.hands);
            LoadArmor(headId, ArmorType.head);
        }
        void LoadArmor(int id, ArmorType t)
        {
            if (id == -1)
            {
                UnequipArmor(t);
                return;
            }
            ItemInventoryInstance item = SessionManager.singleton.GetArmorItem(id);
            ArmorContainer a = ResourcesManager.singleton.GetArmor(item.itemId);
            EquipArmor(a);
        }
        public void EquipAllMultiplayer()
        {
            LoadArmorMultiplayer(m_chestId, ArmorType.chest);
            LoadArmorMultiplayer(m_headId, ArmorType.head);
            LoadArmorMultiplayer(m_handsId, ArmorType.hands);
            LoadArmorMultiplayer(m_legsId, ArmorType.legs);
        }
        void LoadArmorMultiplayer(string id, ArmorType a)
        {
            if (string.Equals("empty", id) || string.IsNullOrEmpty(id))
            {
                UnequipArmor(a);
                return;
            }

        }

        public void UnequipArmor(ArmorType t)
        {
            switch (t)
            {
                case ArmorType.chest:
                    chestPiece.enabled = true;
                    a_chestPiece.gameObject.SetActive(false);
                    break;
                case ArmorType.legs:
                    legsPiece.enabled = true;
                    a_legsPiece.gameObject.SetActive(false);
                    break;
                case ArmorType.hands:
                    handsPiece.enabled = true;
                    a_handsPiece.gameObject.SetActive(false);
                    break;
                case ArmorType.head:
                    headPiece.enabled = true;
                    a_headPiece.gameObject.SetActive(false);
                    break;
                default:
                    break;
            }
        }
        public void EquipArmor(ArmorContainer a)
        {
            switch (a.armorType)
            {
                case ArmorType.chest:
                    UpdateSkinMeshRenderer(a, a_chestPiece, chestPiece);
                    break;
                case ArmorType.legs:
                    UpdateSkinMeshRenderer(a, a_legsPiece, legsPiece);
                    break;
                case ArmorType.hands:
                    UpdateSkinMeshRenderer(a, a_handsPiece, handsPiece);
                    break;
                case ArmorType.head:
                    UpdateSkinMeshRenderer(a, a_headPiece, headPiece);
                    break;
            }

        }
        void UpdateSkinMeshRenderer(ArmorContainer a, SkinnedMeshRenderer ren, SkinnedMeshRenderer bodyRen)
        {
            ren.sharedMesh = a.armorMesh;

            Material[] newMats = new Material[a.armorMaterials.Length];
            for (int i = 0; i < a.armorMaterials.Length; i++)
            {
                newMats[i] = a.armorMaterials[i];
            }
            ren.materials = newMats;

            bodyRen.enabled = a.BaseBodyEnabled;
            ren.gameObject.SetActive(true);
        }
        public ArmorSnapshot GetSnapshot()
        {
            ArmorSnapshot a = new ArmorSnapshot();
            a.m_chestId = GetArmorIdFromInt(chestId);
            a.m_legsId = GetArmorIdFromInt(legsId);
            a.m_handsId = GetArmorIdFromInt(handsId);
            a.m_headId = GetArmorIdFromInt(headId);
            return a;
        }
        public void LoadSnapshot(ArmorSnapshot snap)
        {
            m_chestId = snap.m_chestId;
            m_legsId = snap.m_legsId;
            m_handsId = snap.m_handsId;
            m_headId = snap.m_headId;
        }
        string GetArmorIdFromInt(int id)
        {
            if (id == -1)
                return "empty";
            ItemInventoryInstance item = SessionManager.singleton.GetArmorItem(id);
            return item.itemId;
        }
        public void ChangeAllToGhotst()
        {
            chestPiece.material = ghost;
            legsPiece.material = ghost;
            handsPiece.material = ghost;
            headPiece.material = ghost;

            a_chestPiece.material = ghost;
            a_legsPiece.material = ghost;
            a_handsPiece.material = ghost;
            a_headPiece.material = ghost;

        }
    }

    public enum ArmorType
    {
        chest, legs, hands, head,
    }
    [System.Serializable]
    public class ArmorContainer
    {
        public string itemId;
        public ArmorType armorType;
        public Mesh armorMesh;
        public Material[] armorMaterials;
        public bool BaseBodyEnabled;

    }
}
