using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;


namespace SA
{
    public class StateManager : MonoBehaviour
    {
        [Header("Model")]
        public GameObject activeModel;

        [Header("Stats")]
        public Attributes attributes;
        public CharacterStats characterStats;


        [Header("Inputs")]
        public float vertical;
        public float horizontal;

        public Vector3 moveDir;
        public bool r, f, q, e;
        public bool rollInput;
        public bool itemInput;
        public bool itemInputPending;



        [Header("Move Speed")]
        public float moveSpeed = 2;
        public float runSpeed = 3.5f;
        public float rotateSpeed = 5;
        public float toGround = 0.5f;
        public float rollSpeed = 1;
        public float parryOffset = 1.4f;
        public float backstabOffset = 1.4f;

        [Header("States")]
        public bool run;
        public bool onGround;
        public bool lockOn;
        public bool inAction;
        public bool canMove;
        public bool damageIsOn;
        public bool canRotate;
        public bool canAttack;
        public bool isSpellCasting;
        public bool enableIK;
        public bool isTwoHanded;
        public bool usingItem;
        public bool isBlocking;
        public bool isLeftHand;
        public bool canBeParried;
        public bool parryIsOn;
        public bool onEmpty;
        public bool closeWeapons;



        [Header("Other")]
        public EnemyTarget lockOnTarget;
        public Transform lockOnTransform;
        public AnimationCurve roll_curve;
        //public EnemyStates parryTarget;


        [HideInInspector]
        public Animator anim;
        [HideInInspector]
        public Rigidbody rigid;
        [HideInInspector]
        public AnimatorHook a_hook;
        [HideInInspector]
        public ActionManager actionManager;
        [HideInInspector]
        public InventoryManager inventoryManager;
        [HideInInspector]
        public PickableItemsManager pickManager;



        [HideInInspector]
        public float delta;
        [HideInInspector]
        public LayerMask ignoreLayers;

        [HideInInspector]
        public Action currentAction;


        [HideInInspector]
        public float airTimer;
        public ActionInput storePrevInput;
        public ActionInput storeActionInput;


        float actionDelay;
        float kickTimer;
        public bool canKick;
        public bool holdKick;
        public float moveAmount;
        public float kickMaxTime = 0.5f;
        public float moveAmountThreshold = 0.05f;

        public bool enabledItem;

