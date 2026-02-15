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
                xml += "<model_pos>" + w.model_pos + "</model_pos>" + "\n";
                xml += "<model_eulers>" + w.model_eulers + "</model_eulers>" + "\n";
                xml += "<model_scale>" + w.model_scale + "</model_scale>" + "\n";

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
                xml += "<ActionsInput>" + a.input.ToString() + "</ActionsInput>" + "\n";
                xml += "<ActionsType>" + a.actionType.ToString() + "</ActionsType>" + "\n";
                xml += "<targetAnim>" + a.targetAnim + "</targetAnim>" + "\n";
                xml += "<mirror>" + a.mirror + "</mirror>" + "\n";
                xml += "<canBeParried>" + a.canBeParried + "</canBeParried>" + "\n";
                xml += "<changeSpeed>" + a.changeSpeed + "</changeSpeed>" + "\n";
                xml += "<animSpeed>" + a.animSpeed.ToString() + "<animSpeed>" + "\n";
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
