using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SA
{
    public class GesturesManager : MonoBehaviour
    {
        public List<GestureContainer> gestures = new List<GestureContainer>();
        Dictionary<string, int> gestures_dict = new Dictionary<string, int>();
        public GameObject gestureGrid;
        public GameObject gestureIconTemplate;
        public RectTransform gestureSelector;

        public int index;

        void Start()
        {
            CreateGesturesUI();
        }

        public void SelectGesture(bool pos)
        {
            if (pos)
            {
                index++;
            }
            else
            {
                index--;
            }
            if (index < 0)
                index = gestures.Count - 1;
            if (index > gestures.Count - 1)
                index = 0;

            IconBase i = gestures[index].baseIcon;
            gestureSelector.transform.SetParent(i.icon.transform);
            gestureSelector.anchoredPosition = Vector2.zero;
         
        }

        public void HandleGestures(bool isOpen)
        {
            if (isOpen)
            {
                if (gestureGrid.activeInHierarchy == false)
                {
                    gestureGrid.SetActive(true);
                    gestureSelector.gameObject.SetActive(true);
                }

            }
            else
            {
                if (gestureGrid.activeInHierarchy)
                {
                    gestureGrid.SetActive(false);
                    gestureSelector.gameObject.SetActive(false);
                }
            }
        }

        void CreateGesturesUI()
        {
            for (int i = 0; i < gestures.Count; i++)
            {
                GameObject go = Instantiate(gestureIconTemplate) as GameObject;
                go.transform.SetParent(gestureGrid.transform);
                go.transform.localScale = Vector3.one;
                go.SetActive(true);
                IconBase icon = go.GetComponentInChildren<IconBase>();
                icon.icon.sprite = SA.UI.Icons.IconRegistry.Instance.Resolve(new IconId(gestures[i].icon)); // รอเช็ค
                icon.id = gestures[i].targetAnim;
                gestures[i].baseIcon = icon;

            }
            gestureIconTemplate.SetActive(false);
            gestureSelector.gameObject.SetActive(false);
        }

        public GestureContainer GetGesture(string id)
        {
            int index = -1;
            if (gestures_dict.TryGetValue(id, out index))
            {
                return gestures[index];
            }
            return null;
        }
        public static GesturesManager singleton;
        void Awake()
        {
            singleton = this;

            for (int i = 0; i < gestures.Count; i++)
            {
                if (gestures_dict.ContainsKey(gestures[i].targetAnim))
                {
                    Debug.Log(gestures[i].targetAnim + " is a duplicate");
                }
                else
                {
                    gestures_dict.Add(gestures[i].targetAnim, i);
                }
            }
        }

        [System.Serializable]
        public class GestureContainer
        {
            public string icon;
            public string targetAnim;
            public IconBase baseIcon;
        }

    }
}