        public void Init()
        {
            SetupAnimator();

            // เช็คว่า anim พร้อมหรือยัง
            if (anim == null)
            {
                Debug.LogError("❌ Animator is NULL after SetupAnimator!");
                return;
            }

            rigid = GetComponent<Rigidbody>();
            rigid.angularDamping = 999;
            rigid.linearDamping = 4;
            rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            inventoryManager = GetComponent<InventoryManager>();
            actionManager = GetComponent<ActionManager>();


            // ===== สำคัญ: ต้อง Init a_hook ก่อน inventoryManager =====
            a_hook = activeModel.GetComponent<AnimatorHook>();
            if (a_hook == null)
                a_hook = activeModel.AddComponent<AnimatorHook>();
            a_hook.Init(this, null);

            // ตอนนี้ a_hook พร้อมแล้ว ถึงค่อย Init inventoryManager
            inventoryManager.Init(this);
            actionManager.Init(this);

            gameObject.layer = 8;
            ignoreLayers = ~(1 << 9);

            anim.SetBool(StaticStrings.OnGround, true);

            pickManager = GetComponent<PickableItemsManager>();

            characterStats.InitCurrent();
            if (UIManager.singleton != null)
                UIManager.singleton.AffectAll(characterStats.hp, characterStats.fp, characterStats.stamina);

            UIManager.singleton.InitSouls(characterStats._souls);
            prevGround = true;
        }
        void SetupAnimator()
        {
            if (activeModel == null)
            {
                anim = GetComponentInChildren<Animator>();
                if (anim == null)
                {
                    Debug.Log("No model found for " + gameObject.name);
                }
                else
                {
                    activeModel = anim.gameObject;
                }
            }

            if (anim == null)
                anim = activeModel.GetComponent<Animator>();

            anim.applyRootMotion = false;

            anim.GetBoneTransform(HumanBodyBones.LeftHand).localScale = Vector3.one;
            anim.GetBoneTransform(HumanBodyBones.RightHand).localScale = Vector3.one;
        }
        public void FixedTick(float d)
        {
            delta = d;

            if (anim == null || inventoryManager == null)
            {
                Debug.LogError("anim หรือ inventoryManager เป็น null!");
                return;
            }

            if (anim == null || a_hook == null)
            {
                Debug.LogError("❌ anim หรือ a_hook ยัง NULL!");
                return;
            }

            isBlocking = false;
            itemInput = itemInputPending;
            usingItem = anim.GetBool(StaticStrings.isInteracting);
            enabledItem = anim.GetBool(StaticStrings.enabledItem);
            anim.SetBool(StaticStrings.spellcasting, isSpellCasting);

            if (!closeWeapons)
            {
                GameObject mm = null;

                if (inventoryManager != null &&
                    inventoryManager.rightHandWeapon != null &&
                    inventoryManager.rightHandWeapon.weaponModel != null)
                {
                    mm = inventoryManager.rightHandWeapon.weaponModel;
                    //inventoryManager.rightHandWeapon.weaponModel.SetActive(!usingItem);
                }


                if (mm == null)
                {
                    if (inventoryManager.leftHandWeapon != null)
                        mm = inventoryManager.leftHandWeapon.weaponModel;
                }

                if (mm != null)
                {
                    mm.SetActive(!usingItem);
                }
                //เเก้เอง
                if (!isTwoHanded)
                {
                    if (inventoryManager.leftHandWeapon != null && inventoryManager.rightHandWeapon != null)
                        inventoryManager.leftHandWeapon.weaponModel.SetActive(true);
                }



                if (inventoryManager.currentConsumable != null)
                {
                    if (inventoryManager.currentConsumable.itemModel != null)
                        inventoryManager.currentConsumable.itemModel.SetActive(enabledItem);
                }

            }
            else
            {
                if (inventoryManager.rightHandWeapon != null)
                    inventoryManager.rightHandWeapon.weaponModel.SetActive(false);
                if (inventoryManager.leftHandWeapon != null)
                    inventoryManager.leftHandWeapon.weaponModel.SetActive(false);
            }

            if (isBlocking == false && isSpellCasting == false)
            {
                enableIK = false;
            }

            //a_hook.useIk = true; สำหรับปรับการใช้ IK

            if (inAction)
            {
                anim.applyRootMotion = true;

                actionDelay += delta;
                if (actionDelay > 0.6f)
                {
                    inAction = false;
                    actionDelay = 0;
                }
                else
                {
                    return;
                }
            }

            onEmpty = anim.GetBool(StaticStrings.onEmpty);
            //anim.applyRootMotion = !onEmpty;
            //canMove = anim.GetBool(StaticStrings.canMove)

            if (onEmpty)
            {
                canAttack = true;
                canMove = true;
                actionManager.actionIndex = 0;
            }
            if (canRotate)
            {
                HandleRotation();
            }

            if (!onEmpty && !canMove && !canAttack)//aniamtion is playing
                return;

            closeWeapons = false;

            if (canMove && !onEmpty)
            {
                if (moveAmount > 0.3f)
                {
                    anim.CrossFade("Empty Override", 0.1f);
                    onEmpty = true;
                }
            }

            MoitorKick();

            if (canAttack)
            {
                DetectAction();
            }

            if (canMove || itemInput)
                DetectItemAction();

            itemInput = false;

            anim.applyRootMotion = false;

            // ตรงนี้ใน FixedTick() ก่อนเรียก a_hook
            if (a_hook == null)
            {
                Debug.LogError("❌ a_hook is NULL!");
                return;
            }
            rigid.linearDamping = (moveAmount > 0 || onGround == false) ? 0 : 4;
            float targetSpeed = moveSpeed;
            if (usingItem || isSpellCasting)
            {
                run = false;
                moveAmount = Mathf.Clamp(moveAmount, 0, 0.45f);
            }

            if (run)
                targetSpeed = runSpeed;

            if (onGround && canMove)
                rigid.linearVelocity = moveDir * (targetSpeed * moveAmount);

            if (run)
                lockOn = false;

            HandleRotation();

            anim.SetBool(StaticStrings.lockon, lockOn);

            if (!lockOn)
            {
                HandleMovementAnimations();
            }
            else
            {
                if (lockOnTransform != null)
                    HandleLockOnAnimations(moveDir);
                else
                    // ถ้าไม่มี lockOnTransform ให้ใช้ animation ปกติ
                    HandleMovementAnimations();
            }

            a_hook.useIk = enableIK;
            // anim.SetBool(StaticStrings.blocking, isBlocking);
            anim.SetBool(StaticStrings.isLeftHand, isLeftHand);

            HanddleBlocking();

            if (isSpellCasting)
            {
                HandleSpellCasting();
                return;
            }
            a_hook.CloseRoll();
            HandleRolls();
        }
        public bool IsInput()
        {
            if (r || f || q || e || rollInput)
                return true;

            return false;
        }
        void HandleRotation()
        {
            Vector3 targetDir;
            if (lockOn && lockOnTransform != null)
            {
                targetDir = lockOnTransform.position - transform.position;
            }
            else
            {
                targetDir = moveDir;
            }

            targetDir.y = 0;
            if (targetDir == Vector3.zero)
                targetDir = transform.forward;
            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, delta * moveAmount * rotateSpeed);
            transform.rotation = targetRotation;
        }

