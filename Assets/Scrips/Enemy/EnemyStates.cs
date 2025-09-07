using UnityEngine;


namespace SA
{
    public class EnemyStates : MonoBehaviour
    {
        public float health;
        public bool isInvicible;

        Animator anim;
        EnemyTarget enTarget;

        void Start()
        {
            anim = GetComponentInChildren<Animator>();
            enTarget = GetComponent<EnemyTarget>();
            enTarget.Init(anim);
        }

        void Update()
        {
            if (isInvicible)
            {
                 isInvicible = !anim.GetBool("canMove"); 
            }
           

        }



        public void DoDamage(float v)
        {
            if (isInvicible)
                return;

            health -= v;
            isInvicible = true;
            anim.Play("damage_01");
            Debug.Log("Enemy Health: " + health);
        }




    }
}
