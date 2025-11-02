using UnityEngine;

namespace SA
{

    public class ParryCollider : MonoBehaviour
    {
        StateManager states;
        EnemyStates eStates;
        public void InitPlayer(StateManager st)
        {
            states = st;
        }
        public void InitEnemy(EnemyStates st)
        {
            eStates = st;
        }
        void OnTriggerEnter(Collider other)
        {
            if (states)
            {
                EnemyStates e_st = other.transform.GetComponentInParent<EnemyStates>();

                if (e_st != null)
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
