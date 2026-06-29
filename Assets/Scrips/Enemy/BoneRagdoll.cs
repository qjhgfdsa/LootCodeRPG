using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    // Attach to the same root GameObject as EnemyStates (e.g. "Boss").
    // Awake() runs before EnemyStates.InitRagdoll() and builds a stable ragdoll skeleton.
    public class BoneRagdoll : MonoBehaviour
    {
        const int RagdollLayer = 10;

        [Header("Collider")]
        [Tooltip("Local-space radius for each bone capsule. Lower = less inter-bone overlap.")]
        public float boneRadius = 0.03f;

        [Header("Rigidbody")]
        public float boneMass = 3f;
        public float linearDamping = 2f;
        public float angularDamping = 2f;

        [Header("Joints")]
        public float twistLimit = 18f;
        public float swingLimit = 22f;
        public float jointSpring = 80f;
        public float jointDamper = 8f;

        [Header("Bone Filter")]
        [Tooltip("Only ragdoll bones whose name contains one of these keywords.")]
        public bool majorBonesOnly = true;

        public string[] majorKeywords =
        {
            "hips", "pelvis", "spine", "chest", "neck", "head",
            "thigh", "calf", "upperarm", "lowerarm", "shoulder", "hand", "foot",
            "arm", "leg", "root"
        };

        [Tooltip("Skip bones whose name contains any of these substrings.")]
        public string[] skipKeywords =
        {
            "_end", "end_", "ik_", "_ik", "twist", "pole", "weapon",
            "tongue", "eye", "finger", "toe", "tip", "claw", "wing", "tail", "horn"
        };

        void Awake()
        {
            if (!BuildRagdoll(majorBonesOnly) && majorBonesOnly)
                BuildRagdoll(false);
        }

        bool BuildRagdoll(bool majorOnly)
        {
            SkinnedMeshRenderer smr = GetComponentInChildren<SkinnedMeshRenderer>();
            if (smr == null || smr.bones.Length == 0)
            {
                Debug.LogWarning($"BoneRagdoll on '{name}': SkinnedMeshRenderer not found.");
                return false;
            }

            var rigidMap = new Dictionary<Transform, Rigidbody>();

            foreach (Transform bone in smr.bones)
            {
                if (bone == null) continue;
                if (ShouldSkip(bone.name)) continue;
                if (majorOnly && !IsMajorBone(bone.name)) continue;
                if (bone.GetComponent<Rigidbody>() != null) continue;

                CapsuleCollider col = bone.gameObject.AddComponent<CapsuleCollider>();
                col.radius = boneRadius;
                col.height = Mathf.Max(boneRadius * 2.5f, boneRadius + 0.01f);
                col.isTrigger = true;

                Rigidbody rb = bone.gameObject.AddComponent<Rigidbody>();
                rb.mass = boneMass;
                rb.isKinematic = true;
                rb.linearDamping = linearDamping;
                rb.angularDamping = angularDamping;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                rb.gameObject.layer = RagdollLayer;

                rigidMap[bone] = rb;
            }

            if (rigidMap.Count == 0)
                return false;

            foreach (var kvp in rigidMap)
            {
                Rigidbody parentRb = FindAncestorRigidbody(kvp.Key.parent, rigidMap);
                if (parentRb == null) continue;

                CharacterJoint joint = kvp.Key.gameObject.AddComponent<CharacterJoint>();
                joint.connectedBody = parentRb;
                joint.enableCollision = false;
                joint.enableProjection = true;
                joint.projectionDistance = 0.1f;
                joint.projectionAngle = 180f;

                joint.swingLimitSpring = new SoftJointLimitSpring { spring = jointSpring, damper = jointDamper };
                joint.twistLimitSpring  = new SoftJointLimitSpring { spring = jointSpring, damper = jointDamper };

                SoftJointLimit swing = new SoftJointLimit { limit = swingLimit };
                SoftJointLimit twistHigh = new SoftJointLimit { limit = twistLimit };
                SoftJointLimit twistLow  = new SoftJointLimit { limit = -twistLimit };

                joint.swing1Limit = swing;
                joint.swing2Limit = swing;
                joint.highTwistLimit = twistHigh;
                joint.lowTwistLimit  = twistLow;
            }

            Debug.Log($"BoneRagdoll: {rigidMap.Count} bones on '{name}' (majorOnly={majorOnly}).");
            return true;
        }

        Rigidbody FindAncestorRigidbody(Transform t, Dictionary<Transform, Rigidbody> map)
        {
            while (t != null)
            {
                if (map.TryGetValue(t, out Rigidbody rb)) return rb;
                t = t.parent;
            }
            return null;
        }

        bool IsMajorBone(string boneName)
        {
            string lower = boneName.ToLower();
            foreach (string kw in majorKeywords)
                if (lower.Contains(kw)) return true;
            return false;
        }

        bool ShouldSkip(string boneName)
        {
            string lower = boneName.ToLower();
            foreach (string kw in skipKeywords)
                if (lower.Contains(kw)) return true;
            return false;
        }
    }
}