        public void DetectItemAction()
        {
            if (!onEmpty || usingItem || isBlocking || isSpellCasting)
                return;

            if (!itemInput)
                return;

            if (inventoryManager.currentConsumable == null)
                return;

            if (inventoryManager.currentConsumable.itemCount < 1 && inventoryManager.currentConsumable.unlimitedCount == false)
                return;

            RuntimeConsumable slot = inventoryManager.currentConsumable;
            if (slot == null || slot.instance == null)
            {
                itemInputPending = false;
                return;
            }

            string targetAnim = slot.instance.targetAnim;

            if (string.IsNullOrEmpty(targetAnim))
            {
                itemInputPending = false;
                return;
            }


            usingItem = true;
            itemInputPending = false;//เพิ่มเองไม่มีในสอนวิดีโอ
            anim.Play(targetAnim);
        }

        public void DetectAction()
        {
            if (canAttack == false && (!onEmpty || usingItem || isSpellCasting))
                return;

            if (!f && !r && !e && !q)
                return;

            ActionInput targetInput = actionManager.GetActionInput(this);
            storeActionInput = targetInput;

            if (!onEmpty)
            {
                a_hook.CloseRoll();
                targetInput = storePrevInput;
            }

            storePrevInput = targetInput;
            Action slot = actionManager.GetActionFromInput(targetInput);

            if (slot == null)
                return;

            switch (slot.actionType)
            {
                case ActionType.attack:
                    AttackAction(slot);
                    break;
                case ActionType.block:
                    BlockAction(slot);
                    break;
                case ActionType.spells:
                    SpellAction(slot);
                    break;
                case ActionType.parry:
                    ParryAction(slot);
                    break;
                default:
                    break;
            }
        }

