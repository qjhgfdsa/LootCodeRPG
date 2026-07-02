using UnityEngine;


namespace SA
{
    public class DamageCollider : MonoBehaviour
    {
        StateManager states;
        EnemyStates eStates;
        public void InitPlayer(StateManager st)
        {
            states = st;
            gameObject.layer = 9;
            gameObject.SetActive(false);
        }
        public void InitEnemy(EnemyStates es)
        {
            eStates = es;
            gameObject.layer = 9;
            gameObject.SetActive(false);

        }
        void OnTriggerEnter(Collider other)
        {
            if (states != null)
            {
                if (states.currentAction == null)
                    return;

                EnemyStates es = other.transform.GetComponentInParent<EnemyStates>();
                if (es == null || es.isDead)
                {
                    RuntimeWeapon rw = states.inventoryManager.GetRuntimeWeapon(states.currentAction.mirror);
                    if (rw == null || rw.weaponStats == null)
                        es.DoDamage(states.currentAction, rw.instance, rw.weaponStats);
                    return;
                }


                StateManager st = other.transform.GetComponentInParent<StateManager>();
                if (st != null)
                {
                    if (st != states)
                    {
                        st.DoDamage(states.currentAction,
                        states.inventoryManager.GetCurrentWeapon(states.currentAction.mirror));
                    }
                        
                }
                return;
            }

            if (eStates != null)
            {
                StateManager st = other.transform.GetComponentInParent<StateManager>();
                if (st != null)
                    st.DoDamage(eStates.GetCurAttack());
            }
        }
    }
}
