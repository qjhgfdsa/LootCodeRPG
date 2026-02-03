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

        void Start()
        {
            UI.QuickSlot.singleton.Init();
            
            states = GetComponent<StateManager>();
            states.Init();

            camManager = CameraManager.singleton;
            camManager.Init(states);

            
        }

        void FixedUpdate()
        {
            delta = Time.fixedDeltaTime;
            GetInput();
            UpdateStates();
            states.FixedTick(delta);
            camManager.Tick(delta);
        }

        void Update()
        {
            delta = Time.deltaTime;
            states.Tick(delta);
            ResetInputNStates();
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
        }

          void UpdateStates()
          {
              states.vertical = vertical;
              states.horizontal = horizontal;

             Vector3 v = vertical * camManager.transform.forward;
             Vector3 h = horizontal * camManager.transform.right;
              states.moveDir = (v + h).normalized;
              float m = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
              states.moveAmount = Mathf.Clamp01(m);

             if (x_input)
                 b_input = false;


              if (b_input && b_timer > 0.5f)
                  {
                      states.run = (states.moveAmount > 0);
                  }

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

            if (states.lockOnTarget != null)
            {
                if (states.lockOnTarget.eStates.isDead)
                {
                    states.lockOn = false;
                    states.lockOnTarget = null;
                    states.lockOnTransform = null;
                    camManager.lockon = states.lockOn = false;
                    camManager.currentEnemyTarget = null;
                    //camManager.lockonTarget = null;
                }
            } else
            {
                
                    states.lockOn = false;
                    states.lockOnTarget = null;
                    states.lockOnTransform = null;
                    camManager.lockon = states.lockOn = false;
                    camManager.currentEnemyTarget = null;
                
            }

            /*  if (Input.GetMouseButtonDown(2))
                {
                    states.lockOn = !states.lockOn;

                    states.lockOnTarget = EnemyManager.singleton.GetEnemy(transform.position);
                    if (states.lockOnTarget == null)
                     states.lockOn = false;

                   
                    //camManager.lockonTarget = states.lockOnTarget;
                 

                    camManager.currentEnemyTarget = states.lockOnTarget;
                    states.lockOnTransform = camManager.lockonTransform;
                    camManager.lockon = states.lockOn;

                } */
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
