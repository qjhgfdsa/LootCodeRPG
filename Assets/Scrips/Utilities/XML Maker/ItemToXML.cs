using UnityEngine;
using SA;
using System.Collections.Generic;
using System.IO;

namespace SA.Utilities
{
    [ExecuteInEditMode]
    public class ItemToXML : MonoBehaviour
    {
        public bool make;
        public List<ItemInstance> canidates = new List<ItemInstance>();
        public string xml_version;
        public string targetName;
        void Update()
        {
            if (!make)
                return;
            make = false;

            string xml = xml_version; // <?xml version = "1.0" encoding = "UTF-8"?>
            xml += "\n";
            xml += "<root>";
            foreach (ItemInstance i in canidates)
            {

                Weapon w = i.instance;

                xml += "<weapon>" + "\n";
                xml += "<weaponId>" + w.weaponId + "</weaponId>" + "\n";
                xml += "<oh_idle>" + w.oh_idle + "</oh_idle>" + "\n";
                xml += "<th_idle>" + w.th_idle + "</th_idle>" + "\n";

                xml += ActionListToString(w.actions, "action");
                xml += ActionListToString(w.two_handedActions, "two_handed");

                xml += "<parryMultiplier>" + w.parryMultiplier + "</parryMultiplier>" + "\n";
                xml += "<backstabMultiplier>" + w.backstabMultiplier + "</backstabMultiplier>" + "\n";
                xml += "<LeftHandMirror>" + w.LeftHandMirror + "</LeftHandMirror>" + "\n";

                xml += "<mp_x>" + w.model_pos.x + "</mp_x>";
                xml += "<mp_y>" + w.model_pos.y + "</mp_y>";
                xml += "<mp_z>" + w.model_pos.z + "</mp_z>" + "\n";

                xml += "<me_x>" + w.model_eulers.x + "</me_x>";
                xml += "<me_y>" + w.model_eulers.y + "</me_y>";
                xml += "<me_z>" + w.model_eulers.z + "</me_z>" + "\n";

                xml += "<ms_x>" + w.model_scale.x + "</ms_x>";
                xml += "<ms_y>" + w.model_scale.y + "</ms_y>";
                xml += "<ms_z>" + w.model_scale.z + "</ms_z>" + "\n";

                xml += "</weapon>" + "\n";
            }

            xml += "</root>";

            string path = StaticStrings.SaveLocation() + StaticStrings.itemFolder;
            if (string.IsNullOrEmpty(targetName))
            {
                targetName = "items_database.xml";
            }

            path += targetName;

            File.WriteAllText(path, xml);
        }

        string ActionListToString(List<Action> l, string nodeName)
        {
            string xml = null;

            foreach (Action a in l)
            {
                xml += "<" + nodeName + ">" + "\n";
                xml += "<ActionInput>" + a.input.ToString() + "</ActionInput>" + "\n";
                xml += "<ActionType>" + a.actionType.ToString() + "</ActionType>" + "\n";
                xml += "<targetAnim>" + a.targetAnim + "</targetAnim>" + "\n";
                xml += "<mirror>" + a.mirror + "</mirror>" + "\n";
                xml += "<canBeParried>" + a.canBeParried + "</canBeParried>" + "\n";
                xml += "<changeSpeed>" + a.changeSpeed + "</changeSpeed>" + "\n";
                xml += "<animSpeed>" + a.animSpeed.ToString() + "</animSpeed>" + "\n";
                xml += "<canParry>" + a.canParry + "</canParry>" + "\n";
                xml += "<canBackStab>" + a.canBackStab + "</canBackStab>" + "\n";
                xml += "<ovverideDamageAnim>" + a.ovverideDamageAnim + "</ovverideDamageAnim>" + "\n";
                xml += "<damageAnim>" + a.damageAnim + "</damageAnim>" + "\n";

                WeaponStats s = a.weaponStats;

                xml += "<physical>" + s.physical + "</physical>" + "\n";
                xml += "<strike>" + s.strike + "</strike>" + "\n";
                xml += "<slash>" + s.slash + "</slash>" + "\n";
                xml += "<thrust>" + s.thrust + "</thrust>" + "\n";
                xml += "<magic>" + s.magic + "</magic>" + "\n";
                xml += "<fire>" + s.fire + "</fire>" + "\n";
                xml += "<lightning>" + s.lightning + "</lightning>" + "\n";
                xml += "<dark>" + s.dark + "</dark>" + "\n";

                xml += "</" + nodeName + ">" + "\n";

            }

            return xml;
        }

    }
}
