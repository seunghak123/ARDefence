using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seunghak.Common
{
    public class JsonDataManager : UnitySingleton<JsonDataManager>
    {
        private static Dictionary<string,string> dicJsonData = new Dictionary<string, string>();
  
        public static List<T> LoadJsonDatas<T>(E_JSON_TYPE loadType)
        {
            String loadPath = "";
            String loadTypeString = loadType.ToString()+".json";

            if (loadTypeString.Contains("Data"))
            {
                loadTypeString = loadTypeString.Replace("Data", "");
            }
            if (dicJsonData.ContainsKey(loadType.ToString()))
            {
                return JsonConvert.DeserializeObject<List<T>>(dicJsonData[loadType.ToString()]);
            }

            List<T> loadedObject = new List<T>();
#if UNITY_EDITOR
            loadPath = FileUtils.JSONFILE_LOAD_PATH + loadTypeString.ToLower();
            object loadData = FileUtils.LoadFile<object>(loadPath);

            loadedObject = JsonConvert.DeserializeObject<List<T>>(loadData.ToString());
#else
            UnityEngine.Object loadObject = GameResourceManager.Instance.LoadObject(loadTypeString.ToLower());
            loadedObject = JsonConvert.DeserializeObject<List<T>>(loadObject.ToString());
#endif
            if (!dicJsonData.ContainsKey(loadType.ToString()))
            {
                dicJsonData[loadType.ToString()] = loadedObject.ToString();
            }
            return loadedObject;
        }

    }
}