        void MoitorKick()
        {
            if (!holdKick)
            {
                if (moveAmount > moveAmountThreshold)
                {
                    kickTimer += delta;
                    if (kickTimer < kickMaxTime)
                    {
                        canKick = true;
                    }
                    else
                    {
                        kickTimer = kickMaxTime;
                        holdKick = true;
                        canKick = false;
                    }
                }
                else
                {

                    kickTimer -= delta * 0.5f;
                    if (kickTimer < 0)
                    {
                        kickTimer = 0;
                        canKick = false;
                    }
                }
            }
            else
            {
                if (moveAmount < moveAmountThreshold)
                {
                    kickTimer -= delta;
                    if (kickTimer < 0)
                    {
                        kickTimer = 0;
                        holdKick = false;
                        canKick = false;
                    }
                }
            }
        }
        void AttackAction(Action slot)
        {
            if (characterStats._stamina < slot.staminaCost)
            {
                Debug.Log("Not enough stamina");
                return;
            }

            if (CheckForParry(slot))
                return;

            if (CheckForBackstab(slot))
                return;

            if (slot.fristStep.input == ActionInput.f)
            {
                if (canKick)
                {
                    string kickAnim = "kick 1";
                    if (slot.overrideKick)
                        kickAnim = slot.kickAnim;

                    PlayAnimation(kickAnim, false);
                    kickTimer = 0;
                    Debug.Log("Kick 1 เฉพาะอยู่กับที่นั้นเท่านั้น");
                    return;
                }
            }

            //เเก้เอง
            int stepIndex = actionManager.actionIndex;
            ActionAnim step = slot.GetActionStep(ref actionManager.actionIndex);
            string targetAnim = step.targetAnim;

            Weapon currentWeapon = inventoryManager.GetCurrentWeapon(actionManager.IsLeftHandslot(slot));
            WeaponScriptableObject.LogPlayingFirstStep(currentWeapon?.Item_id, slot.GetfirstInput(), stepIndex, targetAnim);
            //ถึงตรงนี้



            if (string.IsNullOrEmpty(targetAnim))
                return;

            currentAction = slot;

            canRotate = false;


            float targetSpeed = 1;
            if (slot.changeSpeed)
            {
                targetSpeed = slot.animSpeed;
                if (targetSpeed == 0)
                    targetSpeed = 1;
            }

            canBeParried = slot.canBeParried;
            anim.SetFloat(StaticStrings.animSpeed, targetSpeed);
            PlayAnimation(targetAnim, slot.mirror);
            characterStats._stamina -= slot.staminaCost;
        }

        void SpellAction(Action slot)
        {
            if (characterStats._stamina < slot.staminaCost)
            {
                Debug.Log("Not enough stamina");
                return;
            }

            if (slot.spellClass != inventoryManager.currentSpell.instance.spellClass ||
           characterStats._focus < slot.focusCost)
            {
                PlayAnimation("cant_spell", slot.mirror);
                return;
            }

            ActionInput inp = actionManager.GetActionInput(this);
            if (inp == ActionInput.e)
                inp = ActionInput.f;
            if (inp == ActionInput.q)
                inp = ActionInput.r;

            Spell s_inst = inventoryManager.currentSpell.instance;
            SpellAction s_slot = s_inst.GetAction(s_inst.actions, inp);
            if (s_slot == null)
            {
                Debug.Log("Spell action not found");
                return;
            }

            SpellEffectManager.singleton.UseSpellEffect(s_inst.spell_effect, this);


            isSpellCasting = true;
            spellCastingTime = 0;
            max_spellCastTime = s_slot.castTime;
            spellTargetAnim = s_slot.throwAnim;
            spellIsMirrored = slot.mirror;
            curSpellType = s_inst.spellType;

            string targetAnim = s_slot.targetAnim;
            if (spellIsMirrored)
                targetAnim += StaticStrings._l;
            else
                targetAnim += StaticStrings._r;

            projectileCanidate = inventoryManager.currentSpell.instance.projecttile;
            inventoryManager.CreateSpellParticle(inventoryManager.currentSpell, spellIsMirrored, (s_inst.spellType == SpellType.looping));
            anim.SetBool(StaticStrings.spellcasting, true);
            anim.SetBool(StaticStrings.mirror, spellIsMirrored);//เพิ่มเพื่อทำให้ตรงกับการทำงานของฟังก์ชั่น HandleSpellCasting() ทดลองเปลี่ยนค่าเป็น spellIsMirrored
            anim.CrossFade(targetAnim, 0.2f);

            cur_staminaCost = s_slot.staminaCost;
            cur_focusCost = s_slot.focusCost;

            a_hook.InitIKForBreathSpell(spellIsMirrored);

            if (spellCast_start != null)
                spellCast_start();
        }
        public void PlayAnimation(string targetAnim, bool isMirrored)
        {
            canAttack = false;
            onEmpty = false;
            canMove = false;
            inAction = true;
            canKick = false;
            canRotate = false;
            anim.SetBool(StaticStrings.mirror, isMirrored);
            anim.CrossFade(targetAnim, 0.2f);
        }
        float cur_focusCost;
        float cur_staminaCost;
        float spellCastingTime;
        float max_spellCastTime;
        string spellTargetAnim;
        bool spellIsMirrored;
        SpellType curSpellType;
        GameObject projectileCanidate;

