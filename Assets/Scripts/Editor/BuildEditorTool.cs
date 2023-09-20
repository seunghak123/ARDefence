using UnityEditor;
using UnityEngine;
using System;
using System.IO;
#if UNITY_IOS
using System.Collections.Generic;
using UnityEditor.iOS.Xcode;
#endif
using Seunghak.Common;
public class BuildEditorTool : MonoBehaviour
{
#if UNITY_ANDROID
    private static string buildPath = "AOS";
    static private bool isAABBundle = false;
#elif UNITY_IOS
    private static string buildPath = "IOS";
#endif
#if UNITY_ANDROID || UNITY_IOS
    [MenuItem("Build/AndroidAAB",true)]
    public static bool SetAndroidAAB()
    {
        isAABBundle = !isAABBundle;
        return isAABBundle;
    }
    [MenuItem("Build/BuildAssetBundles", false, 1002)]
    public static void BuildBundles()
    {
        BuildPipeline.BuildAssetBundles("Assets/Android", BuildAssetBundleOptions.None, BuildTarget.Android);
    }

    [MenuItem("Build/PlatformBuild", false, 1001)]
    public static void BuildPlatform()
    {
        BuildPlayerOptions buildOption = new BuildPlayerOptions();

        buildOption.locationPathName = $"./Build/{buildPath}/{System.DateTime.UtcNow.ToString("yy_MM_dd_HH_mm")}.aab";
        PlayerSettings.bundleVersion = "1.0.1";
        PlayerSettings.Android.bundleVersionCode = 1;
#if UNITY_ANDROID
        EditorUserBuildSettings.buildAppBundle = true;
        BuildForAndroid();
        buildOption.target = BuildTarget.Android;
        buildOption.locationPathName = buildOption.locationPathName;
#elif UNITY_IOS
        buildOption.target = BuildTarget.iOS;
#endif
        buildOption.scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes);
        BuildPipeline.BuildPlayer(buildOption);
        MakeVersionAndCDNFile();
        //if (Directory.Exists($"../../Build/{buildPath}"))
        //{
        //    string argument = $"../../Build/{buildPath}";
        //    try
        //    {
        //        System.Diagnostics.Process.Start("open", argument);
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.Log($"Fail open file {e.ToString()}");
        //    }
        //}
    }
#endif
    [MenuItem("Build/InfoFiles", false, 1002)]
    private static void MakeVersionAndCDNFile()
    {
        UpdateVersionInfo userUpdateVersion = new UpdateVersionInfo();
        userUpdateVersion.currentVersion = PlayerSettings.bundleVersion;
        userUpdateVersion.forcedUpdateVersion = userUpdateVersion.currentVersion;
        userUpdateVersion.cdnAddressInfoPath = $"{ApplicationManager.cdnAddressPath}{FileUtils.GetPlatformString()}";
        string dataSavePath = $"{FileUtils.DATA_SAVE_PATH}";
        FileUtils.SaveFile<UpdateVersionInfo>(dataSavePath, FileUtils.VERSION_INFO_FILE_NAME, userUpdateVersion);

        CDNUpdateAddressInfo cdnUpdateInfo = new CDNUpdateAddressInfo();
        cdnUpdateInfo.updateCDNPath = $"{userUpdateVersion.cdnAddressInfoPath}/{userUpdateVersion.currentVersion}/";
        FileUtils.SaveFile<CDNUpdateAddressInfo>(dataSavePath, FileUtils.CDN_ADDRESS_FILE_NAME, cdnUpdateInfo);
    }
    private static void BuildForAndroid()
    {
        PlayerSettings.Android.keystorePass = "asdf1234";
        PlayerSettings.Android.keyaliasPass = "asdf1234";
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        //build aab
        PlayerSettings.Android.buildApkPerCpuArchitecture = true;
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
