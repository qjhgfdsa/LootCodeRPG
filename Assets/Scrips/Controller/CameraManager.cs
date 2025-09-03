using Unity.VisualScripting;
using UnityEngine;


namespace SA
{
    public class CameraManager : MonoBehaviour
    {
        public bool lockon;
        public float followSpeed = 9;
        public float mouseSeed = 2;
        //public float controllerSeed = 2;
        public Transform target;
        public EnemyTarget lockonTarget;
        public Transform lockonTransform;
        
        [HideInInspector]
        public Transform pivot;
        [HideInInspector]
        public Transform camTrans;
        StateManager states;

        float turnSmooting = .1f;
        public float minAngle = -35;
        public float maxAngle = 35;
        float smoothX;
        float smoothY;
        float smoothXVelocity;
        float smoothYVelocity;
        public float lookAngle;
        public float tiltAngle;

        bool usedMouseAxis;

      



        public void Init(StateManager st)
        {
            states = st;
            target = st.transform;

            camTrans = Camera.main.transform;
            pivot = camTrans.parent;
        }

        public void Tick(float d)
        {

            float h = Input.GetAxis("Mouse X");
            float v = Input.GetAxis("Mouse Y");

            float targetSpeed = mouseSeed;

            if (lockonTarget != null && states != null) // เช็คก่อนใช้งานทุกอย่าง
            {
                if (lockonTransform == null)
                {
                    lockonTransform = lockonTarget.GetTarget();
                    states.lockOnTransform = lockonTransform;
                }

                // ย้ายโค้ดเปลี่ยนเป้าหมายเข้ามาใน if นี้
                if (Mathf.Abs(h) > 0.6f)
                {
                    if (!usedMouseAxis)
                    {
                        if (h > 0)
                        {
                            // เมาส์เลื่อนขวา = หาเป้าหมายถัดไป
                            lockonTransform = lockonTarget.GetTarget();
                        }
                        else
                        {
                            // เมาส์เลื่อนซ้าย = หาเป้าหมายก่อนหน้า
                            lockonTransform = lockonTarget.GetTarget(true);
                        }
                        states.lockOnTransform = lockonTransform;
                        usedMouseAxis = true;
                    }
                }

                if (usedMouseAxis)
                {
                    if (Mathf.Abs(h) < 0.6f)
                    {
                        usedMouseAxis = false;
                    }
                }
            }

            // เรียกฟังก์ชันอื่นๆ ที่จำเป็น
            FollowTarget(d);
            HandleRotations(d, v, h, targetSpeed);
        }


        void FollowTarget(float d)
        {
            float speed = d * followSpeed;
            Vector3 targetPosition = Vector3.Lerp(transform.position, target.position, speed);
            transform.position = targetPosition;
        }

        void HandleRotations(float d, float v, float h, float targetSpeed)
        {
            if (turnSmooting > 0)
            {
                smoothX = Mathf.SmoothDamp(smoothX, h, ref smoothXVelocity, turnSmooting);
                smoothY = Mathf.SmoothDamp(smoothY, v, ref smoothYVelocity, turnSmooting);
            }
            else
            {
                smoothX = h;
                smoothY = v;
            }

            tiltAngle -= smoothY * targetSpeed;
            tiltAngle = Mathf.Clamp(tiltAngle, minAngle, maxAngle);
            pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);

            if (lockon && lockonTarget != null)
            {
                Vector3 targetDir = lockonTransform.position - transform.position;
                targetDir.Normalize();
                //targetDir.y = 0;

                if (targetDir == Vector3.zero)
                    targetDir = transform.forward;
                Quaternion targetRot = Quaternion.LookRotation(targetDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, d * 9);
                lookAngle = transform.eulerAngles.y;

                return;

            }
          

            lookAngle += smoothX * targetSpeed;
            transform.rotation = Quaternion.Euler(0, lookAngle, 0);

           

        }

        public static CameraManager singleton;
        void Awake()
        {
            singleton = this;
        }
        

    }


}