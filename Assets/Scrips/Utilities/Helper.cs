using UnityEngine;


namespace SA
{
    public class Helper : MonoBehaviour
    {
        [Range(-1, 1)]
        public float vertical;
         [Range(-1, 1)]
        public float horizontal;

        
        public bool playAnim;
        public string[] oh_attacks;
        public string[] th_attacks;

        public bool twoHanded;
        public bool enableRM;
        public bool useItem;
        public bool interacting;
        public bool lockon;

        Animator anim;
        void Start()
        {
            anim = GetComponent<Animator>();
        }

        void Update()
        {

            enableRM = !anim.GetBool("canMove");
            anim.applyRootMotion = enableRM;

            interacting = anim.GetBool("interacting");

            if (lockon == false)
            {
                horizontal = 0;
                vertical = Mathf.Clamp01(vertical);
            }
            anim.SetBool("lockOn", lockon);

            if (enableRM)
                return;

            if (useItem)
            {
                anim.Play("use_item");
                useItem = false;
            }

            if (interacting)
            {
                playAnim = false;
                vertical = Mathf.Clamp(vertical,0,0.5f);
            }


            anim.SetBool("two_handed", twoHanded);

            if (playAnim)
            {
                string targetAnim;
                if (!twoHanded)
                {
                    int r = Random.Range(0, th_attacks.Length);
                    targetAnim = th_attacks[r];

                    if (vertical > 0.5f)
                        targetAnim = "oh_attack_3";

                }
                else
                {
                    int r = Random.Range(0, th_attacks.Length);
                    targetAnim = th_attacks[r];
                }

                if (vertical > 0.5f)
                    targetAnim = "oh_attack_3";

                vertical = 0;

                anim.CrossFade(targetAnim, 0.2f);
                //anim.SetBool("canMove", false);
                //enableRM = true;
                playAnim = false;
            }
            anim.SetFloat("vertical", vertical);
            anim.SetFloat("horizontal", horizontal);

           

            
            input();

        }


        void input()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Space key was pressed.");
            }

            if (Input.GetMouseButtonDown(0))  // 0 = ซ้าย, 1 = ขวา, 2 = กลาง
            {
                Debug.Log("Left mouse clicked!");
            }
        }
    }

}