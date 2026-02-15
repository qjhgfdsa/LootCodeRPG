using System.Collections.Generic;
using SA;
using UnityEngine;

public class ResourcesManager : MonoBehaviour
{
    public List<Weapon> weaponList = new List<Weapon>();



    public static ResourcesManager singleton;
    void Awake()
    {
        singleton = this;
    }
   
    
}
