using UnityEngine;

namespace SA
{
    public class InputHandler : MonoBehaviour
    {
        float vertical;
        float horizontal;
        bool b_input;
        bool a_input;
        bool rolls_input;
        bool y_input;

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
            vertical = Input.GetAxis("Vertical");
            horizontal = Input.GetAxis("Horizontal");

            b_input = Input.GetKey(KeyCode.LeftShift);//run
            a_input = Input.GetKeyDown(KeyCode.Space);
            //rolls_input = Input.GetKeyDown(KeyCode.LeftControl);
            y_input = Input.GetKeyDown(KeyCode.T);//two handed
            lockon_input = Input.GetKeyDown(KeyCode.Tab);

            // rt_axis = Input.GetAxis("RT");
            //rt_axis = Input.GetAxis("RT");

            lt_input = Input.GetKey(KeyCode.Q);
            lb_input = Input.GetKey(KeyCode.E);
            rt_input = Input.GetKey(KeyCode.R);
            rb_input = Input.GetKey(KeyCode.F);

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


             if (b_input && b_timer > 0.5f)
                 {
                     states.run = (states.moveAmount > 0);
                 }

             if (b_input == false && b_timer > 0 && b_timer < 0.5f)
                 states.rollInput = true;

             states.rt = rt_input;
             states.lt = lt_input;
             states.rb = rb_input;
             states.lb = lb_input;



             if (y_input)
             {
                states.istwoHanded = !states.istwoHanded;
                 states.HandleTwoHanded();
             }

             if (lockon_input)
             {
                 states.lockOn = !states.lockOn;

                 if (states.lockOnTarget == null)
                     states.lockOn = false;

                 camManager.lockonTarget = states.lockOnTarget;
                 states.lockOnTransform = camManager.lockonTransform;
                 camManager.lockon = states.lockOn;

             }


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
