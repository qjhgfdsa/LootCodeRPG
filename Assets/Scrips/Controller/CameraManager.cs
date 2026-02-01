using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;


namespace SA
{
   /* public class CameraManager : MonoBehaviour
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

        bool changeTargetLeft;
        bool changeTargetRight;

      



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

            changeTargetLeft = Input.GetKeyUp(KeyCode.L);
            changeTargetRight = Input.GetKeyUp(KeyCode.K);

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
                       /* if(!usedMouseAxis)
                        {
                            lockonTransform = lockonTarget.GetTarget(h > 0);
                            states.lockOnTransform = lockonTransform;
                            usedMouseAxis = true;
                        } */
                       /* if (h > 0)
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

                /*if(changeTargetLeft || changeTargetRight)
                {
                    
                            lockonTransform = lockonTarget.GetTarget(changeTargetLeft);
                            states.lockOnTransform = lockonTransform;
                    
                }*/

              /*  if (usedMouseAxis)
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
        

    } */
  public class CameraManager : MonoBehaviour
    {
        // ─── Follow ───
        public float followSpeed = 9;
        public Transform target;

        // ─── Pivot / Cam refs ───
        [HideInInspector] public Transform pivot;
        [HideInInspector] public Transform camTrans;
        StateManager states;

        // ─── Rotation smoothing ───
        float turnSmoothing = .1f;
        float smoothX, smoothY;
        float smoothXVelocity, smoothYVelocity;
        public float lookAngle;
        public float tiltAngle;
        public float mouseSeed = 2;
        public float minAngle = -35;
        public float maxAngle = 35;

        // ─── Lock-On ───
        public bool lockon;
        public Transform lockonTransform;       // เป้าหมายที่ lock อยู่ (root ของ enemy)
        public EnemyTarget currentEnemyTarget;  // EnemyTarget script ของ enemy ที่ lock อยู่
        bool usedMouseAxis;

        // ─── Enemy Detection Layer ───
        [Header("Enemy Detection")]
        public LayerMask enemyLayer;            // assign ใน Inspector ครับ (เช่น "Enemy")

        // ─── Soft Lock (auto-face on attack) ───
        [Header("Soft Lock Settings")]
        public float softLockRadius = 8f;
        public float softLockAngle = 60f;       // มุม cone หน้า player
        public float softLockSmoothSpeed = 12f;

        // ─── Camera Targeting ───
        [Header("Camera Targeting")]
        public float cameraSearchAngle = 55f;   // มุม cone จาก camera forward

        // ─── Singleton ───
        public static CameraManager singleton;
        void Awake() { singleton = this; }

        // ════════════════════════════════════════
        public void Init(StateManager st)
        {
            states = st;
            target = st.transform;
            camTrans = Camera.main.transform;
            pivot = camTrans.parent;
        }

        // ════════════════════════════════════════
        public void Tick(float d)
        {
            float h = Input.GetAxis("Mouse X");
            float v = Input.GetAxis("Mouse Y");

            // ── Toggle lock-on กับ middle mouse ──
            if (Input.GetMouseButtonDown(2))
            {
                if (lockon)
                    ClearLockOn();
                else
                    TryLockOn();
            }

            // ── เปลี่ยน lock target ด้วย Mouse X เมื่อ lock อยู่ ──
            if (lockon && lockonTransform != null)
            {
                if (Mathf.Abs(h) > 0.6f)
                {
                    if (!usedMouseAxis)
                    {
                        SwitchLockTarget(h > 0);
                        usedMouseAxis = true;
                    }
                }
                else
                {
                    usedMouseAxis = false;
                }
            }

            // ── เช็ค enemy หายไปไหม ──
            if (lockon && lockonTransform == null)
                ClearLockOn();

            FollowTarget(d);
            HandleRotations(d, v, h);
        }

        // ════════════════════════════════════════
        void FollowTarget(float d)
        {
            transform.position = Vector3.Lerp(
                transform.position, target.position, d * followSpeed);
        }

