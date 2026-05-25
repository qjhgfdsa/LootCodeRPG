using System.Collections.Generic;
using UnityEngine;


namespace SA
{
    public class StaticFunctions
    {
        public static void DeepCopyWeapon(Weapon from, Weapon to)
        {
            to.icon = from.icon;
            to.oh_idle = from.oh_idle;
            to.th_idle = from.th_idle;

            to.actions = new List<Action>();
            for (int i = 0; i < from.actions.Count; i++)
            {
                Action a = new Action();
                a.weaponStats = new WeaponStats();
                DeepCopyActionToAction(a, from.actions[i]);
                to.actions.Add(a);
            }

            to.two_handedActions = new List<Action>();
            for (int i = 0; i < from.two_handedActions.Count; i++)
            {
                Action a = new Action();
                a.weaponStats = new WeaponStats();
                DeepCopyActionToAction(a, from.two_handedActions[i]);
                to.two_handedActions.Add(a);
            }

            to.parryMultiplier = from.parryMultiplier;
            to.backstabMultiplier = from.backstabMultiplier;
            to.LeftHandMirror = from.LeftHandMirror;
            to.modelPrefab = from.modelPrefab;
            to.l_model_pos = from.l_model_pos;
            to.l_model_eulers = from.l_model_eulers;
            to.r_model_pos = from.r_model_pos;
            to.r_model_eulers = from.r_model_eulers;
            to.model_scale = from.model_scale;
        }

        public static void DeepCopyActionToAction(Action to, Action from)
        {
            to.fristStep = new ActionAnim();
            to.fristStep.input = from.fristStep.input;
            to.fristStep.targetAnim = from.fristStep.targetAnim;

            to.actionType = from.actionType;
            to.spellClass = from.spellClass;
            to.mirror = from.mirror;
            to.canParry = from.canParry;
            to.canBeParried = from.canBeParried;
            to.changeSpeed = from.changeSpeed;
            to.animSpeed = from.animSpeed;
            to.canBackStab = from.canBackStab;
            to.staminaCost = from.staminaCost;
            to.focusCost = from.focusCost;
            to.overrideDamageAnim = from.overrideDamageAnim;
            to.damageAnim = from.damageAnim;

            DeepCopyStepList(from, to);
        }
        public static void DeepCopyStepList(Action from, Action to)
        {

            to.comboSteps = new List<ActionAnim>();

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


        public static void DeepCopyAction(Weapon w, ActionInput inp, ActionInput assing, List<Action> actionList, bool isLeftHand = false)
        {
            Action a = GetAction(assing, actionList);
            Action w_a = w.GetAction(w.actions, inp);
            if (w_a == null)
            {
                Debug.Log("no weapon action found");
                return;
            }

            DeepCopyStepList(w_a, a);
         
            a.actionType = w_a.actionType;
            a.spellClass = w_a.spellClass;
            a.canBeParried = w_a.canBeParried;
            a.changeSpeed = w_a.changeSpeed;
            a.animSpeed = w_a.animSpeed;
            a.canBackStab = w_a.canBackStab;
            a.staminaCost = w_a.staminaCost;
            a.focusCost = w_a.focusCost;
            a.overrideDamageAnim = w_a.overrideDamageAnim;
            a.damageAnim = w_a.damageAnim;
            a.parryMultiplier = w.parryMultiplier;
            a.backstabMultiplier = w.backstabMultiplier;

            if (isLeftHand)
            {
                a.mirror = true;
            }

        }

        public static void DeepCopyWeaponStats(WeaponStats from, WeaponStats to)
        {
            if (from == null || to == null)
            {
                Debug.Log(to.weaponId + "WeaponStats is null");
                return;
            }
            to.weaponId = from.weaponId;
            to.physical = from.physical;
            to.slash = from.slash;
            to.strike = from.strike;
            to.thrust = from.thrust;
            to.magic = from.magic;
            to.lightning = from.lightning;
            to.fire = from.fire;
            to.dark = from.dark;
        }

        public static Action GetAction(ActionInput input, List<Action> actionSlots)
        {

            for (int i = 0; i < actionSlots.Count; i++)
            {
                if (actionSlots[i].GetfirstInput() == input)
                    return actionSlots[i];
            }

            return null;
        }

        public static void DeepCopySpell(Spell from, Spell to)
        {
            to.itemName = from.itemName;
            to.itemDescription = from.itemDescription;
            to.icon = from.icon;
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
    }
}