using UnityEngine;

namespace SA
{
    public class StateManager : MonoBehaviour
    {
        [Header("Model")]
        public GameObject activeModel;


        [Header("Inputs")]
        public float vertical;
        public float horizontal;
        public float moveAmount;
        public Vector3 moveDir;

        [Header("Move Speed")]
        public float moveSpeed = 2;
        public float runSpeed = 3.5f;
        public float rotateSpeed = 5;
        public float toGround = 0.5f;

        [Header("States")]
        public bool run;
        public bool onGround;
        public bool lockon;



        [HideInInspector]
        public Animator anim;
        [HideInInspector]
        public Rigidbody rigid;

        [HideInInspector]
        public float delta;
        [HideInInspector]
        public LayerMask ignoreLayers;


        public void Init()
        {
            SetupAnimator();
            rigid = GetComponent<Rigidbody>();
            rigid.angularDamping = 999;
            rigid.linearDamping = 4;
            rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            gameObject.layer = 8;
            ignoreLayers = ~(1 << 9);
            
           
        }

        void SetupAnimator()
        {
            if (activeModel == null)
            {
                anim = GetComponentInChildren<Animator>();
                if (anim == null)
                {
                    Debug.Log("No model found for " + gameObject.name);
                }
                else
                {
                    activeModel = anim.gameObject;
                }
            }

            if (anim == null)
                anim = activeModel.GetComponent<Animator>();
        }

        public void FixedTick(float d)
        {
            delta = d;

            rigid.linearDamping = (moveAmount > 0 || onGround == false) ? 0 : 4;


            float targetSpeed = moveSpeed;
            if (run)
                targetSpeed = runSpeed;

            if (onGround)   
                rigid.linearVelocity = moveDir * (targetSpeed * moveAmount);

            if (run)
                lockon = false;

            if (!lockon)
                {
                    Vector3 targetDir = moveDir;
                    targetDir.y = 0;
                    if (targetDir == Vector3.zero)
                        targetDir = transform.forward;
                    Quaternion tr = Quaternion.LookRotation(targetDir);
                    Quaternion targetRotation = Quaternion.Slerp(transform.rotation, tr, rotateSpeed * moveAmount * delta);
                    transform.rotation = targetRotation;
                }
            
            HandleMovementAnimetions();

        }

        public void Tick(float d)
        {
            delta = d;
            onGround = OnGround();

            anim.SetBool("onGround", onGround);
        }
        void HandleMovementAnimetions()
        {
            anim.SetFloat("vertical", moveAmount, 0.4f, delta);
            anim.SetBool("run", run);
        }

        public bool OnGround()
        {
            bool r = false;

            Vector3 origin = transform.position + (Vector3.up * toGround);
            Vector3 dir = -Vector3.up;
            float dis = toGround + 0.3f;
            RaycastHit hit;
            if (Physics.Raycast(origin, dir, out hit, dis, ignoreLayers))
            {
                r = true;
                Vector3 tagetPostition = hit.point;
                transform.position = tagetPostition;

            }

            return r;
        }
    }
}