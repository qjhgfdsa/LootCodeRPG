using UnityEngine;

namespace SA
{

    public class AnimatorHook : MonoBehaviour
    {
        StateManager states;
        Animator anim;
        Rigidbody rigid;
        EnemyStates eStates;

        public bool jumping;

        public float rm_Mutil;
        bool rolling;
        float roll_t;
        float delta;
        AnimationCurve rollCurve;
        Vector3 rollDirection = Vector3.forward;
        bool stepBack;

        public Transform ikTarget;
        public Transform bodyTarget;
        public Transform headTarget;

        public Transform ikTargetShield;
        public Transform ikTargetBodyshield;

        HandelIK ik_handler;
        public bool useIk;
        public AvatarIKGoal currentHand;
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

            ik_handler = gameObject.GetComponent<HandelIK>();
            if (ik_handler != null)
                ik_handler.Init(anim);
        }
        public void InitForRoll(Vector3 worldDirection, bool isStepBack = false)
        {
            rolling = true;
            stepBack = isStepBack;
            roll_t = 0;
            rollDirection = worldDirection.sqrMagnitude > 0.01f
                ? worldDirection.normalized
                : transform.forward;
        }

        public void CloseRoll()
        {
            if (rolling == false)
                return;

            rm_Mutil = 1;
            rolling = false;
            stepBack = false;
        }
        void OnAnimatorMove()
        {
            if (ik_handler != null)
            {
                ik_handler.OnAnimatorMoveTick((currentHand == AvatarIKGoal.LeftHand));
            }
            if (states == null && eStates == null)
                return;

            if (rigid == null)
                return;

            if (jumping)
                return;

            if (states != null)
            {
                if (states.onEmpty && !rolling)
                    return;

                delta = states.delta;
            }

            if (eStates != null)
            {
                if (eStates.isDead || eStates.canMove)
                    return;

                delta = eStates.delta;
            }

            rigid.linearDamping = 0;

            if (rm_Mutil == 0)
                rm_Mutil = 1;

            if (delta <= 0f)
                return;

            if (rolling)
            {
                roll_t += delta / 0.6f;
                if (roll_t > 1)
                    roll_t = 1;

                if (stepBack)
                {
                    Vector3 stepDelta = anim.deltaPosition;
                    Vector3 stepVel = (stepDelta * rm_Mutil) / delta;
                    stepVel += Physics.gravity;
                    rigid.linearVelocity = stepVel;
                    return;
                }

                if (rollCurve != null)
                {
                    float zValue = rollCurve.Evaluate(roll_t);
                    Vector3 v2 = rollDirection * (zValue * rm_Mutil);
                    v2 += Physics.gravity;
                    rigid.linearVelocity = v2;
                    return;
                }
            }

            Vector3 delta2 = anim.deltaPosition;
            Vector3 v = (delta2 * rm_Mutil) / delta;

            if (states)
            {
                if (!states.onGround)
                    v.y = rigid.linearVelocity.y;
            }
            if (eStates && eStates.AgentReady)
            {
                eStates.agent.velocity = v;
            }
            else if (!rigid.isKinematic)
            {
                rigid.linearVelocity = v;
            }

            v += Physics.gravity;

            if (!rigid.isKinematic && float.IsFinite(v.x) && float.IsFinite(v.y) && float.IsFinite(v.z))
                rigid.linearVelocity = v;

        }
        void OnAnimatorIK()
        {
            if (ik_handler == null)
                return;

            if (!useIk)
            {
                if (ik_handler.weight > 0)
                {
                    ik_handler.IKTick(currentHand, 0);
                }
                else
                {
                    ik_handler.weight = 0;
                }
            }
            else
            {
                ik_handler.IKTick(currentHand, 1);
            }
        }
        void LateUpdate()
        {
            if (ik_handler == null)
                return;

            ik_handler.LateTick();
        }
        public void OpenAttack()
        {
            if (states)
            {
                states.canAttack = true;
            }
        }
        public void OpenCanMove()
        {
            if (states)
            {
                states.canMove = true;
            }
            if (eStates)
            {
                eStates.anim.SetBool(StaticStrings.onEmpty, true);
            }
        }
        public void CloseCanMove()
        {
            if (states)
            {
                states.canMove = false;
            }
            if (eStates)
            {
                eStates.anim.SetBool(StaticStrings.onEmpty, false);
                if (eStates.AgentReady)
                {
                    eStates.agent.isStopped = true;
                    eStates.agent.velocity = Vector3.zero;
                }
                eStates.anim.SetFloat(StaticStrings.Vertical_Axis, 0);
            }
        }
        public void OpenCanRotate()
        {
            if (states)
            {
                states.canRotate = true;
            }
            if (eStates)
            {
                eStates.rotateToTarget = true;
            }
        }
        public void CloseCanRotate()
        {
            if (states)
            {
                states.canRotate = false;
            }
            if (eStates)
            {
                eStates.rotateToTarget = false;
            }
        }
        public void OpenDamageColliders()
        {
            if (states)
            {
                states.inventoryManager.OpenAllDamageColliders();
            }
            if (eStates)
            {
                eStates.OpenDamageCollider();
            }
            OpenParryFlag();

        }
        public void CloseDamageColliders()
        {
            if (states)
            {
                states.inventoryManager.CloseAllDamageColliders();
            }
            if (eStates)
            {
                eStates.CloseDamageCollider();
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
        public void CloseParticle()
        {
            if (states)
            {
                if (states.inventoryManager.currentSpell.currentParticle != null)
                    states.inventoryManager.currentSpell.currentParticle.SetActive(false);
            }
        }
        public void InitiateThrowForProjecttile()
        {
            if (states)
            {
                states.ThrowProjectile();
            }

        }
        public void InitIKForShield(bool isLeft)
        {
            ik_handler.UpdateIKTargets((isLeft) ? IKSnapShotType.shield_l : IKSnapShotType.shield_r, isLeft);
        }
        public void InitIKForBreathSpell(bool isLeft)
        {
            ik_handler.UpdateIKTargets(IKSnapShotType.breath, isLeft);
        }
        public void InitIKForSword(bool isLeft)//ยังไม่ได้ใช้
        {
            ik_handler.UpdateIKTargets((isLeft) ? IKSnapShotType.sword_l : IKSnapShotType.sword_r, isLeft);
        }
        public void ConsumeCurrentItem()
        {
            if (states)
            {
                if (states.inventoryManager.currentConsumable)
                {
                    states.inventoryManager.currentConsumable.itemCount--;
                    ItemEffectManager.singleton.UseItemEffect(states.inventoryManager.currentConsumable.instance.consumableEffect, states);
                }
            }

        }
    }
}