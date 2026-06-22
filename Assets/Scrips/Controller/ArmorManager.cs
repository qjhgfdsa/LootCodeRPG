using UnityEngine;

namespace SA
{
    public class ArmorManager : MonoBehaviour
    {
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

        public void Init()
        {
            EquipAll();
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
