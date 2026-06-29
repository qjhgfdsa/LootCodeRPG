using UnityEngine;
using UnityEngine.Animations;

namespace SA
{

    public class ParryCollider : MonoBehaviour
    {
        StateManager states;
        EnemyStates eStates;
        public float maxTimer = 0.6f;
        float timer;
        public void InitPlayer(StateManager st)
        {
            states = st;
        }
        void Update()
        {
            if (states)
            {
                timer += states.delta;
                if (timer > maxTimer)
                {
                    gameObject.SetActive(false);
                    timer = 0;
                }
            }
            
            if (eStates)
            {
                timer += eStates.delta;
                if (timer > maxTimer)
                {
                    gameObject.SetActive(false);
                    timer = 0;
                }
            }
        }
        public void InitEnemy(EnemyStates st)
        {
            eStates = st;
        }
        void OnTriggerEnter(Collider other)
        {
           // DamageCollider dc = other.GetComponent<DamageCollider>();
            //if (dc == null)
              //  return;

            if (states)
            {
                EnemyStates e_st = other.transform.GetComponentInParent<EnemyStates>();

                if (e_st != null && !e_st.isDead)
                {
                     e_st.CheckForParry(transform.root, states);
                }

            }
            if(eStates)
            {
                // Enemy is parrying the player
            }
        }
    }
}