        public delegate void SpellCast_Start();
        public delegate void SpellCast_Loop();
        public delegate void SpellCast_Stop();
        public SpellCast_Start spellCast_start;
        public SpellCast_Loop spellCast_loop;
        public SpellCast_Stop spellCast_stop;

        void HandleSpellCasting()
        {
            if (curSpellType == SpellType.looping)
            {
                enableIK = true;
                a_hook.currentHand = (spellIsMirrored) ? AvatarIKGoal.LeftHand : AvatarIKGoal.RightHand;

                if ((f == false && e == false) || characterStats._focus <= 1)
                {
                    isSpellCasting = false;

                    enableIK = false;

                    inventoryManager.breathCollider.SetActive(false);
                    inventoryManager.blockCollider.SetActive(false);

                    if (spellCast_stop != null)
                        spellCast_stop();

                    return;
                }

                if (spellCast_loop != null)
                    spellCast_loop();

                characterStats._focus -= 0.25f * delta;

                return;
            }
            spellCastingTime += delta;

            if (inventoryManager.currentSpell.currentParticle != null)
                inventoryManager.currentSpell.currentParticle.SetActive(true);

            if (spellCastingTime > max_spellCastTime)
            {
                onEmpty = false;
                canMove = false;
                canAttack = true;
                inAction = true;
                isSpellCasting = false;

                string targetAnim = spellTargetAnim;
                anim.SetBool(StaticStrings.mirror, spellIsMirrored);
                anim.CrossFade(targetAnim, 0.2f);
            }

        }
        bool blockAnim;
        string block_idle_anim;
        void HanddleBlocking()
        {
            if (isBlocking == false)
            {
                if (blockAnim)
                {
                    anim.CrossFade(block_idle_anim, 0.1f);
                    blockAnim = false;
                }

            }
            else
            {

            }
        }
        public void ThrowProjectile()
        {
            if (projectileCanidate == null)
                return;

            GameObject go = Instantiate(projectileCanidate) as GameObject;
            Transform p = anim.GetBoneTransform((spellIsMirrored) ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand);
            go.transform.position = p.position;

            if (lockOnTransform && lockOn)
                go.transform.LookAt(lockOnTransform.position);
            else
                go.transform.rotation = transform.rotation;

            Projectile proj = go.GetComponent<Projectile>();
            proj.Init();

            characterStats._stamina -= cur_staminaCost;
            characterStats._focus -= cur_focusCost;
        }
        bool CheckForParry(Action slot)
        {
            if (slot.canParry == false)
                return false;

            EnemyStates parryTarget = null;
            Vector3 origin = transform.position;
            origin.y += 1;
            Vector3 rayDir = transform.forward;
            RaycastHit hit;
            if (Physics.Raycast(origin, rayDir, out hit, 3, ~ignoreLayers))
            {
                parryTarget = hit.transform.GetComponentInParent<EnemyStates>();
            }

            Debug.DrawRay(origin, rayDir * 3, Color.red, 0.5f);

            if (parryTarget == null)
                return false;

            if (parryTarget.canBeParried == false)
                return false;

            // ใช้ได้ทั้งสองแบบ: (1) กดปุ่ม parry แล้วศัตรูชน ParryCollider → parriedBy ถูก set
            // (2) โจมตีศัตรูที่กำลังโจมตี → parryIsOn = true
            if (parryTarget.parriedBy == null && parryTarget.parryIsOn == false)
                return false;

            /*if (parryTarget == null)
               return false;


           float dis = Vector3.Distance(parryTarget.transform.position, transform.position);

           if (dis > 3)
               return false;*/

            Vector3 dir = parryTarget.transform.position - transform.position;
            dir.Normalize();
            dir.y = 0;
            float angle = Vector3.Angle(transform.forward, dir);

            if (angle < 60)
            {
                Debug.Log("Parry ถูกใช้! (ทำดาเมจตอบ)");

                Vector3 targetPosition = -dir * parryOffset;
                targetPosition += parryTarget.transform.position;
                transform.position = targetPosition;

                if (dir == Vector3.zero)
                    dir = -parryTarget.transform.forward;

                Quaternion eRotation = Quaternion.LookRotation(-dir);
                Quaternion ourRot = Quaternion.LookRotation(dir);

                parryTarget.transform.rotation = eRotation;
                transform.rotation = ourRot;

                parryTarget.parriedBy = this;
                parryTarget.IsGettingParried(slot);

                onEmpty = false;
                canMove = false;
                canAttack = false;
                inAction = true;
                anim.SetBool(StaticStrings.mirror, slot.mirror);
                anim.CrossFade(StaticStrings.parry_attack, 0.2f);
                lockOnTarget = null;
                return true;
            }

            return false;

        }
        bool CheckForBackstab(Action slot)
        {
            if (slot.canBackStab == false)
                return false;

            EnemyStates backstab = null;
            Vector3 origin = transform.position;
            origin.y += 1;
            Vector3 rayDir = transform.forward;
            RaycastHit hit;
            if (Physics.Raycast(origin, rayDir, out hit, 1, ~ignoreLayers))
            {
                backstab = hit.transform.GetComponentInParent<EnemyStates>();
            }

            if (backstab == null)
                return false;

            Vector3 dir = transform.position - backstab.transform.position;
            dir.Normalize();
            dir.y = 0;
            float angle = Vector3.Angle(backstab.transform.forward, dir);

            if (angle > 150)
            {
                Vector3 targetPosition = dir * backstabOffset;
                targetPosition += backstab.transform.position;
                transform.position = targetPosition;


                backstab.transform.rotation = transform.rotation;
                backstab.IsGettingBackStabbed(slot);

                onEmpty = false;
                canMove = false;
                canAttack = false;
                inAction = true;
                anim.SetBool(StaticStrings.mirror, slot.mirror);
                anim.CrossFade(StaticStrings.parry_attack, 0.2f);
                lockOnTarget = null;
                //lockOn = false;
                return true;

            }
            return false;
        }
        void BlockAction(Action slot)
        {
            isBlocking = true;
            enableIK = true;
            isLeftHand = slot.mirror;
            a_hook.currentHand = (slot.mirror) ? AvatarIKGoal.LeftHand : AvatarIKGoal.RightHand;
            a_hook.InitIKForShield(slot.mirror);

            if (blockAnim == false)
            {
                block_idle_anim = (isTwoHanded == false) ?
                inventoryManager.GetCurrentWeapon(isLeftHand).oh_idle :
                inventoryManager.GetCurrentWeapon(isLeftHand).th_idle;

                block_idle_anim += (isLeftHand) ? "_l" : "_r";

                string targetAnim = slot.fristStep.targetAnim;
                targetAnim += (isLeftHand) ? "_l" : "_r";
                anim.CrossFade(targetAnim, 0.1f);
                blockAnim = true;
            }
        }
        void ParryAction(Action slot)
        {
            string targetAnim = null;
            targetAnim = slot.GetActionStep(ref actionManager.actionIndex).targetAnim;

            if (string.IsNullOrEmpty(targetAnim))
                return;

            float targetSpeed = 1;
            if (slot.changeSpeed)
            {
                targetSpeed = slot.animSpeed;
                if (targetSpeed == 0)
                    targetSpeed = 1;
            }

            anim.SetFloat(StaticStrings.animSpeed, targetSpeed);

            canBeParried = slot.canBeParried;
            onEmpty = false;
            canMove = false;
            canAttack = false;
            inAction = true;
            anim.SetBool(StaticStrings.mirror, slot.mirror);
            anim.CrossFade(targetAnim, 0.2f);
        }
        public void Tick(float d)
        {
            delta = d;
            onGround = OnGround();
            anim.SetBool(StaticStrings.OnGround, onGround);

            if (!onGround)
            {
                airTimer += delta;
            }
            else
            {
                airTimer = 0;
            }
            pickManager.Tick();
        }

