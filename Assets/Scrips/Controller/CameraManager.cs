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

        public float defZ;
        float curZ;
        public float zSpeed = 19;

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
            if (camTrans == null)
            {
                Debug.LogError("❌ Camera.main ไม่เจอ!");
                return;
            }
            pivot = camTrans.parent;
            curZ = defZ;
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
            HandlePivotPosition();
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
            if (target == null)
                return; // ไม่มี target ไม่ต้องทำอะไร

            float speed = d * followSpeed;
            Vector3 targetPosition = Vector3.Lerp(transform.position, target.position, speed);
            transform.position = targetPosition;
        }
        void HandleRotations(float d, float v, float h, float targetSpeed)
        {
            // เช็คก่อนใช้
            if (pivot == null)
                return;
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
        void HandlePivotPosition()
        {
            float targetZ = defZ;
            CameraCollision(defZ, ref targetZ);

            curZ = Mathf.Lerp(curZ, targetZ, states.delta * zSpeed);
            Vector3 tp = Vector3.zero;
            tp.z = curZ;
            camTrans.localPosition = tp;
        }
        void CameraCollision(float targetZ, ref float actualZ)
        {
            float step = Mathf.Abs(targetZ);
            int stepCount = 2;
            float stepIncrement = step / stepCount;

            RaycastHit hit;
            Vector3 origin = pivot.position;
            Vector3 direction = -pivot.forward;

            if (Physics.Raycast(origin, direction, out hit, step, states.ignoreLayers))
            {
                float distance = Vector3.Distance(hit.point, origin);
                actualZ = -(distance / 2);
            }
            else
            {
                for (int s = 0; s < stepCount+1; s++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Vector3 dir = Vector3.zero;
                        Vector3 secondOrigin = origin + (direction * s) * stepIncrement;

                        switch (i)
                        {
                            case 0:
                                dir = camTrans.right;
                                break;
                            case 1:
                                dir = -camTrans.right;
                                break;
                            case 2:
                                dir = camTrans.up;
                                break;
                            case 3:
                                dir = -camTrans.up;
                                break;
                        }
                        if (Physics.Raycast(secondOrigin, dir, out hit, 0.5f, states.ignoreLayers))
                        {
                            float distance = Vector3.Distance(secondOrigin, origin);
                            actualZ = -(distance / 2);
                            if (actualZ < 0.2f)
                                actualZ = 0;
                            return;
                        }
                    }

                }
            }
        }
    }
}




