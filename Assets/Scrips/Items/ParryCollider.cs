using UnityEngine;

namespace SA
{

    public class ParryCollider : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            EnemyStates e_st = other.transform.GetComponentInParent<EnemyStates>();
            if (e_st == null)
                return;
            e_st.CheckForParry(transform.root);

        }
    }
}