        void HandleRolls()
        {
            if (!rollInput || usingItem)
                return;

            rollInput = false;

            float v;
            float h;

            if (lockOn && lockOnTransform != null)
            {
                if (moveAmount > 0.3f)
                {
                    Vector3 relativeDir = transform.InverseTransformDirection(moveDir);
                    h = Mathf.Clamp(relativeDir.x, -1f, 1f);
                    v = Mathf.Clamp(relativeDir.z, -1f, 1f);
                }
                else
                {
                    v = 0f;
                    h = 0f;
                }
            }
            else
            {
                h = 0f;
                v = (moveAmount > 0.3f) ? 1f : 0f;

                if (v != 0f)
                {
                    if (moveDir == Vector3.zero)
                        moveDir = transform.forward;
                    Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
                    transform.rotation = targetRot;
                }
            }

            bool isStepBack = Mathf.Approximately(v, 0f) && Mathf.Approximately(h, 0f);
            a_hook.rm_Mutil = isStepBack ? 1f : rollSpeed;

            Vector3 localDir = isStepBack
                ? Vector3.back
                : new Vector3(h, 0f, v).normalized;
            Vector3 worldDir = transform.TransformDirection(localDir);
            a_hook.InitForRoll(worldDir, isStepBack);

            anim.SetFloat(StaticStrings.Vertical_Axis, v);
            anim.SetFloat(StaticStrings.Horizontal_Axis, h);

            onEmpty = false;
            canMove = false;
            canAttack = false;
            inAction = true;
            anim.SetBool(StaticStrings.onEmpty, false);
            anim.applyRootMotion = true;

            anim.CrossFade(StaticStrings.Rolls, 0.2f);
            isBlocking = false;
        }

