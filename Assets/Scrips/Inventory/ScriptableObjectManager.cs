using UnityEngine;
using System.IO;
using System;
using UnityEditor;

namespace SA
{
    public class ScriptableObjectManager
    {
        public static void CreateAsset<T>() where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();

            if (Resources.Load(typeof(T).ToString()) == null)
            {
                String assetPath = AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/" + typeof(T).ToString() + ".asset");
                AssetDatabase.CreateAsset(asset, assetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = asset;
            }
            else
            {
                Debug.Log(typeof(T).ToString() + " already exists");
            }
        }

        [MenuItem("Assets/Inventory/Create Inventory List Scriptable Object")]
        public static void CreateInventory()
        {

        }

        [MenuItem("Assets/Inventory/Create Spell Items List Scriptable Object")]
        public static void CreateSpellItemsList()
        {
            ScriptableObjectManager.CreateAsset<SpellItemScriptableObject>();
        }


        [MenuItem("Assets/Inventory/Create Weapon List Scriptable Object")]
        public static void CreateWeaponList()
        {
            ScriptableObjectManager.CreateAsset<WeaponScriptableObject>();
        }
    }
}