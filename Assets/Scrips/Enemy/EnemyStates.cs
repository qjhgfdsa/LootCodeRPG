using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.AI;


namespace SA
{
    public class EnemyStates : MonoBehaviour
    {
        [Header("Stats")]
        public int health;
        public float airTimer;
        public float poiseDegrade = 2;


        [Header("Values")]
        public float delta;
        public float horizontal;
        public float vertical;

        public CharacterStats characterStats;

        AIAttacks curAttack;
        public void SetCurAttack(AIAttacks a)
        {
            curAttack = a;
        }
        public AIAttacks GetCurAttack()
        {
            return curAttack;
        }
        public GameObject[] defaultDamageCollider;


        [Header("States")]
        public bool canBeParried = true;
        public bool parryIsOn = true;
        // public bool doParry = false;
        public bool isInvicible;
        public bool dontDoAnything;
        public bool canMove;
        public bool isDead;
        public bool hasDestination;
        public Vector3 targetDestination;

        public Vector3 dirToTarget;
        public bool rotateToTarget;


        public StateManager parriedBy;

        //references
        public Animator anim;
        EnemyTarget enTarget;
        AnimatorHook a_hook;
        public Rigidbody rigid;
        public NavMeshAgent agent;
        public bool AgentReady => agent != null && agent.enabled && agent.isOnNavMesh;

        public LayerMask ignoreLayers;

        List<Rigidbody> ragdollRigids = new List<Rigidbody>();
        List<Collider> ragdollColliders = new List<Collider>();

        const int ControllerLayer = 8;
        const int RagdollLayer = 10;
        const int GroundLayer = 0;
        static bool layerCollisionConfigured;
        static StateManager cachedPlayer;

        public delegate void SpellEffect_Loop();
        public SpellEffect_Loop spellEffect_loop;
        float timer;
        public void Init()
        {
            health = 100;
            anim = GetComponentInChildren<Animator>();
            enTarget = GetComponent<EnemyTarget>();
            enTarget.Init(this);

            rigid = GetComponent<Rigidbody>();
            agent = GetComponent<NavMeshAgent>();
            rigid.isKinematic = true;

            a_hook = anim.GetComponent<AnimatorHook>();
            if (a_hook == null)
                a_hook = anim.gameObject.AddComponent<AnimatorHook>();
            a_hook.Init(null, this);

            InitRagdoll();
            parryIsOn = false;
            ignoreLayers = ~(1 << 9); 

            EnemyManager.singleton.enemyTargets.Add(transform.GetComponent<EnemyTarget>());
        }
        void InitRagdoll()
        {
            if (!layerCollisionConfigured)
            {
                Physics.IgnoreLayerCollision(ControllerLayer, RagdollLayer, true);
                Physics.IgnoreLayerCollision(9, RagdollLayer, true);
                layerCollisionConfigured = true;
            }

            Rigidbody[] rigs = GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < rigs.Length; i++)
            {
                if (rigs[i] == rigid)
                    continue;

                rigs[i].gameObject.layer = RagdollLayer;
                ragdollRigids.Add(rigs[i]);
                rigs[i].isKinematic = true;
                rigs[i].linearDamping = 2f;
                rigs[i].angularDamping = 2f;

                Collider col = rigs[i].GetComponent<Collider>();
                if (col == null) continue;
                col.isTrigger = true;
                ragdollColliders.Add(col);
            }
        }

        public void EnableRagdoll()
        {
            AIHandler ai = GetComponent<AIHandler>();
            Transform playerRoot = ai != null ? ai.target : null;
            if (ai != null) ai.enabled = false;

            if (AgentReady)
            {
                agent.isStopped = true;
                agent.enabled = false;
            }

            if (anim != null)
            {
                anim.enabled = false;
                if (a_hook != null)
                    a_hook.enabled = false;
            }

            CloseAllDamageColliders();
            RemoveFromLockOnTargets();

            Physics.SyncTransforms();

            for (int i = 0; i < ragdollColliders.Count; i++)
            {
                for (int j = i + 1; j < ragdollColliders.Count; j++)
                    Physics.IgnoreCollision(ragdollColliders[i], ragdollColliders[j], true);
            }

            IgnorePlayerCollisions(playerRoot);

            for (int i = 0; i < ragdollRigids.Count; i++)
            {
                Rigidbody rb = ragdollRigids[i];
                rb.gameObject.layer = RagdollLayer;
                rb.isKinematic = false;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                Collider col = rb.GetComponent<Collider>();
                if (col != null)
                    col.isTrigger = false;
            }

            Collider controllerCollider = rigid != null ? rigid.GetComponent<Collider>() : null;
            if (controllerCollider != null)
                controllerCollider.enabled = false;
            if (rigid != null)
                rigid.isKinematic = true;

            enabled = false;
        }