        void HandleMovementAnimations()
        {
            anim.SetFloat(StaticStrings.Vertical_Axis, moveAmount, 0.4f, delta);
            anim.SetBool(StaticStrings.run, run);
        }

        void HandleLockOnAnimations(Vector3 moveDir)
        {
            Vector3 relativeDir = transform.InverseTransformDirection(moveDir);
            float h = relativeDir.x;
            float v = relativeDir.z;

            anim.SetFloat(StaticStrings.Vertical_Axis, v, 0.2f, delta);
            anim.SetFloat(StaticStrings.Horizontal_Axis, h, 0.2f, delta);



        }
        public void Jump()
        {
            a_hook.jumping = true;
            onEmpty = false;
            canMove = false;
            canAttack = false;
            inAction = true;
            anim.SetBool(StaticStrings.onEmpty, false);

            anim.Play(StaticStrings.Jump_start);
            isBlocking = false;

            skipGroundCheck = true;
            Vector3 targetVel = transform.forward * 7;
            targetVel.y = 5;
            rigid.linearVelocity = targetVel;
        }
        bool skipGroundCheck;
        float skipTimer;
        bool prevGround;
        public bool OnGround()
        {
            if (skipGroundCheck)
            {
                skipTimer += delta;
                if (skipTimer > 0.2f)
                    skipGroundCheck = false;
                prevGround = false;
                return false;
            }
            skipTimer = 0;

            bool r = false;

            Vector3 origin = transform.position + (Vector3.up * toGround);
            Vector3 dir = -Vector3.up;
            float dis = toGround + 0.3f;
            RaycastHit hit;
            Debug.DrawRay(origin, dir * dis, Color.red, 0.5f);
            if (Physics.Raycast(origin, dir, out hit, dis, ignoreLayers))
            {
                r = true;
                Vector3 tagetPostition = hit.point;
                transform.position = tagetPostition;
            }
            if (r && !prevGround)
            {
                Land();
            }
            prevGround = r;
            return r;
        }
        void Land()
        {
            onEmpty = false;
            canMove = false;
            canAttack = false;
            inAction = true;
            isBlocking = false;
            a_hook.jumping = false;

            if (moveAmount == 0)
            {
                anim.Play(StaticStrings.Jump_land);
                Debug.Log("Jump landed");
            }
            else
            {
                if (moveDir == Vector3.zero)
                    moveDir = transform.forward;
                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                transform.rotation = targetRot;
                a_hook.InitForRoll(moveDir);
                a_hook.rm_Mutil = rollSpeed;

                anim.SetFloat(StaticStrings.Vertical_Axis, 1);
                anim.SetFloat(StaticStrings.Horizontal_Axis, 0);

                anim.CrossFade(StaticStrings.Rolls, 0.2f);
            }
        }
        public void PlayInteractAnimation()
        {
            onEmpty = false;
            canMove = false;
            canAttack = false;
            inAction = true;
            isBlocking = false;
           
            anim.SetBool(StaticStrings.onEmpty, false);
            anim.CrossFade(StaticStrings.pick_up, 0.2f);

        }
        public void HandleTwoHanded()
        {
            bool isRight = true;

            Weapon w = inventoryManager.rightHandWeapon.instance;
            if (w == null)
            {
                w = inventoryManager.leftHandWeapon.instance;
                isRight = false;
            }

            if (w == null)
            {
                return;
            }

            if (isTwoHanded)
            {
                anim.CrossFade(w.th_idle, 0.2f);
                actionManager.UpdateActionsTwoHanded();

                if (isRight)
                {
                    if (inventoryManager.leftHandWeapon)
                        inventoryManager.leftHandWeapon.weaponModel.SetActive(false);
                }
                else
                {
                    if (inventoryManager.rightHandWeapon)
                        inventoryManager.rightHandWeapon.weaponModel.SetActive(false);
                }
            }
            else
            {

                string targetAnim = w.oh_idle;
                targetAnim += (isRight) ? StaticStrings._r : StaticStrings._l;
                //anim.CrossFade(targetAnim,0.2f);
                anim.Play(StaticStrings.equipWeapon_oh);
                actionManager.UpdateActionsOneHanded();


                if (isRight)
                {
                    if (inventoryManager.leftHandWeapon)
                        inventoryManager.leftHandWeapon.weaponModel.SetActive(true);
                }
                else
                {
                    if (inventoryManager.rightHandWeapon)
                        inventoryManager.rightHandWeapon.weaponModel.SetActive(true);
                }
            }
        }
        public void IsGettingParried()
        {

        }
        public void AddHealth()
        {
            characterStats.fp++;
        }
        public void MonitorStats()
        {
            if (run && moveAmount > 0)
            {
                characterStats._stamina -= delta * 10;
            }
            else
            {
                characterStats._stamina += delta * 10;
            }
            if (characterStats._stamina > characterStats.fp)
                characterStats._stamina = characterStats.fp;

            characterStats._health = Mathf.Clamp(characterStats._health, 0, characterStats.hp);
            characterStats._focus = Mathf.Clamp(characterStats._focus, 0, characterStats.fp);
            // characterStats._stamina = Mathf.Clamp(characterStats._stamina, 0, characterStats.stamina);
        }

        public void SubstractStaminaOverTime()
        {
            characterStats._stamina -= cur_staminaCost * delta;
        }
        public void SubstractFocusOverTime()
        {
            characterStats._focus -= cur_focusCost * delta;
        }
        public void AffectBlocking()
        {
            isBlocking = true;
        }
        public void StopAffectinBlocking()
        {
            isBlocking = false;
        }
    }
}