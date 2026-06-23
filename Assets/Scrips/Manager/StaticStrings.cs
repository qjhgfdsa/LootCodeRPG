using System.IO;
using UnityEngine;

namespace SA
{
    public static class StaticStrings
    {
        // Input Manager Axes and Keys
        public static string Vertical = "Vertical";
        public static string Horizontal = "Horizontal";

        public static string KeyShift = "left shift";   // วิ่ง
        public static string KeySpace = "space";         // กระโดด / ม้วน
        public static string KeyT = "t";                 // สองมือ
        public static string KeyX = "x";                 // ใช้ไอเทม
        public static string KeyTab = "tab";             // lock-on (สำรอง)

        public static string KeyQ = "q";                 // โจมตี Q
        public static string KeyE = "e";                 // โจมตี E
        public static string KeyR = "r";                 // โจมตี R
        public static string KeyF = "f";                 // โจมตี F
        public static string KeyG = "g";                 // เลือกการจัดการคำสั่ง Gesture Menu
        public static string KeyI = "i";                 // เปิด inventory

        //Animation Parameters
        public static string Vertical_Axis = "vertical";
        public static string Horizontal_Axis = "horizontal";
        public static string mirror = "mirror";
        public static string parry_attack = "parry_attack";
        public static string isInteracting = "interacting";
        public static string canMove = "canMove";
        public static string onEmpty = "onEmpty";
        public static string two_handed = "two_handed";
        public static string run = "run";
        public static string blocking = "blocking";
        public static string lockon = "lockOn";
        public static string OnGround = "onGround";
        public static string animSpeed = "animSpeed";
        public static string isLeftHand = "isLeft";
        public static string spellcasting = "spellcasting";
        public static string enabledItem = "enabledItem";

       
        // Animator states
        public static string Rolls = "Rolls";
        public static string attack_interupt = "attack_interrupt";
        public static string parry_receive = "parry_recieved";
        public static string changeWeapon = "changeWeapon";
        public static string backstabed = "backstabbed";
        public static string damage1 = "damage_1";
        public static string damage2 = "damage_2";
        public static string damage3 = "damage_3";
        public static string emptyBoth = "Empty Both";
        public static string emptyLeft = "Empty Left";
        public static string emptyRight = "Empty Right";
        public static string equipWeapon_oh = "equipWeapon_oh";

        //Other
        public static string _l = "_l";
        public static string _r = "_r";

        //Data
        public static string itemFolder = "/Item/";
        public static string SaveLocation()
        {
            string r = Application.streamingAssetsPath;
            if (!Directory.Exists(r))
            {
                Directory.CreateDirectory(r);
            }

            return r;
        }
    }
    
}

