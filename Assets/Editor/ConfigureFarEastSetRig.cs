#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
static class ConfigureFarEastSetRigAutoRun
{
    const string PendingFile = "Assets/Editor/.configure_fareast_rig_pending";

    static ConfigureFarEastSetRigAutoRun()
    {
        EditorApplication.delayCall += () =>
        {
            if (!File.Exists(PendingFile))
                return;

            File.Delete(PendingFile);
            ConfigureFarEastSetRig.Execute();
        };
    }
}

public static class ConfigureFarEastSetRig
{
    const string ModelPath = "Assets/Model/Far East Set/far east set.fbx";

    static readonly (string humanName, string boneName)[] BoneMap =
    {
        ("Hips", "Pelvis"),
        ("LeftUpperLeg", "Left_Thigh"),
        ("RightUpperLeg", "Right_Thigh"),
        ("LeftLowerLeg", "Left_Calf"),
        ("RightLowerLeg", "Right_Calf"),
        ("LeftFoot", "Left_Foot"),
        ("RightFoot", "Right_Foot"),
        ("LeftToes", "Left_Toe"),
        ("RightToes", "Right_Toe"),
        ("Spine", "Spine"),
        ("Chest", "Spine_1"),
        ("UpperChest", "Spine_2"),
        ("Neck", "Neck"),
        ("Head", "Head"),
        ("LeftShoulder", "Left_Clavicle"),
        ("RightShoulder", "Right_Clavicle"),
        ("LeftUpperArm", "Left_Upper_Arm"),
        ("RightUpperArm", "Right_Upper_Arm"),
        ("LeftLowerArm", "Left_ForeArm"),
        ("RightLowerArm", "Right_Forearm"),
        ("LeftHand", "LeftHand"),
        ("RightHand", "RightHand"),
        ("Left Thumb Proximal", "LeftHandThumb1"),
        ("Left Thumb Intermediate", "LeftHandThumb2"),
        ("Left Thumb Distal", "LeftHandThumb3"),
        ("Left Index Proximal", "LeftHandIndex1"),
        ("Left Index Intermediate", "LeftHandIndex2"),
        ("Left Index Distal", "LeftHandIndex3"),
        ("Left Middle Proximal", "LeftHandMiddle1"),
        ("Left Middle Intermediate", "LeftHandMiddle2"),
        ("Left Middle Distal", "LeftHandMiddle3"),
        ("Left Ring Proximal", "LeftHandRing1"),
        ("Left Ring Intermediate", "LeftHandRing2"),
        ("Left Ring Distal", "LeftHandRing3"),
        ("Left Little Proximal", "LeftHandPinky1"),
        ("Left Little Intermediate", "LeftHandPinky2"),
        ("Left Little Distal", "LeftHandPinky3"),
        ("Right Thumb Proximal", "RightHandThumb1"),
        ("Right Thumb Intermediate", "RightHandThumb2"),
        ("Right Thumb Distal", "RightHandThumb3"),
        ("Right Index Proximal", "RightLeftHandIndex1"),
        ("Right Index Intermediate", "RightHandIndex2"),
        ("Right Index Distal", "RightHandIndex3"),
        ("Right Middle Proximal", "RightHandMiddle1"),
        ("Right Middle Intermediate", "RightHandMiddle2"),
        ("Right Middle Distal", "RightHandMiddle3"),
        ("Right Ring Proximal", "RightHandRing1"),
        ("Right Ring Intermediate", "RightHandRing2"),
        ("Right Ring Distal", "RightHandRing3"),
        ("Right Little Proximal", "RightHandPinky1"),
        ("Right Little Intermediate", "RightHandPinky2"),
        ("Right Little Distal", "RightHandPinky3"),
    };

    [MenuItem("Tools/LootCodeRPG/Configure Far East Set Rig")]
    public static void Execute()
    {
        var importer = AssetImporter.GetAtPath(ModelPath) as ModelImporter;
        if (importer == null)
        {
            Debug.LogError("ConfigureFarEastSetRig: ModelImporter not found at " + ModelPath);
            return;
        }

        importer.animationType = ModelImporterAnimationType.Human;
        importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
        importer.autoGenerateAvatarMappingIfUnspecified = false;

        HumanDescription description = importer.humanDescription;
        var bones = new List<HumanBone>(BoneMap.Length);
        for (int i = 0; i < BoneMap.Length; i++)
        {
            bones.Add(new HumanBone
            {
                boneName = BoneMap[i].boneName,
                humanName = BoneMap[i].humanName,
                limit = new HumanLimit { useDefaultValues = true }
            });
        }

        description.human = bones.ToArray();
        importer.humanDescription = description;
        importer.SaveAndReimport();

        Avatar humanAvatar = null;
        Object[] assets = AssetDatabase.LoadAllAssetsAtPath(ModelPath);
        for (int i = 0; i < assets.Length; i++)
        {
            if (assets[i] is Avatar avatar)
            {
                humanAvatar = avatar;
                break;
            }
        }

        if (humanAvatar == null)
        {
            Debug.LogError("ConfigureFarEastSetRig: Avatar sub-asset not found after reimport.");
            return;
        }

        Debug.Log("ConfigureFarEastSetRig: mapped " + bones.Count + " bones, avatar valid = " + humanAvatar.isValid);
    }

    public static void RunFromBatch()
    {
        Execute();
        EditorApplication.Exit(0);
    }
}
#endif
