using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

public static class FileUtils
{
    public const string BUNDLE_LIST_FILE_NAME = "AssetbundleList.json";
    public static T LoadFile<T>(string filePath)
    {
        string fildData = File.ReadAllText(filePath);

        T loadClass = JsonConvert.DeserializeObject<T>(fildData);

        return loadClass;
    }
    public static string GetPlatformString()
    {
#if UNITY_ANDROID
        return "/Android/";
#elif UNITY_IOS
            return "/IOS/;
#endif
    }
    public static void SaveFile<T>(string savePath,string fileName,T saveData)
    {
        string saveStringData = JsonConvert.SerializeObject(saveData);

        if (Directory.Exists(savePath) == false)
        {
            Directory.CreateDirectory(savePath);
        }

        File.WriteAllText($"{savePath}{fileName}", saveStringData);
    }
}
