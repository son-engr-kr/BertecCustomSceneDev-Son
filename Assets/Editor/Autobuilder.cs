// Build helper for the custom scene.
using System.IO;
using UnityEditor;
using UnityEngine;

#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable IDE0051 // Remove unused private members


class Autobuilder
{
    static private string[] scenes = {
    "Assets/Scenes/VisualFlow/CustomVisualFlow.unity",
    };

    static private string outputFolder()
    {
        return Path.Combine(Application.dataPath, "..", "BuildOutput");
    }

    static void SwitchToAndroid()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
    }

    [MenuItem("Build/Build APK")]
    static void InstallBuild() // called from the Unity Build Script during the install build process
    {
        Bertec.BuildServices.BuildProject(scenes, outputFolder());
    }
}

