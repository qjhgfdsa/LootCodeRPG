using UnityEngine;
using System.Collections.Generic;

namespace SA
{
    public class ItemScriptableObjectScript : ScriptableObject
    {
        public List<Item> cons_items = new List<Item>();
        public List<Item> weapon_items = new List<Item>();
        public List<Item> spell_items = new List<Item>();

    }
}