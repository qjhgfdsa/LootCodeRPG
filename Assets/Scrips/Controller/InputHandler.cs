using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace SA
{
    public class InputHandler : MonoBehaviour
    {
        float vertical;
        float horizontal;
        bool shift_input;
        bool space_input;
        bool rolls_input;
        bool t_input;
        bool x_input;

        bool key1_input;
        bool key2_input;
        bool key3_input;
        bool key4_input;

        bool f_input;
        bool r_input;
        bool e_input;
        bool q_input;
        bool lockon_input;

        float shift_timer;



        StateManager states;
        CameraManager camManager;
        UIManager uiManager;

        bool isGesturesOpen;
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
            uiManager = UIManager.singleton;
            if (uiManager == null)
                Debug.LogWarning("InputHandler: UIManager is missing from the scene — HUD will not update.");
        }
        void FixedUpdate()
        {
            if (camManager == null)
                return;

            delta = Time.fixedDeltaTime;
            camManager.Tick(delta);
            states.FixedTick(delta);
        }

        void Update()
        {
            delta = Time.deltaTime;

            GetInput();
            HandleUI();
            UpdateStates();
            states.Tick(delta);

            ResetInputNStates();
            states.MonitorStats();
            if (uiManager != null)
                uiManager.Tick(states.characterStats, delta);
        }

        void GetInput()
        {
            vertical = Input.GetAxis(StaticStrings.Vertical);
            horizontal = Input.GetAxis(StaticStrings.Horizontal);

            shift_input = Input.GetKey(StaticStrings.KeyShift);
            space_input = Input.GetKeyDown(StaticStrings.KeySpace);
            t_input = Input.GetKeyDown(StaticStrings.KeyT);
            x_input = Input.GetKeyDown(StaticStrings.KeyX);


            q_input = Input.GetKey(StaticStrings.KeyQ);
            e_input = Input.GetKey(StaticStrings.KeyE);
            r_input = Input.GetKey(StaticStrings.KeyR);
            f_input = Input.GetKey(StaticStrings.KeyF);

            if (shift_input)
                shift_timer += delta;

            key1_input = Input.GetKeyDown(KeyCode.Alpha1);
            key2_input = Input.GetKeyDown(KeyCode.Alpha2);
            key3_input = Input.GetKeyDown(KeyCode.Alpha3);
            key4_input = Input.GetKeyDown(KeyCode.Alpha4);

            bool gesturesMenu = Input.GetKeyDown(StaticStrings.KeyG);
            if (gesturesMenu)
            {
                isGesturesOpen = !isGesturesOpen;
            }
        }

        void HandleUI()
        {
            if (uiManager == null || uiManager.gestures == null)
                return;

            uiManager.gestures.HandleGestures(isGesturesOpen);

            if (isGesturesOpen)
            {
                curUIState = UIState.gestures;
            }
            else
            {
                curUIState = UIState.game;
            }

            switch (curUIState)
            {
                case UIState.game:
                    HandleQuickSlotChanges();
                    break;
                case UIState.gestures:
                    HandleGesturesUI();
                    break;
                case UIState.inventory:
                    break;
            }

        }

        UIState curUIState;
        enum UIState
        {
            game, gestures, inventory
        }

        void HandleGesturesUI()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > 0f)
                uiManager.gestures.SelectGesture(true);
            if (scroll < 0f)
                uiManager.gestures.SelectGesture(false);

            if (Input.GetMouseButtonDown(0))
            {
                isGesturesOpen = false;
                states.usingItem = true;

                var selected = uiManager.gestures.gestures[uiManager.gestures.index];
                if (selected.closeWeapon)
                    states.closeWeapons = true;

                states.PlayAnimation(selected.targetAnim, false);
            }
        }

        void UpdateStates()
        {
            if (isGesturesOpen)
                return;

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
                shift_input = false;

            if (shift_input && shift_timer > 0.5f)
                states.run = (states.moveAmount > 0) && states.characterStats._stamina > 0;
            else
                states.run = false;

            if (shift_input == false && shift_timer > 0 && shift_timer < 0.5f)
                states.rollInput = true;

            if (x_input)
                states.itemInputPending = true;
            states.r = r_input;
            states.q = q_input;
            states.f = f_input;
            states.e = e_input;

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

            if (key1_input)
                states.inventoryManager.ChangeToNextSpell();

            if (states.onEmpty == false)
                return;
            if (states.isTwoHanded)
                return;

            if (key2_input)
                states.inventoryManager.ChangeToNextConsumable();

            if (key3_input)
                states.inventoryManager.ChangeToNextWeapon(true);
            if (key4_input)
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
            if (shift_input == false)
                shift_timer = 0;
        }
    }
}
