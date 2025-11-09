using System.IO;
using UnityEngine;

namespace SA.Utilities
{
    [ExecuteInEditMode]
    public class IconMaker : MonoBehaviour
    {
        
        public bool create;
        public RenderTexture ren;
        public Camera bakeCam;

        public string spriteName;

        void Update()
        {
            if (create)
            {
                create = false;
                CreateIcon();        
            }

        }

        void CreateIcon()
        {
            if (string.IsNullOrEmpty(spriteName))
            {
                spriteName = "icon";
            }

            string path = SaveLocation();
            path += spriteName;

            bakeCam.targetTexture = ren;

            RenderTexture currentRT = RenderTexture.active;
            bakeCam.targetTexture.Release();
            RenderTexture.active = bakeCam.targetTexture;
            bakeCam.Render();

            Texture2D imgPng = new Texture2D(bakeCam.targetTexture.width, bakeCam.targetTexture.height, TextureFormat.ARGB32, false);
            imgPng.ReadPixels(new Rect(0, 0, bakeCam.targetTexture.width, bakeCam.targetTexture.height), 0, 0);
            imgPng.Apply();
            RenderTexture.active = currentRT;
            byte[] bytesPng = imgPng.EncodeToPNG();
            System.IO.File.WriteAllBytes(path + ".png", bytesPng);

            RenderTexture.active = currentRT;

            byte[] bytes = imgPng.EncodeToPNG();
            File.WriteAllBytes(path + ".png", bytes);

            Debug.Log("Icon saved to: " + path + ".png");
        }
        
        string SaveLocation()
        {
            string SaveLocation = Application.dataPath + "/Resources/ItemIcons/";

            if (!Directory.Exists(SaveLocation))
            {
                Directory.CreateDirectory(SaveLocation);
            }
            
            return SaveLocation;
        }
        
    }
}
