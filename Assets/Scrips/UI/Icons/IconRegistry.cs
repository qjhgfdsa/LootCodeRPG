using System.Collections.Generic;
using SA;
using UnityEngine;

namespace SA.UI.Icons
{
    [CreateAssetMenu(fileName = "IconRegistry", menuName = "SA/UI/Icon Registry")]
    public class IconRegistry : ScriptableObject
    {
        const string DefaultResourcePath = "SA.IconRegistry";

        static IconRegistry instance;

        [SerializeField] List<IconRegistryEntry> entries = new List<IconRegistryEntry>();
        [SerializeField] Sprite missingIcon;

        Dictionary<string, Sprite> lookup;

        public static IconRegistry Instance
        {
            get
            {
                if (instance == null)
                    instance = Resources.Load<IconRegistry>(DefaultResourcePath);

                return instance;
            }
        }

        public static void SetInstance(IconRegistry registry)
        {
            instance = registry;
            if (instance != null)
                instance.BuildLookup();
        }

        void OnEnable()
        {
            lookup = null;
            BuildLookup();
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            lookup = null;
        }
#endif

        public Sprite Resolve(IconId id)
        {
            if (id.IsEmpty)
                return missingIcon;

            BuildLookup();

            if (lookup != null && lookup.TryGetValue(id.Value, out Sprite sprite) && sprite != null)
                return sprite;

            Debug.LogWarning($"IconRegistry: No sprite for icon id '{id.Value}'.");
            return missingIcon;
        }

        void BuildLookup()
        {
            if (lookup != null)
                return;

            lookup = new Dictionary<string, Sprite>();
            for (int i = 0; i < entries.Count; i++)
            {
                IconRegistryEntry entry = entries[i];
                if (entry == null || string.IsNullOrWhiteSpace(entry.id) || entry.sprite == null)
                    continue;

                string key = IconId.Normalize(entry.id);
                if (lookup.ContainsKey(key))
                {
                    Debug.LogWarning($"IconRegistry: Duplicate icon id '{key}'.");
                    continue;
                }

                lookup.Add(key, entry.sprite);
            }
        }

        [System.Serializable]
        public class IconRegistryEntry
        {
            public string id;
            public Sprite sprite;
        }
    }
}
