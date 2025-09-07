using UnityEngine;

namespace SA
{

    public class AnimatorHook : MonoBehaviour
    {
        StateManager states;
        Animator anim;

        public float rm_Mutil;
        bool rolling;
        float roll_t;
        public AnimationCurve rollCurve;


        public void Init(StateManager st)
        {
            states = st;
            anim = states.anim;
            rollCurve = states.roll_curve;

        }

        public void InitForRoll()
        {
            rolling = true;
            roll_t = 0;
        }

        public void CloseRoll()
        {
            if (rolling == false)
                return;

            rm_Mutil = 1;
            rolling = false;
        }

        void OnAnimatorMove()
        {
            if (states == null)
                return;
                
            if (states.canMove)
                return;

            states.rigid.linearDamping = 0;

            if (rm_Mutil == 0)
                rm_Mutil = 1;

            if (rolling == false)
            {
                Vector3 delta = anim.deltaPosition;
                delta.y = 0;
                Vector3 v = (delta * rm_Mutil) / states.delta;
                states.rigid.linearVelocity = v;
            }
            else
            {
                roll_t += Time.deltaTime / 0.6f;
                if (roll_t > 1)
                    roll_t = 1;

                float zValue = rollCurve.Evaluate(roll_t);
                Vector3 v1 = Vector3.forward * zValue;
                Vector3 relative = transform.TransformDirection(v1);
                Vector3 v2 = (relative * rm_Mutil);
                states.rigid.linearVelocity = v2;

            }

        }

        public void OpenDamageColliders()
        {
            if (states == null)
                return;
            states.inventoryManager.curWeapon.w_Hook.OpenDamageColliders();

        }

        public void CloseDamageColliders()
        {
            if (states == null)
                return;
            states.inventoryManager.curWeapon.w_Hook.CloseDamageColliders();

        }
    }
}