using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System;
using UnityEngine.Networking;
using System.Collections;

public static class FileUtils
{
    public const string BUNDLE_LIST_FILE_NAME = "AssetbundleList.json";
    public static IEnumerator RequestTextFile<T>(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (!request.isNetworkError)
            {
                string bundleData = request.downloadHandler.text;

                T loadClass = JsonConvert.DeserializeObject<T>(bundleData);

                yield return loadClass;
            }
        }
    }
    public static T LoadFile<T>(string filePath)
    {
        T loadClass;
        try
        {
            string fildData = File.ReadAllText(filePath);

            loadClass = JsonConvert.DeserializeObject<T>(fildData);
        }
        catch(Exception e)
        {
            loadClass = default(T);
        }

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
        if (!savePath.EndsWith("/"))
        {
            savePath = $"{savePath}/";
        }
        if (Directory.Exists(savePath) == false)
        {
            Directory.CreateDirectory(savePath);
        }
        try
        {
            File.WriteAllText($"{savePath}{fileName}", saveStringData);
        }catch(Exception e)
        {

        }
    }
}