        // ════════════════════════════════════════
        void HandleRotations(float d, float v, float h)
        {
            if (turnSmoothing > 0)
            {
                smoothX = Mathf.SmoothDamp(smoothX, h, ref smoothXVelocity, turnSmoothing);
                smoothY = Mathf.SmoothDamp(smoothY, v, ref smoothYVelocity, turnSmoothing);
            }
            else { smoothX = h; smoothY = v; }

            // ── Tilt ──
            tiltAngle -= smoothY * mouseSeed;
            tiltAngle = Mathf.Clamp(tiltAngle, minAngle, maxAngle);
            pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);

            // ── Hard Lock-On ──
            if (lockon && lockonTransform != null)
            {
                Vector3 dir = lockonTransform.position - transform.position;
                dir.Normalize();
                if (dir == Vector3.zero) dir = transform.forward;

                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, d * 9f);
                lookAngle = transform.eulerAngles.y;
                return;
            }

            // ── ปกติ ──
            lookAngle += smoothX * mouseSeed;
            transform.rotation = Quaternion.Euler(0, lookAngle, 0);
        }

        // ════════════════════════════════════════
        // กดปุ่ม lock ครั้งแรก → หาเป้าใน camera cone แล้ว lock
        // ════════════════════════════════════════
        void TryLockOn()
        {
            List<EnemyTarget> nearby = FindAllEnemiesInCamera();
            if (nearby.Count == 0) return;

            Camera cam = Camera.main;
            EnemyTarget best = null;
            float bestScore = float.MaxValue;

            foreach (var et in nearby)
            {
                if (et == null || et.eStates == null) continue;

                Transform eRoot = et.eStates.transform;

                // เช็ค อยู่ใน camera cone ไหม
                Vector3 dirToCam = eRoot.position - cam.transform.position;
                float angleToCam = Vector3.Angle(dirToCam, cam.transform.forward);
                if (angleToCam > cameraSearchAngle) continue;

                // project to screen
                Vector3 screenPos = cam.WorldToScreenPoint(eRoot.position);
                if (screenPos.z < 0) continue; // อยู่หลัง camera

                // distance จาก screen center
                Vector2 center = new Vector2(cam.pixelWidth * 0.5f, cam.pixelHeight * 0.5f);
                float screenDist = Vector2.Distance(new Vector2(screenPos.x, screenPos.y), center);
                float normalizedDist = screenDist / Mathf.Max(cam.pixelWidth, cam.pixelHeight);

                // บวก world distance เล็กน้อย เพื่อ prefer ใกล้กว่า
                float worldDist = Vector3.Distance(target.position, eRoot.position);
                float score = normalizedDist + worldDist * 0.05f;

                Debug.Log("ล็อคเเล้ว");

                if (score < bestScore)
                {
                    bestScore = score;
                    best = et;
                }
            }

            if (best != null)
            {
                lockon = true;
                currentEnemyTarget = best;
                lockonTransform = best.eStates.transform;
                if (states != null)
                    states.lockOnTransform = lockonTransform;
            }
        }

        // ════════════════════════════════════════
        // เปลี่ยน lock ไปยัง enemy ตัวอื่น (ซ้าย / ขวา)
        // ════════════════════════════════════════
        void SwitchLockTarget(bool next)
        {
            List<EnemyTarget> nearby = FindAllEnemiesInCamera();
            if (nearby.Count <= 1) return; // มีคนเดียวไม่ต้อง switch

            // เรียง sort ตาม angle จาก camera forward (ซ้าย-ขวา)
            Camera cam = Camera.main;
            nearby.Sort((a, b) =>
            {
                float angleA = GetSignedAngleFromCamera(a.eStates.transform, cam);
                float angleB = GetSignedAngleFromCamera(b.eStates.transform, cam);
                return angleA.CompareTo(angleB);
            });

            // หา index ของ current lock target
            int currentIndex = -1;
            for (int i = 0; i < nearby.Count; i++)
            {
                if (nearby[i] == currentEnemyTarget)
                { currentIndex = i; break; }
            }

            // ถ้าหา current ไม่เจ้อย เริ่มใหม่
            if (currentIndex < 0) currentIndex = 0;

            int newIndex = next
                ? (currentIndex + 1) % nearby.Count
                : (currentIndex - 1 + nearby.Count) % nearby.Count;

            EnemyTarget newTarget = nearby[newIndex];
            currentEnemyTarget = newTarget;
            lockonTransform = newTarget.eStates.transform;
            if (states != null)
                states.lockOnTransform = lockonTransform;
        }

        // ════════════════════════════════════════
        // หา signed angle ของ enemy จาก camera (สำหรับ sort ซ้าย-ขวา)
        // ════════════════════════════════════════
        float GetSignedAngleFromCamera(Transform enemyRoot, Camera cam)
        {
            Vector3 dir = enemyRoot.position - cam.transform.position;
            dir.y = 0;
            Vector3 camFwd = cam.transform.forward;
            camFwd.y = 0;
            return Vector3.SignedAngle(camFwd, dir, Vector3.up);
        }

        // ════════════════════════════════════════
        // หา EnemyTarget ทั้งหมดที่อยู่ใน camera view
        // ════════════════════════════════════════
        List<EnemyTarget> FindAllEnemiesInCamera()
        {
            List<EnemyTarget> result = new List<EnemyTarget>();
            Camera cam = Camera.main;

            // OverlapSphere ใช้ รัสมี camera frustum (เท่า far clip plane)
            Collider[] hits = Physics.OverlapSphere(
                cam.transform.position, cam.farClipPlane, enemyLayer);

            foreach (var col in hits)
            {
                EnemyTarget et = col.GetComponentInParent<EnemyTarget>();
                if (et == null || et.eStates == null || result.Contains(et)) continue;

                Transform eRoot = et.eStates.transform;

                // เช็ค อยู่ใน camera cone ไหม
                Vector3 dir = eRoot.position - cam.transform.position;
                float angle = Vector3.Angle(dir, cam.transform.forward);
                if (angle > cameraSearchAngle) continue;

                // เช็ค project to screen ว่า z > 0 (อยู่หน้า camera)
                Vector3 screenPos = cam.WorldToScreenPoint(eRoot.position);
                if (screenPos.z < 0) continue;

                result.Add(et);
            }
            return result;
        }

        // ════════════════════════════════════════
        // Soft Lock: หาเป้าใกล้ player ในมุม forward (เรียกจาก attack)
        // ════════════════════════════════════════
        public Transform FindSoftLockTarget()
        {
            List<EnemyTarget> nearby = FindAllEnemiesInCamera();
            if (nearby.Count == 0) return null;

            EnemyTarget best = null;
            float bestDist = float.MaxValue;

            foreach (var et in nearby)
            {
                if (et == null || et.eStates == null) continue;
                Transform eRoot = et.eStates.transform;

                float dist = Vector3.Distance(target.position, eRoot.position);
                if (dist > softLockRadius) continue;

                Vector3 dir = eRoot.position - target.position;
                float angle = Vector3.Angle(target.forward, dir);
                if (angle > softLockAngle) continue;

                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = et;
                }
            }

            return best?.eStates?.transform;
        }

        // ════════════════════════════════════════
        // หันหน้า player เข้าหา soft target (เรียกจาก attack state)
        // ════════════════════════════════════════
        public void FaceTarget(Transform t, float d)
        {
            if (t == null) return;
            Vector3 dir = t.position - target.position;
            dir.y = 0;
            if (dir == Vector3.zero) return;

            Quaternion rot = Quaternion.LookRotation(dir);
            target.rotation = Quaternion.Slerp(target.rotation, rot, d * softLockSmoothSpeed);

            // อัปเดต lookAngle ให้ camera ตาม
            lookAngle = target.eulerAngles.y;
        }

        // ════════════════════════════════════════
        void ClearLockOn()
        {
            lockon = false;
            lockonTransform = null;
            currentEnemyTarget = null;
            if (states != null)
                states.lockOnTransform = null;
            Debug.Log("ปลดล็อค");
        }
    }
}