using System.Collections.Generic;
using UnityEngine;


namespace SA
{
    public class StaticFunctions
    {
        public static void DeepCopyWeapon(Weapon from, Weapon to)
        {
          //  to.iconId = from.iconId;
         //   to.icon = from.icon;
            to.Item_id = from.Item_id;
            to.oh_idle = from.oh_idle;
            to.th_idle = from.th_idle;

            to.actions = new List<Action>();
            for (int i = 0; i < from.actions.Count; i++)
            {
                Action a = new Action();
                //a.weaponStats = new WeaponStats();
                DeepCopyActionToAction(a, from.actions[i]);
                to.actions.Add(a);
            }

            to.two_handedActions = new List<Action>();
            for (int i = 0; i < from.two_handedActions.Count; i++)
            {
                Action a = new Action();
                // a.weaponStats = new WeaponStats();
                DeepCopyActionToAction(a, from.two_handedActions[i]);
                to.two_handedActions.Add(a);
            }

            to.parryMultiplier = from.parryMultiplier;
            to.backstabMultiplier = from.backstabMultiplier;
            to.LeftHandMirror = from.LeftHandMirror;
            to.modelPrefab = from.modelPrefab;
        }

        public static void DeepCopyActionToAction(Action to, Action from)
        {
            to.fristStep = new ActionAnim();
            to.fristStep.input = from.fristStep.input;
            to.fristStep.targetAnim = from.fristStep.targetAnim;

            to.comboSteps = new List<ActionAnim>();

            to.actionType = from.actionType;
            to.spellClass = from.spellClass;
          //  to.mirror = from.mirror;
            to.canParry = from.canParry;
            to.canBeParried = from.canBeParried;
            to.changeSpeed = from.changeSpeed;
            to.animSpeed = from.animSpeed;
            to.canBackStab = from.canBackStab;
           // to.staminaCost = from.staminaCost;
          //  to.focusCost = from.focusCost;
            to.overrideDamageAnim = from.overrideDamageAnim;
            to.damageAnim = from.damageAnim;
            to.overrideKick = from.overrideKick;
            to.kickAnim = from.kickAnim;

            DeepCopyStepList(from, to);
        }
        public static void DeepCopyStepList(Action from, Action to)
        {

            if (from.comboSteps == null)
                return;

            for (int i = 0; i < from.comboSteps.Count; i++)
            {
                ActionAnim a = new ActionAnim();
                a.input = from.comboSteps[i].input;
                a.targetAnim = from.comboSteps[i].targetAnim;
                to.comboSteps.Add(a);
            }
        }


        public static bool DeepCopyAction(Weapon w, ActionInput inp, ActionInput assing, List<Action> actionList, bool isLeftHand = false)
        {
            if (w == null || actionList == null)
                return false;

            Action a = GetAction(assing, actionList);
            if (a == null)
            {
                Debug.LogWarning("no action slot found for " + assing);
                return false;
            }

            Action from = w.GetAction(w.actions, inp);
            if (from == null)
                return false;

            if (from.fristStep == null)
            {
                Debug.LogWarning("weapon action has no first step for " + inp);
                return false;
            }

            a.fristStep.targetAnim = from.fristStep.targetAnim;
            a.comboSteps = new List<ActionAnim>();
            DeepCopyStepList(from, a);

            a.actionType = from.actionType;
            a.spellClass = from.spellClass;
            a.canBeParried = from.canBeParried;
            a.changeSpeed = from.changeSpeed;
            a.animSpeed = from.animSpeed;
            a.canBackStab = from.canBackStab;
           // a.staminaCost = from.staminaCost;
           // a.focusCost = from.focusCost;
            a.overrideDamageAnim = from.overrideDamageAnim;
            a.damageAnim = from.damageAnim;
            a.parryMultiplier = w.parryMultiplier;
            a.backstabMultiplier = w.backstabMultiplier;
            a.mirror = isLeftHand;

            a.overrideKick = from.overrideKick;
            a.kickAnim = from.kickAnim;

            return true;
        }

      /*  public static void DeepCopyAction(Weapon w, ActionInput inp, ActionInput assing, List<Action> actionList, bool isLeftHand = false)
        {
           if (!TryDeepCopyAction(w, inp, assing, actionList, isLeftHand))
                Debug.Log("no weapon action found for " + inp);
        } */

        public static void DeepCopyWeaponStats(WeaponStats from, WeaponStats to)
        {
            if (from == null || to == null)
            {
                Debug.Log(to.weaponId + "WeaponStats is null");
                return;
            }
            to.weaponId = from.weaponId;
            to.a_physical = from.a_physical;
            to.a_slash = from.a_slash;
            to.a_strike = from.a_strike;
            to.a_thrust = from.a_thrust;
            to.a_magic = from.a_magic;
            to.a_lightning = from.a_lightning;
            to.a_fire = from.a_fire;
            to.a_dark = from.a_dark;
            to.a_frost = from.a_frost;
            to.a_curse = from.a_curse;
            to.a_poison = from.a_poison;
            to.critical = from.critical;
            to.d_physical = from.d_physical;
            to.d_strike = from.d_strike;
            to.d_slash = from.d_slash;
            to.d_thrust = from.d_thrust;
            to.d_magic = from.d_magic;
            to.d_lightning = from.d_lightning;
            to.d_fire = from.d_fire;
            to.d_dark = from.d_dark;
            to.d_frost = from.d_frost;
            to.d_curse = from.d_curse;
            to.d_poison = from.d_poison;
            to.stability = from.stability;
        }

        public static Action GetAction(ActionInput input, List<Action> actionSlots)
        {
            if (actionSlots == null)
                return null;

            int index = (int)input;
            if (index >= 0 && index < actionSlots.Count)
                return actionSlots[index];

            for (int i = 0; i < actionSlots.Count; i++)
            {
                if (actionSlots[i].GetfirstInput() == input)
                    return actionSlots[i];
            }

            return null;
        }

        public static void DeepCopySpell(Spell from, Spell to)

        {
            to.Item_id = from.Item_id;
           // to.itemDescription = from.itemDescription;
          //  to.iconId = from.iconId;
          //  to.icon = from.icon;
            to.spellType = from.spellType;
            to.spellClass = from.spellClass;
            to.projecttile = from.projecttile;
            to.spell_effect = from.spell_effect;
            to.particlePrefab = from.particlePrefab;

            to.actions = new List<SpellAction>();
            for (int i = 0; i < from.actions.Count; i++)
            {
                SpellAction a = new SpellAction();
                DeepCopySpellAction(a, from.actions[i]);
                to.actions.Add(a);
            }
        }
        public static void DeepCopySpellAction(SpellAction to, SpellAction from)
        {
            to.input = from.input;
            to.targetAnim = from.targetAnim;
            to.throwAnim = from.throwAnim;
            to.castTime = from.castTime;
            to.staminaCost = from.staminaCost;
            to.focusCost = from.focusCost;
        }
        public static void DeepCopyConsumable(Consumable from, Consumable to)
        {
            to.consumableEffect = from.consumableEffect;
            to.targetAnim = from.targetAnim;
            to.Item_id = from.Item_id;
            to.itemPrefab = from.itemPrefab;
        }
    }
}