        void CloseAllDamageColliders()
        {
            if (defaultDamageCollider != null)
                ObjectListStatus(defaultDamageCollider, false);

            DamageCollider[] dcs = GetComponentsInChildren<DamageCollider>(true);
            for (int i = 0; i < dcs.Length; i++)
            {
                Collider col = dcs[i].GetComponent<Collider>();
                if (col != null)
                    col.enabled = false;
            }
        }

        void RemoveFromLockOnTargets()
        {
            EnemyTarget target = GetComponent<EnemyTarget>();
            if (target != null && EnemyManager.singleton != null)
                EnemyManager.singleton.enemyTargets.Remove(target);
        }

        void IgnorePlayerCollisions(Transform playerRoot)
        {
            if (playerRoot == null)
            {
                if (cachedPlayer == null)
                    cachedPlayer = FindAnyObjectByType<StateManager>();
                if (cachedPlayer != null)
                    playerRoot = cachedPlayer.transform;
            }

            if (playerRoot == null)
                return;

            Collider[] playerCols = playerRoot.GetComponentsInChildren<Collider>(true);
            for (int i = 0; i < ragdollColliders.Count; i++)
            {
                for (int p = 0; p < playerCols.Length; p++)
                {
                    if (playerCols[p] != null && ragdollColliders[i] != null)
                        Physics.IgnoreCollision(ragdollColliders[i], playerCols[p], true);
                }
            }
        }

        IEnumerator DestroyAfterDeath(float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(this.gameObject);
        }

        public void Tick(float d)
        {
            delta = d;
            canMove = anim.GetBool(StaticStrings.onEmpty);

            if (spellEffect_loop != null)
                spellEffect_loop();

            if (dontDoAnything)
            {
                dontDoAnything = !canMove;
                return;
            }

            if (rotateToTarget)
                LookTowardTarget();

            if (health <= 0)
            {
                if (!isDead)
                {
                    isDead = true;
                    if (ragdollRigids.Count != 0)
                    {
                        EnableRagdoll();
                    }
                    else
                    {
                        // fallback: no ragdoll bones — play death anim then destroy
                        if (AgentReady)
                        {
                            agent.isStopped = true;
                            agent.enabled = false;
                        }
                        anim.Play("Armature|Death", 1); // Override layer
                        StartCoroutine(DestroyAfterDeath(3f));
                    }
                }
                return;
            }

            if (isInvicible)
            {
                isInvicible = !canMove;
            }

            if (parriedBy != null && parryIsOn == false)
            {
                //parriedBy.parryTarget = null;
                parriedBy = null;

            }

            if (canMove)
            {
                parryIsOn = false;
                anim.applyRootMotion = false;
                MovementAnimations();
            }
            else
            {
                if (anim.applyRootMotion == false)
                    anim.applyRootMotion = true;
            }

            characterStats.poise -= delta * poiseDegrade;
            if (characterStats.poise < 0)
                characterStats.poise = 0;

        }
        public void MovementAnimations()
        {
            if (!AgentReady)
            {
                anim.SetFloat(StaticStrings.Vertical_Axis, 0, 0.2f, delta);
                return;
            }

            if (agent.isStopped || !agent.hasPath)
            {
                anim.SetFloat(StaticStrings.Vertical_Axis, 0, 0.2f, delta);
                return;
            }

            float square = agent.desiredVelocity.sqrMagnitude;
            float v = Mathf.Clamp(square, 0, 1);

            anim.SetFloat(StaticStrings.Vertical_Axis, v, 0.2f, delta);
            /* Vector3 desired = agent.desiredVelocity;
             Vector3 relative = transform.InverseTransformDirection(desired);

             float v = relative.z;
             float h = relative.x;

             v = Mathf.Clamp(v, -1, 1);
             h = Mathf.Clamp(h, -1, 1);

             anim.SetFloat(StaticStrings.Vertical_Axis, v, 0.2f, delta);
             anim.SetFloat(StaticStrings.Horizontal_Axis, h, 0.2f, delta);*/
        }
        bool HasAnimState(string stateName)
        {
            int hash = Animator.StringToHash(stateName);
            for (int i = 0; i < anim.layerCount; i++)
                if (anim.HasState(i, hash)) return true;
            return false;
        }

