using System.Collections.Generic;
using UnityEngine;


namespace SA
{
    public class WeaponScriptableObject : ScriptableObject
    {
        public List<Weapon> weapons_all = new List<Weapon>();
        public List<WeaponStats> weaponStats_all = new List<WeaponStats>();

        [Header("Debug")]
        public bool debugFirstStepAnimations = true;

        static WeaponScriptableObject cached;

        public static bool IsFirstStepDebugEnabled()
        {
            if (cached == null)
                cached = Resources.Load<WeaponScriptableObject>("SA.WeaponScriptableObject");

            return cached != null && cached.debugFirstStepAnimations;
        }

        [ContextMenu("Debug FirstStep Animations")]
        void DebugLogAllFirstSteps()
        {
            DebugLogFirstSteps(weapons_all);
        }

        public static void DebugLogFirstSteps(List<Weapon> weapons)
        {
            if (weapons == null)
            {
                Debug.LogWarning("[WeaponSO] weapons list is null");
                return;
            }

            Debug.Log($"[WeaponSO] FirstStep audit — {weapons.Count} weapon(s)");

            for (int i = 0; i < weapons.Count; i++)
                LogWeaponFirstSteps(weapons[i]);
        }

        static void LogWeaponFirstSteps(Weapon weapon)
        {
            if (weapon == null)
            {
                Debug.LogWarning("[WeaponSO] null weapon entry in list");
                return;
            }

            LogActionList(weapon.Item_id, "one-handed", weapon.actions);
            LogActionList(weapon.Item_id, "two-handed", weapon.two_handedActions);
        }

        static void LogActionList(string weaponId, string handLabel, List<Action> actions)
        {
            if (actions == null || actions.Count == 0)
            {
                Debug.LogWarning($"[WeaponSO] {weaponId} | {handLabel}: no actions");
                return;
            }

            for (int i = 0; i < actions.Count; i++)
            {
                Action action = actions[i];
                if (action == null)
                    continue;

                ActionInput input = action.GetfirstInput();

                if (action.fristStep == null)
                {
                    Debug.LogWarning($"[WeaponSO] {weaponId} | {handLabel} | {input}: fristStep NULL");
                    continue;
                }

                if (string.IsNullOrEmpty(action.fristStep.targetAnim))
                    Debug.LogWarning($"[WeaponSO] {weaponId} | {handLabel} | {input}: targetAnim EMPTY");
                else
                    Debug.Log($"[WeaponSO] {weaponId} | {handLabel} | {input}: {action.fristStep.targetAnim}");
            }
        }

        public static void LogPlayingFirstStep(string weaponId, ActionInput input, int stepIndex, string targetAnim)
        {
            if (!IsFirstStepDebugEnabled())
                return;

            string id = string.IsNullOrEmpty(weaponId) ? "?" : weaponId;

            if (string.IsNullOrEmpty(targetAnim))
                Debug.LogWarning($"[WeaponSO Play] {id} | {input} step[{stepIndex}]: EMPTY — ลืมใส่ animation?");
            else
                Debug.Log($"[WeaponSO Play] {id} | {input} step[{stepIndex}]: {targetAnim}");
        }
    }
}
