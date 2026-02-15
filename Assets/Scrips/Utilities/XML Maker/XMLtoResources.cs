using UnityEngine;
using SA;
using System.Xml;
using System.IO;
using System.Collections.ObjectModel;

namespace SA.Utilities
{
    [ExecuteInEditMode]
    public class XMLtoResources : MonoBehaviour
    {
        public bool load;

        public ResourcesManager resourcesManager;
        public string weaponFileName = "items_database.xml";

        void Update()
        {
            if (!load)
                return;

            load = false;

            LoadWeaponData(resourcesManager);
        }

        public void LoadWeaponData(ResourcesManager rm)
        {
            string filePath = StaticStrings.SaveLocation() + StaticStrings.itemFolder;
            filePath += weaponFileName;

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            foreach (XmlNode w in doc.DocumentElement.SelectNodes("//weapon"))
            {
                Weapon _w = new Weapon();
                XmlNode weaponId = w.SelectSingleNode("weaponId");
                _w.weaponId = weaponId.InnerText;

                XmlNode oh_idle = w.SelectSingleNode("oh_idle");
                _w.oh_idle = oh_idle.InnerText;

                XmlNode th_idle = w.SelectSingleNode("th_idle");
                _w.th_idle = th_idle.InnerText;

                XmlNode parryMultiplier = w.SelectSingleNode("parryMultiplier");
                float.TryParse(parryMultiplier.InnerText, out _w.parryMultiplier);
                XmlNode backstabMultiplier = w.SelectSingleNode("backstabMultiplier");
                float.TryParse(backstabMultiplier.InnerText, out _w.backstabMultiplier);

                XmlToActions(doc, "actions", ref _w);
                XmlToActions(doc, "act", ref _w);

                XmlNode LeftHandMirror = w.SelectSingleNode("LeftHandMirror");
                _w.LeftHandMirror = (LeftHandMirror.InnerText == "True");


            }
        }

        void XmlToActions(XmlDocument doc,string nodeName, ref Weapon _w)
        {
            foreach (XmlNode a in doc.DocumentElement.SelectNodes("//" + nodeName))
            {

            }
        }
    }
}