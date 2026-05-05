using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace SA
{
    public class InputHandler : MonoBehaviour
    {
        float vertical;
        float horizontal;
        bool b_input;
        bool a_input;
        bool rolls_input;
        bool t_input;
        bool x_input;

        bool d_up;
        bool d_down;
        bool d_right;
        bool d_left;


        bool rb_input;
        bool rt_input;
        float rt_axis;
        bool lb_input;
        float lt_axis;
        bool lt_input;
        bool lockon_input;

        float b_timer;



        StateManager states;
        CameraManager camManager;
        float delta;

        void Awake()
        {
            states = GetComponent<StateManager>();
        }

        void Start()
        {
            if (UI.QuickSlot.singleton != null)
                UI.QuickSlot.singleton.Init();
            else
                Debug.LogWarning("InputHandler: QuickSlot is missing from the scene — hotbar UI will not update.");

            states.Init();

            // รอให้ CameraManager พร้อม
            camManager = CameraManager.singleton;

            if (camManager == null)
            {
                // ถ้ายังเป็น null ลองหาจาก Scene
                camManager = FindAnyObjectByType<CameraManager>();

                if (camManager != null)
                {
                    // เจอแล้ว ให้มันเป็น singleton เลย
                    CameraManager.singleton = camManager;
                }
                else
                {
                    Debug.LogError("❌ ไม่เจอ CameraManager ใน Scene! ต้องมี GameObject ที่แนบ CameraManager script");
                    enabled = false;
                    return;
                }
            }

            camManager.Init(states);
        }
        void FixedUpdate()
        {
            delta = Time.fixedDeltaTime;
           // states.FixedTick(delta);
            camManager.Tick(delta);
        }

        void Update()
        {
            delta = Time.deltaTime;
            GetInput();
            UpdateStates();
            states.Tick(delta);
            ResetInputNStates();
            states.FixedTick(delta);//สลับจาก FixedUpdate เป็น Update
        }

        void GetInput()
        {
            vertical = Input.GetAxis(StaticStrings.Vertical);
            horizontal = Input.GetAxis(StaticStrings.Horizontal);

            b_input = Input.GetKey(StaticStrings.RunKey);//run
            a_input = Input.GetKeyDown(StaticStrings.JumpKey);//jump
            //rolls_input = Input.GetKeyDown(KeyCode.LeftControl);
            t_input = Input.GetKeyDown(StaticStrings.TwoHandedKey);//two handed
            x_input = Input.GetKeyDown(StaticStrings.UseItemKey); //using item
                                                                  // lockon_input = Input.GetKeyDown(StaticStrings.lockOnKey); ไม่ได้ใช้

            // rt_axis = Input.GetAxis("RT");
            //rt_axis = Input.GetAxis("RT");

            lt_input = Input.GetKey(StaticStrings.AttackKey1);
            lb_input = Input.GetKey(StaticStrings.AttackKey2);
            rt_input = Input.GetKey(StaticStrings.AttackKey3);
            rb_input = Input.GetKey(StaticStrings.AttackKey4);


            if (b_input)
                b_timer += delta;

            d_up = Input.GetKeyDown(KeyCode.Alpha1);
            d_down = Input.GetKeyDown(KeyCode.Alpha2);
            d_left = Input.GetKeyDown(KeyCode.Alpha3);
            d_right = Input.GetKeyDown(KeyCode.Alpha4);
        }

        void UpdateStates()
        {
            states.vertical = vertical;
            states.horizontal = horizontal;

            // เช็คทุกครั้งก่อนใช้ (ป้องกัน camManager หาย)
            if (camManager == null)
            {
                camManager = CameraManager.singleton;
                if (camManager == null)
                {
                    camManager = FindAnyObjectByType<CameraManager>();
                    if (camManager == null)
                    {
                        Debug.LogError("❌ CameraManager หายไประหว่างเล่น!");
                        return; // หยุดทำงานต่อ
                    }
                }
            }

            Vector3 v = vertical * camManager.transform.forward;
            Vector3 h = horizontal * camManager.transform.right;
            states.moveDir = (v + h).normalized;

            float m = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
            states.moveAmount = Mathf.Clamp01(m);

            if (x_input)
                b_input = false;

            if (b_input && b_timer > 0.5f)
                states.run = (states.moveAmount > 0);

            if (b_input == false && b_timer > 0 && b_timer < 0.5f)
                states.rollInput = true;

            states.itemInput = x_input;
            states.rt = rt_input;
            states.lt = lt_input;
            states.rb = rb_input;
            states.lb = lb_input;

            if (t_input)
            {
                states.isTwoHanded = !states.isTwoHanded;
                states.HandleTwoHanded();
            }

            // ========== LOCK-ON SYSTEM ==========

            // [1] เช็ค target ที่ล็อคอยู่
            if (states.lockOn)
            {
                bool shouldClear = false;

                if (states.lockOnTarget == null)
                    shouldClear = true;
                else if (states.lockOnTarget.eStates != null && states.lockOnTarget.eStates.isDead) // เพิ่มการเช็ค eStates
                    shouldClear = true;

                if (shouldClear)
                    ClearLockOn();
            }

            // [2] กด scroll wheel click
            if (Input.GetMouseButtonDown(2))
            {
                if (states.lockOn)
                {
                    ClearLockOn();
                }
                else
                {
                    TryLockOn();
                }
            }

            HandleQuickSlotChanges();
        }

        void HandleQuickSlotChanges()
        {
            if (states.isSpellCasting || states.usingItem)
                return;
           
            if (d_up)
                states.inventoryManager.ChangeToNextSpell();

             if (states.canMove == false)
                return;
            if (states.isTwoHanded)
                return;

            if (d_left)
                states.inventoryManager.ChangeToNextWeapon(true);
            if (d_right)
                states.inventoryManager.ChangeToNextWeapon(false);

        }

        void TryLockOn()
        {
            // เช็ค EnemyManager ก่อน
            if (EnemyManager.singleton == null)
            {
                Debug.LogWarning("EnemyManager.singleton is null!");
                return;
            }

            EnemyTarget target = EnemyManager.singleton.GetEnemy(transform.position);

            if (target == null)
                return;

            Transform targetTransform = target.GetTarget();
            if (targetTransform == null)
                return;

            // Set ทุกอย่าง
            states.lockOnTarget = target;
            states.lockOnTransform = targetTransform;
            states.lockOn = true;

            camManager.lockonTarget = target;
            camManager.lockonTransform = targetTransform;
            camManager.lockon = true;
        }

        void ClearLockOn()
        {
            states.lockOn = false;
            states.lockOnTarget = null;
            states.lockOnTransform = null;

            camManager.lockon = false;
            camManager.lockonTarget = null;
            camManager.lockonTransform = null;
        }



        void ResetInputNStates()
        {

            if (b_input == false)
                b_timer = 0;

            if (states.rollInput)
                states.rollInput = false;

            if (states.run)
                states.run = false;
        }
    }
}
