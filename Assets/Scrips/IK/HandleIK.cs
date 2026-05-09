using UnityEngine;

namespace SA
{
    public class HandelIK : MonoBehaviour
    {
        Animator anim;

        Transform ikTarget;
        Transform bodyTarget;
        Vector3 bodyLocalStartPos;
        public float t;
        public void Init(Animator a, Transform ik_t, Transform b_target)
        {
            anim = a;
            ikTarget = ik_t;
            bodyTarget = b_target;

            if (bodyTarget != null)
                bodyLocalStartPos = bodyTarget.localPosition;
        }
        public void IKTick(AvatarIKGoal goal, float w)
        {
            if (anim == null || ikTarget == null || bodyTarget == null)
                return;

            t = Mathf.Lerp(t, w, Time.deltaTime);

            Vector3 targetPosition = bodyLocalStartPos;

            if (goal == AvatarIKGoal.LeftHand)
            {
                targetPosition.x = -targetPosition.x;
            }

            bodyTarget.localPosition = targetPosition;

            Vector3 ikPos = ikTarget.position;
            Quaternion ikRot = ikTarget.rotation;

            if (goal == AvatarIKGoal.LeftHand)
            {
                ikPos = MirrorPointOnCharacterLocalX(ikPos);
                ikRot = MirrorRotationOnCharacterLocalX(ikRot);
            }

            anim.SetIKPositionWeight(goal, t);
            anim.SetIKPosition(goal, ikPos);
            anim.SetIKRotation(goal, ikRot);
            anim.SetIKRotationWeight(goal, t);

            //หาก clamp เต็ม เลยแทบไม่หมุน “ตัว” แม้ Hand IK จะทำงานปกติ
            //ค่าตัวสุดท้าย (clampWeight = 1) คือบังคับไม่ให้หมุนเกือบทั้งหมด
            //ผลคือ LookAt (ลำตัว/หัว/ตา) ดูเหมือนไม่ทำงาน
            //ให้ลองปรับเป็นแบบนี้: anim.SetLookAtWeight(b_w, 0.8f, 1f, 0f, 0.2f);
            anim.SetLookAtWeight(t, 0.8f, 1, 1, 1f);
            anim.SetLookAtPosition(bodyTarget.position);
        }

        Vector3 MirrorPointOnCharacterLocalX(Vector3 worldPoint)
        {
            Transform root = anim.transform;
            Vector3 local = root.InverseTransformPoint(worldPoint);
            local.x = -local.x;
            return root.TransformPoint(local);
        }

        Quaternion MirrorRotationOnCharacterLocalX(Quaternion worldRotation)
        {
            Transform root = anim.transform;

            Vector3 forward = worldRotation * Vector3.forward;
            Vector3 up = worldRotation * Vector3.up;

            Vector3 localForward = root.InverseTransformDirection(forward);
            Vector3 localUp = root.InverseTransformDirection(up);

            localForward.x = -localForward.x;
            localUp.x = -localUp.x;

            Vector3 mirroredForward = root.TransformDirection(localForward);
            Vector3 mirroredUp = root.TransformDirection(localUp);

            if (mirroredForward.sqrMagnitude < 0.0001f || mirroredUp.sqrMagnitude < 0.0001f)
                return worldRotation;

            return Quaternion.LookRotation(mirroredForward, mirroredUp);
        }
    }
}