using UnityEngine;

namespace SA
{
    public class InputHandler : MonoBehaviour
    {
        float vertical;
        float horizontal;
        bool runInput;

        StateManager states;
        CameraManager camManager;
        float delta;

        void Start()
        {
            states = GetComponent<StateManager>();
            states.Init();

            camManager = CameraManager.singleton;
            camManager.Init(this.transform);
        }

        void FixedUpdate()
        {
            delta = Time.fixedDeltaTime;
            GetInput();
            UpdateStates();
            states.FixedTick(delta);
            camManager.Tick(delta);


            // Here you can add logic to handle the input, such as moving a character or triggering actions
        }

        void Update()
        {
            delta = Time.deltaTime;
            states.Tick(delta);
            
        }

        void GetInput()
        {
            vertical = Input.GetAxis("Vertical");
            horizontal = Input.GetAxis("Horizontal");
            
            runInput = Input.GetKey(KeyCode.LeftShift);
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

            if (runInput)
            {
                states.run = (states.moveAmount > 0);
            }
            else
            {
                states.run = false;
            }

          

        }
    }
}
