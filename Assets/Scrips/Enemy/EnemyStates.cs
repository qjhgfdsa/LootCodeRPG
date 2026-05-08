using System.Collections.Generic;
using UnityEngine;
using System.Collections;


namespace SA
{
    public class EnemyStates : MonoBehaviour
    {
        public int health;
        public CharacterStats characterStats;
        public bool canBeParried = true;
        public bool parryIsOn = true;
        public bool doParry = false;
        public bool isInvicible;
        public bool dontDoAnything;
        public bool canMove;
        public bool isDead;

        public StateManager parriedBy;

        public Animator anim;
        EnemyTarget enTarget;
        AnimatorHook a_hook;
        public Rigidbody rigid;
        public float delta;
        public float poiseDegrade = 2;

        List<Rigidbody> ragdollRigids = new List<Rigidbody>();
        List<Collider> ragdollColliders = new List<Collider>();

        public delegate void SpellEffect_Loop();
        public SpellEffect_Loop spellEffect_loop;
        float timer;
        void Start()
        {
            health = 10000;
            anim = GetComponentInChildren<Animator>();
            enTarget = GetComponent<EnemyTarget>();
            enTarget.Init(this);

            rigid = GetComponent<Rigidbody>();

            a_hook = anim.GetComponent<AnimatorHook>();
            if (a_hook == null)
                a_hook = anim.gameObject.AddComponent<AnimatorHook>();
            a_hook.Init(null, this);

            InitRagdoll();
            parryIsOn = false;
        }

        void InitRagdoll()
        {
            Rigidbody[] rigs = GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < rigs.Length; i++)
            {
                if (rigs[i] == rigid)
                {
                    continue;
                }
                ragdollRigids.Add(rigs[i]);
                rigs[i].isKinematic = true;

                Collider col = rigs[i].gameObject.GetComponent<Collider>();
                col.isTrigger = true;
                ragdollColliders.Add(col);
            }

        }

        public void EnableRagdoll()
        {

            for (int i = 0; i < ragdollRigids.Count; i++)
            {
                ragdollRigids[i].isKinematic = false;
                ragdollColliders[i].isTrigger = false;
            }

            Collider controllerCollider = rigid.gameObject.GetComponent<Collider>();
            controllerCollider.enabled = false;
            rigid.isKinematic = true;

            StartCoroutine(CloseAnimator());
        }

        IEnumerator CloseAnimator()
        {
            yield return new WaitForSeconds(0.1f);
            anim.enabled = false;
            this.enabled = false;
        }

        void Update()
        {
            delta = Time.deltaTime;
            canMove = anim.GetBool(StaticStrings.canMove);

            if (spellEffect_loop != null)
                spellEffect_loop();

            if (dontDoAnything)
            {
                dontDoAnything = !canMove;
                return;
            }

            if (health <= 0)
            {
                if (!isDead)
                {
                    isDead = true;
                    EnableRagdoll();
                }

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

                timer += Time.deltaTime;
                if (timer > 3)
                {
                    DoAction();
                    timer = 0;
                }
            }

            characterStats.poise -= delta * poiseDegrade;
            if (characterStats.poise < 0)
                characterStats.poise = 0;

        }

        void DoAction()
        {
            anim.applyRootMotion = true;
            parryIsOn = true; // กำลังโจมตี — ผู้เล่นสามารถ parry ได้
            anim.Play("oh_attack_1");
            anim.SetBool(StaticStrings.canMove, false);

        }
        public void DoDamage(Action a)
        {
            if (isInvicible)
                return;

            int damage = StatsCalculations.CalculateBaseDamage(a.weaponStats, characterStats);

            characterStats.poise += damage;
            health -= damage;

            if (canMove || characterStats.poise > 100)
            {
                if (a.ovverideDamageAnim)
                    anim.Play(a.damageAnim);
                else
                {
                    int ran = Random.Range(0, 100);
                    string tA = (ran > 50) ? StaticStrings.damage1 : StaticStrings.damage2;
                    anim.Play(tA);
                }
            }

            //  Debug.Log(" Damage is " + damage + " Poise is " + characterStats.poise);

            isInvicible = true;
            anim.applyRootMotion = true;
            anim.SetBool(StaticStrings.canMove, false);
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
            if (canBeParried == false || parryIsOn == false || isInvicible)
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
            int damage = StatsCalculations.CalculateBaseDamage(a.weaponStats, characterStats, a.parryMultiplier);
            health -= damage;
            dontDoAnything = true;
            anim.SetBool(StaticStrings.canMove, false);
            anim.Play(StaticStrings.parry_receive);
            // Debug.Log("Enemy Got Parried!" + damage);
        }

        public void IsGettingBackStabbed(Action a)
        {

            int damage = StatsCalculations.CalculateBaseDamage(a.weaponStats, characterStats, a.backstabMultiplier);
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
    }
}
