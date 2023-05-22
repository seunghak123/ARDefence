using UnityEditor;
using UnityEngine;
using System;
using System.IO;
#if UNITY_IOS
using System.Collections.Generic;
using UnityEditor.iOS.Xcode;
#endif

public class BuildEditorTool : MonoBehaviour
{
#if UNITY_ANDROID
    private static string buildPath = "AOS";
#elif UNITY_IOS
    private static string buildPath = "IOS";
#endif

    [MenuItem("Build/BuildAssetBundles", false, 1002)]
    public static void BuildBundles()
    {
        BuildPipeline.BuildAssetBundles("Assets/Android", BuildAssetBundleOptions.None, BuildTarget.Android);
    }
    [MenuItem("Build/PlatformBuild", false, 1001)]
    public static void BuildPlatform()
    {
        BuildPlayerOptions buildOption = new BuildPlayerOptions();
        buildOption.locationPathName = $"../../Build/{buildPath}/{System.DateTime.UtcNow.ToString("yy_MM_DD_HH_mm")}";

#if UNITY_ANDROID
        BuildForAndroid();
        buildOption.target = BuildTarget.Android;
#elif UNITY_IOS
        buildOption.target = BuildTarget.iOS;
#endif
        buildOption.scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
        BuildPipeline.BuildPlayer(buildOption);

        if (Directory.Exists($"../../Build/{buildPath}"))
        {
            string argument = $"../../Build/{buildPath}";
            try
            {
                System.Diagnostics.Process.Start("open",argument);
            }catch(Exception e)
            {
                Debug.Log($"Fail open file {e.ToString()}");
            }
        }
    }
    private static void BuildForAndroid()
    {
        PlayerSettings.Android.keystorePass = "asdf1234";
        PlayerSettings.Android.keyaliasPass = "asdf1234";
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
    }
#if UNITY_IOS
    private static string PHOTO_USAGE_STRING = "해당 앱을 사용하시려면 사진첩 접근 권한이 필요합니다.";
    [UnityEditor.Callbacks.PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string buildPath)
    {
        string pbxProjectPath = PBXProject.GetPBXProjectPath(buildPath);
        string plistPath = Path.Combine(buildPath, "Info.plist");

        PBXProject pbxProject = new PBXProject();
        pbxProject.ReadFromFile(pbxProjectPath);

#if UNITY_2019_3_OR_NEWER
        string targetGUID = pbxProject.GetUnityFrameworkTargetGuid();
#else
        string targetGUID = pbxProject.TargetGuidByName(pbxProject.GetUnityMainTargetGuid());
#endif
        //프로퍼티 추가 방식
        //pbxProject.AddBuildProperty(targetGUID, "OTHER_LDFLAGS", "-weak_framework PhotosUI");
        PlistDocument plist = new PlistDocument();
        plist.ReadFromString(File.ReadAllText(plistPath));

        PlistElementDict rootDic = plist.root;

        rootDic.SetBoolean("ITSAppUsesNonExemptEncryption", false);
        rootDic.SetString("NSPhotoLibraryUsageDescription", PHOTO_USAGE_STRING);
        //사진 추가 권한
        rootDic.SetString("NSPhotoLibraryAddUsageDescription", PHOTO_USAGE_STRING);
        //마이크 사용 권한
        rootDic.SetString("NSMicrophonUsageDescription", PHOTO_USAGE_STRING);
        File.WriteAllText(plistPath, plist.WriteToString());
    }
#endif
}
