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
    #region Base64
    public static string Base64Encode(string data)
    {
        try
        {
            byte[] encData_byte = new byte[data.Length];
            encData_byte = System.Text.Encoding.UTF8.GetBytes(data);
            string encodedData = Convert.ToBase64String(encData_byte);
            return encodedData;
        }
        catch (Exception e)
        {
            throw new Exception("Error in Base64Encode: " + e.Message);
        }
    }
    public static string Base64Decode(string data)
    {
        try
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();

            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(data);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            return result;
        }
        catch (Exception e)
        {
            throw new Exception("Error in Base64Decode: " + e.Message);
        }
    }
    #endregion
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
