using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SA
{
    [ExecuteInEditMode]
    public class WeaponPlacer : MonoBehaviour
    {
        public string itemName;
        public GameObject targetModel;
        public bool leftHand;
        public bool saveItem;

        public SaveType saveType;

        public enum SaveType
        {
            Weapon, Consumable
        }

        void Update()
        {
            if (!saveItem)
                return;

            saveItem = false;

            switch (saveType)
            {
                case SaveType.Weapon:
                    SaveWeapon();
                    break;
                case SaveType.Consumable:
                    SaveConsumable();
                    break;
            }
        }

        void SaveWeapon()
        {
            if (targetModel == null)
            {
                Debug.LogWarning("WeaponPlacer: targetModel is null.");
                return;
            }
            if (string.IsNullOrEmpty(itemName))
            {
                Debug.LogWarning("WeaponPlacer: itemName is empty.");
                return;
            }

            WeaponScriptableObject obj = Resources.Load("SA.WeaponScriptableObject") as WeaponScriptableObject;
            if (obj == null)
            {
                Debug.LogWarning("WeaponPlacer: SA.WeaponScriptableObject not found.");
                return;
            }

            for (int i = 0; i < obj.weapons_all.Count; i++)
            {
                if (obj.weapons_all[i].Item_id != itemName)
                    continue;

                Weapon w = obj.weapons_all[i];
                if (leftHand)
                {
                    w.l_model_eulers = targetModel.transform.localEulerAngles;
                    w.l_model_pos = targetModel.transform.localPosition;
                }
                else
                {
                    w.r_model_eulers = targetModel.transform.localEulerAngles;
                    w.r_model_pos = targetModel.transform.localPosition;
                }
                w.model_scale = targetModel.transform.localScale;

                PersistAsset(obj);
                string hand = leftHand ? "left" : "right";
                Debug.Log($"WeaponPlacer: saved weapon \"{itemName}\" ({hand} hand) pos={targetModel.transform.localPosition} eulers={targetModel.transform.localEulerAngles} scale={targetModel.transform.localScale}");
                return;
            }

            Debug.Log($"WeaponPlacer: \"{itemName}\" wasn't found in weapons.");
        }

        void SaveConsumable()
        {
            if (targetModel == null)
            {
                Debug.LogWarning("WeaponPlacer: targetModel is null.");
                return;
            }
            if (string.IsNullOrEmpty(itemName))
            {
                Debug.LogWarning("WeaponPlacer: itemName is empty.");
                return;
            }

            ConsumablesScriptableObject obj = Resources.Load("SA.ConsumablesScriptableObject") as ConsumablesScriptableObject;
            if (obj == null)
            {
                Debug.LogWarning("WeaponPlacer: SA.ConsumablesScriptableObject not found.");
                return;
            }

            for (int i = 0; i < obj.consumables.Count; i++)
            {
                if (obj.consumables[i].Item_id != itemName)
                    continue;

                Consumable c = obj.consumables[i];
                c.r_model_eulers = targetModel.transform.localEulerAngles;
                c.r_model_pos = targetModel.transform.localPosition;
                c.model_scale = targetModel.transform.localScale;

                PersistAsset(obj);
                Debug.Log($"WeaponPlacer: saved consumable \"{itemName}\" (right hand) pos={targetModel.transform.localPosition} eulers={targetModel.transform.localEulerAngles} scale={targetModel.transform.localScale}");
                return;
            }

            Debug.Log($"WeaponPlacer: \"{itemName}\" wasn't found in consumables.");
        }

        static void PersistAsset(ScriptableObject obj)
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(obj);
            AssetDatabase.SaveAssets();
#endif
        }
    }
}
