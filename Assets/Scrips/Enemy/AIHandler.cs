using UnityEngine;
using System.Collections.Generic;

namespace SA
{
    public class AIHandler : MonoBehaviour
    {
        public AIAttacks[] ai_Attacks;
        public EnemyStates states;
        public StateManager en_states;
        public Transform target;
        public float sight;
        public float fov_angle;


        public int closeCount = 10;
        int _close;

        public int frameCount = 30;
        int _frame;

        public int attackCount = 30;
        int _attack;

        float dis;
        float angle;
        float delta;
        Vector3 dirToTarget;

        float distanceFromTarget()
        {
            if (target == null)
                return 100;

            return Vector3.Distance(target.position, transform.position);
        }
        float angleToTarget()
        {
            float a = 180;
            if (target)
            {
                Vector3 d = dirToTarget;
                a = Vector3.Angle(d, transform.forward);
            }
            return a;
        }


        void Start()
        {
            if (states == null)
                states = GetComponent<EnemyStates>();

            states.Init();
        }
        public AIState aiState;
        public enum AIState
        {
            far, close, inSight, attacking
        }
        void Update()
        {
            delta = Time.deltaTime;
            dis = distanceFromTarget();
            angle = angleToTarget();

            if (target != null)
                dirToTarget = target.position - transform.position;
            states.dirToTarget = dirToTarget;

            switch (aiState)
            {
                case AIState.far:
                    HandleFarSight();
                    break;
                case AIState.close:
                    HandleCloseSight();
                    break;
                case AIState.inSight:
                    InSight();
                    break;
                case AIState.attacking:
                    if (states.canMove)
                    {
                        aiState = AIState.inSight;
                        states.agent.isStopped = false;
                        states.rotateToTarget = true;
                       // states.agent.enabled = true;
                    }
                    break;
                default:
                    break;
            }
            states.Tick(delta);
        }
        void HandleCloseSight()
        {
            _close++;
            if (_close > closeCount)
            {
                _close = 0;

                if (dis > sight || angle > fov_angle)
                {
                    aiState = AIState.far;
                    return;

                }
            }
            RaycastToTarget();
        }
        void GoToTarget()
        {
            states.hasDestination = false;
            states.SetDestination(target.position);
        }
        void InSight()
        {

            HandleCooldowns();


            float d2 = Vector3.Distance(states.targetDestination, transform.position);
            if (d2 > 2 || dis > sight * .5)
                GoToTarget();

            if (dis < 2)
                states.agent.isStopped = true;

            if (_attack > 0)
            {
                _attack--;
                return;
            }
            _attack = attackCount;

            AIAttacks attack = WillAttack();

            if (attack != null)
            {
                aiState = AIState.attacking;
                states.anim.Play(attack.targetAnim);
                states.anim.SetBool(StaticStrings.onEmpty, false);
                states.canMove = false;
                attack._cool = attack.cooldown;
                states.agent.isStopped = true;
                states.rotateToTarget = false;
                return;
            }

        }
        void HandleCooldowns()
        {
            for (int i = 0; i < ai_Attacks.Length; i++)
            {
                AIAttacks a = ai_Attacks[i];
                if (a._cool > 0)
                {
                    a._cool -= delta;
                    if (a._cool < 0)
                        a._cool = 0;
                }
            }
        }
      
        public AIAttacks WillAttack()
        {
            int w = 0;
            List<AIAttacks> l = new List<AIAttacks>();

            for (int i = 0; i < ai_Attacks.Length; i++)
            {
                AIAttacks a = ai_Attacks[i];
                if (a._cool > 0)
                {
                    continue;
                }

                if (dis > a.minDistance)
                    continue;
                if (angle < a.minAngle)
                    continue;
                if (angle > a.maxAngle)
                    continue;
                if (a.weight == 0)
                    continue;

                w += a.weight;
                l.Add(a);
            }

            if (l.Count == 0)
                return null;

            int ran = Random.Range(0, w + 1);
            int c_w = 0;

            for (int i = 0; i < l.Count; i++)
            {
                c_w += l[i].weight;
                if (c_w > ran)
                {
                    return l[i];
                }
            }
            return null;
        }

        void RaycastToTarget()
        {
            RaycastHit hit;
            Vector3 origin = transform.position;
            origin.y += 0.5f;
            Vector3 dir = dirToTarget;
            dir.y += 0.5f;
            if (Physics.Raycast(origin, dir, out hit, sight, states.ignoreLayers))
            {
                StateManager st = hit.transform.GetComponentInParent<StateManager>();
                if (st != null)
                {
                    states.rotateToTarget = true;
                    aiState = AIState.inSight;
                    states.SetDestination(target.position);
                }
            }

        }
        void HandleFarSight()
        {
            if (target == null)
                return;

            _frame++;
            if (_frame > frameCount)
            {
                _frame = 0;

                if (dis < sight)
                {
                    if (angle < fov_angle)
                    {
                        aiState = AIState.close;
                    }
                }

            }
        }
    }
    [System.Serializable]
    public class AIAttacks
    {
        public int weight;
        public float minDistance;
        public float minAngle;
        public float maxAngle;

        public float cooldown = 2;
        public float _cool;
        public string targetAnim;
    }
}