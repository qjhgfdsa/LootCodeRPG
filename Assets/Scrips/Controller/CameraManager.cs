using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;


namespace SA
{
    public class CameraManager : MonoBehaviour
    {
        public bool lockon;
        public float followSpeed = 9;
        public float mouseSpeed = 2; // แก้จาก mouseSeed
        public Transform target;
        public EnemyTarget lockonTarget;
        public Transform lockonTransform;
        
        [HideInInspector]
        public Transform pivot;
        [HideInInspector]
        public Transform camTrans;
        
        StateManager states;

        float turnSmoothing = .1f; // แก้จาก turnSmooting
        public float minAngle = -35;
        public float maxAngle = 35;
        float smoothX;
        float smoothY;
        float smoothXVelocity;
        float smoothYVelocity;
        public float lookAngle;
        public float tiltAngle;

        bool usedMouseAxis;
        bool changeTargetLeft;
        bool changeTargetRight;

        public static CameraManager singleton;

        void Awake()
        {
            // ป้องกัน duplicate singleton
            if (singleton != null && singleton != this)
            {
                Destroy(gameObject);
                return;
            }
            singleton = this;
        }

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

            changeTargetLeft = Input.GetKeyUp(KeyCode.L);
            changeTargetRight = Input.GetKeyUp(KeyCode.K);

            // จัดการ Lock-on Target
            HandleLockOnTarget(h);

            // เรียกฟังก์ชันหลัก
            FollowTarget(d);
            HandleRotations(d, v, h, mouseSpeed);
        }

        void HandleLockOnTarget(float h)
        {
            // ถ้าไม่มี lockon หรือไม่มี target ให้ clear
            if (!lockon || lockonTarget == null)
            {
                lockonTransform = null;
                if (states != null)
                    states.lockOnTransform = null;
                return;
            }

            // กำหนด lockonTransform ครั้งแรก
            if (lockonTransform == null)
            {
                lockonTransform = lockonTarget.GetTarget();
                if (states != null)
                    states.lockOnTransform = lockonTransform;
            }

            // เปลี่ยนเป้าหมายด้วย Mouse
            if (Mathf.Abs(h) > 0.6f && !usedMouseAxis)
            {
                Transform newTarget = lockonTarget.GetTarget(h < 0); // h < 0 = ซ้าย, h > 0 = ขวา
                
                if (newTarget != null)
                {
                    lockonTransform = newTarget;
                    if (states != null)
                        states.lockOnTransform = lockonTransform;
                }
                
                usedMouseAxis = true;
            }

            // เปลี่ยนเป้าหมายด้วยปุ่ม L/K
            if (changeTargetLeft || changeTargetRight)
            {
                Transform newTarget = lockonTarget.GetTarget(changeTargetLeft);
                
                if (newTarget != null)
                {
                    lockonTransform = newTarget;
                    if (states != null)
                        states.lockOnTransform = lockonTransform;
                }
            }

            // Reset flag เมื่อไม่กด
            if (usedMouseAxis && Mathf.Abs(h) < 0.6f)
            {
                usedMouseAxis = false;
            }
        }

        void FollowTarget(float d)
        {
            float speed = d * followSpeed;
            Vector3 targetPosition = Vector3.Lerp(transform.position, target.position, speed);
            transform.position = targetPosition;
        }

        void HandleRotations(float d, float v, float h, float targetSpeed)
        {
            // Smooth camera movement
            if (turnSmoothing > 0)
            {
                smoothX = Mathf.SmoothDamp(smoothX, h, ref smoothXVelocity, turnSmoothing);
                smoothY = Mathf.SmoothDamp(smoothY, v, ref smoothYVelocity, turnSmoothing);
            }
            else
            {
                smoothX = h;
                smoothY = v;
            }

            // Tilt angle (มองขึ้น-ลง)
            tiltAngle -= smoothY * targetSpeed;
            tiltAngle = Mathf.Clamp(tiltAngle, minAngle, maxAngle);
            pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);

            // Lock-on rotation
            if (lockon && lockonTransform != null)
            {
                Vector3 targetDir = lockonTransform.position - transform.position;
                targetDir.Normalize();

                if (targetDir != Vector3.zero)
                {
                    Quaternion targetRot = Quaternion.LookRotation(targetDir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, d * 9);
                    lookAngle = transform.eulerAngles.y;
                }
                
                return;
            }

            // Free camera rotation
            lookAngle += smoothX * targetSpeed;
            transform.rotation = Quaternion.Euler(0, lookAngle, 0);
        }
    }
}

        

    