        void LookTowardTarget()
        {
            Vector3 dir = dirToTarget;
            dir.y = 0;
            if (dir == Vector3.zero)
                dir = transform.forward;
            Quaternion t = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, t, delta * 5);
        }
        public void SetDestination(Vector3 d)
        {
            if (hasDestination || !AgentReady)
                return;

            hasDestination = true;
            agent.isStopped = false;
            agent.SetDestination(d);
            targetDestination = d;
        }
        void DoAction()
        {
            anim.applyRootMotion = true;
            parryIsOn = true; // กำลังโจมตี — ผู้เล่นสามารถ parry ได้
            anim.Play("oh_attack_1");
            anim.SetBool(StaticStrings.canMove, false);

        }
        public void DoDamage(Action a, Weapon w,WeaponStats ws)
        {
            if (isDead || isInvicible)
                return;

         int damage = StatsCalculations.CalculateBaseDamage(ws, characterStats); //ยังไม่ได้ใช้ weaponStats

            characterStats.poise += damage;
         health -= damage;


            if (canMove || characterStats.poise > 100)
            {
                if (a.overrideDamageAnim)
                {
                    if (HasAnimState(a.damageAnim))
                        anim.Play(a.damageAnim);
                }
                else
                {
                    int ran = Random.Range(0, 100);
                    string tA = (ran > 50) ? StaticStrings.damage1 : StaticStrings.damage2;
                    if (HasAnimState(tA))
                        anim.Play(tA);
                }
            }

            isInvicible = true;
            anim.applyRootMotion = true;
            Debug.Log("Enemy Health: " + health);
        }
        public void DoDamage_()
        {
            if (isInvicible)
                return;

            anim.Play("damage_3");
        }
        public void CheckForParry(Transform target, StateManager states)
        {
            if (isDead || canBeParried == false || parryIsOn == false || isInvicible)
                return;

            Vector3 dir = transform.position - target.position;
            dir.Normalize();
            float dot = Vector3.Dot(target.forward, dir);
            if (dot < 0)
                return;

            isInvicible = true;
            anim.Play(StaticStrings.attack_interupt);
            anim.applyRootMotion = true;
            anim.SetBool(StaticStrings.canMove, false);
            //states.parryTarget = this;
            parriedBy = states;
            return;
        }
        public void IsGettingParried(Action a)
        {
            int damage = StatsCalculations.CalculateBaseDamage(null, characterStats, a.parryMultiplier); //ยังไม่ได้ใช้ weaponStats
            health -= damage;
            dontDoAnything = true;
            anim.SetBool(StaticStrings.canMove, false);
            anim.Play(StaticStrings.parry_receive);
            // Debug.Log("Enemy Got Parried!" + damage);
        }

        public void IsGettingBackStabbed(Action a)
        {

            int damage = StatsCalculations.CalculateBaseDamage(null, characterStats, a.backstabMultiplier);
            health -= damage;
            dontDoAnything = true;
            anim.SetBool(StaticStrings.canMove, false);
            anim.Play(StaticStrings.backstabed);
            // Debug.Log("Enemy Got Back Stabbed!" + damage);
        }
        public ParticleSystem fireParticle;
        float _t;

        public void OnFire()
        {
            if (fireParticle == null)
                return;

            if (_t < 3)
            {
                _t += Time.deltaTime;
                fireParticle.Emit(1);
            }
            else
            {
                _t = 0;
                spellEffect_loop = null;
            }
        }
        public void OpenDamageCollider()
        {
            if (curAttack == null)
                return;

            if (curAttack.isDefaultDamageCollider || curAttack.damageCollider.Length == 0)
            {
                ObjectListStatus(defaultDamageCollider, true);
            }
            else
            {
                ObjectListStatus(curAttack.damageCollider, true);
            }
        }
        public void CloseDamageCollider()
        {
            if (curAttack == null)
                return;

            if (curAttack.isDefaultDamageCollider || curAttack.damageCollider.Length == 0)
            {
                ObjectListStatus(defaultDamageCollider, false);
            }
            else
            {
                ObjectListStatus(curAttack.damageCollider, false);
            }
        }
        void ObjectListStatus(GameObject[] l, bool status)
        {
            for (int i = 0; i < l.Length; i++)
            {
                l[i].SetActive(status);
            }
        }
    }
}
