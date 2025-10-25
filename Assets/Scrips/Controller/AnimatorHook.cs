using UnityEngine;

namespace SA
{

    public class AnimatorHook : MonoBehaviour
    {
        StateManager states;
        Animator anim;
        Rigidbody rigid;
        EnemyStates eStates;


        public float rm_Mutil;
        bool rolling;
        float roll_t;
        float delta;
        public AnimationCurve rollCurve;


        public void Init(StateManager st, EnemyStates eSt)
        {
            states = st;
            eStates = eSt;

            if (st != null)
            {
                anim = states.anim;
                rigid = st.rigid;
                rollCurve = states.roll_curve;
                delta = st.delta;
            }

            if (eSt != null)
            {
                anim = eStates.anim;
                rigid = eSt.rigid;
                delta = eSt.delta;
            }

            // rollCurve = states.roll_curve;
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
            if (states == null && eStates == null)
                return;

            if (rigid == null)
                return;


            if (states != null)
            {
                if (states.canMove)
                    return;

                delta = states.delta;

            }

            if (eStates != null)
            {
                if (eStates.canMove)
                    return;

                delta = eStates.delta;
            }

            rigid.linearDamping = 0;

            if (rm_Mutil == 0)
                rm_Mutil = 1;

            if (rolling == false)
            {
                Vector3 delta2 = anim.deltaPosition;
                delta2.y = 0;
                Vector3 v = (delta2 * rm_Mutil) / delta;

                if (!rigid.isKinematic)
                    rigid.linearVelocity = v;
            }
            else
            {
                roll_t += delta / 0.6f;

                if (roll_t > 1)
                {
                    roll_t = 1;
                }

                if (states == null)
                    return;

                float zValue = rollCurve.Evaluate(roll_t);
                Vector3 v1 = Vector3.forward * zValue;
                Vector3 relative = transform.TransformDirection(v1);
                Vector3 v2 = (relative * rm_Mutil);
                rigid.linearVelocity = v2;
            }

        }

        public void OpenDamageColliders()
        {
            if (states)
            {
                states.inventoryManager.OpenAllDamageColliders();
            }
            OpenParryFlag();

        }

        public void CloseDamageColliders()
        {
            if (states)
            {
                states.inventoryManager.CloseAllDamageColliders();
            }
            CloseParryFlag();
        }

        public void OpenParryCollider()
        {
            if (states == null)
                return;
            states.inventoryManager.OpenParryCollider();
        }
        public void CloseParryCollider()
        {
            if (states == null)
                return;
            states.inventoryManager.CloseParryCollider();
        }
        public void OpenParryFlag()
        {
            if (states)
            {
                states.parryIsOn = true;

            }
            if (eStates)
            {
                eStates.parryIsOn = true;
            }
        }

        public void CloseParryFlag()
        {
             if (states)
            {
                states.parryIsOn = false;

            }
            if (eStates)
            {
                eStates.parryIsOn = false;
            }

        } 
    }
}