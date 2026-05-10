using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace SA
{
    public class HandelIK : MonoBehaviour
    {
        Animator anim;

        Transform handHelper;
        Transform bodyHelper;
        Transform headHelper;
        Transform shoulderHelper;
        Transform animShoulder;
        Transform headTrans;

        public float weight;

        public IKSnapShot[] iKSnapShots;
        public Vector3 defaultHeadPos;
        IKSnapShot GetSnapshot(IKSnapShotType type)
        {
            for (int i = 0; i < iKSnapShots.Length; i++)
            {
                if (iKSnapShots[i].type == type)
                {
                    return iKSnapShots[i];
                }
            }
            return null;
        }

        public void Init(Animator a)
        {
            anim = a;

            headHelper = new GameObject().transform;
            headHelper.name = "head helper";
            handHelper = new GameObject().transform;
            handHelper.name = "hand helper";
            bodyHelper = new GameObject().transform;
            bodyHelper.name = "body helper";
            shoulderHelper = new GameObject().transform;
            shoulderHelper.name = "shoulder helper";

            shoulderHelper.parent = transform.parent;
            shoulderHelper.localPosition = Vector3.zero;
            shoulderHelper.localRotation = Quaternion.identity;

            headHelper.parent = shoulderHelper;
            bodyHelper.parent = shoulderHelper;
            handHelper.parent = shoulderHelper;

            headTrans = anim.GetBoneTransform(HumanBodyBones.Head);
        }
        public void UpdateIKTargets(IKSnapShotType type, bool isLeft)
        {
            IKSnapShot snap = GetSnapshot(type);

            Vector3 targetBodyPos = snap.bodyPos;
            if (isLeft)
                targetBodyPos.x = -targetBodyPos.x;

            bodyHelper.localPosition = targetBodyPos;

            handHelper.localPosition = snap.handPos;
            handHelper.localEulerAngles = snap.hand_eulers;

            if (snap.overwritHeadPos)
                headHelper.localPosition = snap.headPos;
            else
                headHelper.localPosition = defaultHeadPos;
        }
        public void OnAnimatorMoveTick(bool isLeft)
        {
            Transform shoulder = anim.GetBoneTransform(
              (isLeft) ? HumanBodyBones.LeftShoulder : HumanBodyBones.RightShoulder);
            
            shoulderHelper.transform.position = shoulder.position;
        }
        public void IKTick(AvatarIKGoal goal, float w)
        {

            weight = Mathf.Lerp(weight, w, Time.deltaTime * 5);


            Vector3 ikPos = handHelper.position;
            Quaternion ikRot = handHelper.rotation;

            if (goal == AvatarIKGoal.LeftHand)
            {
                ikPos = MirrorPointOnCharacterLocalX(ikPos);
                ikRot = MirrorRotationOnCharacterLocalX(ikRot);
            }

            anim.SetIKPositionWeight(goal, weight);
            anim.SetIKRotationWeight(goal, weight);

            anim.SetIKPosition(goal, ikPos);
            anim.SetIKRotation(goal, ikRot);


            //หาก clamp เต็ม เลยแทบไม่หมุน “ตัว” แม้ Hand IK จะทำงานปกติ
            //ค่าตัวสุดท้าย (clampWeight = 1) คือบังคับไม่ให้หมุนเกือบทั้งหมด
            //ผลคือ LookAt (ลำตัว/หัว/ตา) ดูเหมือนไม่ทำงาน
            //ให้ลองปรับเป็นแบบนี้: anim.SetLookAtWeight(b_w, 0.8f, 1f, 0f, 0.2f);
            anim.SetLookAtWeight(weight, 0.2f, 1, 1, 1f);
            anim.SetLookAtPosition(bodyHelper.position);
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
        public void LateTick()
        {
            if (headHelper == null || headTrans == null)
                return;

            Vector3 direction = headHelper.position - headTrans.position;
            if (direction == Vector3.zero)
                direction = headTrans.forward;

            Quaternion targtRot = Quaternion.LookRotation(direction);
            Quaternion curRot = Quaternion.Slerp(headTrans.rotation, targtRot, weight);
            headTrans.rotation = curRot;
        }
    }
    public enum IKSnapShotType
    {
        breath, shield_r, shield_l
    }
    [System.Serializable]
    public class IKSnapShot
    {
        public IKSnapShotType type;
        public Vector3 handPos;
        public Vector3 hand_eulers;
        public Vector3 bodyPos;

        public bool overwritHeadPos;
        public Vector3 headPos;
    }
}