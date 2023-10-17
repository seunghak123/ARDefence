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

        public static List<T> LoadJsonDatas<T>(E_JSON_TYPE loadType) where T : JBaseData
        {
            String loadPath = "";
            String loadTypeString = loadType.ToString();

            if (loadTypeString.Contains("Data"))
            {
                loadTypeString = loadTypeString.Replace("Data", "");
            }
            if (dicJsonData.ContainsKey(loadType.ToString()))
            {
                return JsonConvert.DeserializeObject<List<T>> (dicJsonData[loadType.ToString()]) ;
            }

            List<T> loadedObject = new List<T>();
#if UNITY_EDITOR
            loadTypeString = loadTypeString + ".json";
            loadPath = FileUtils.JSONFILE_LOAD_PATH + loadTypeString.ToLower();
            object loadData = FileUtils.LoadFile<object>(loadPath);

            loadedObject = JsonConvert.DeserializeObject<List<T>>(loadData.ToString());
#else
            UnityEngine.Object loadObject = GameResourceManager.Instance.LoadObject(loadTypeString.ToLower());
            loadedObject = JsonConvert.DeserializeObject<List<T>>(loadObject.ToString());
#endif
            if (!dicJsonData.ContainsKey(loadType.ToString()))
            {
                dicJsonData[loadType.ToString()] = loadData.ToString();
            }
            return loadedObject;
        }

        public List<JUnitData> GetUnitDatas()
        {
            List<JUnitData> unitDatas = LoadJsonDatas<JUnitData>(E_JSON_TYPE.JUnitData);

            return unitDatas;
        }
        public JUnitData[] GetUnitDatasArray(params int[] unitIdArray)
        {
            List<JUnitData> unitDatas = LoadJsonDatas<JUnitData>(E_JSON_TYPE.JUnitData);

            JUnitData[] unitArray = unitDatas.FindAll(find => { 
                for(int i = 0; i < unitIdArray.Length; i++)
                {
                    if (find.index == unitIdArray[i])
                    {
                        return true;
                    }
                }
                return false; 
            }).ToArray();

            return unitArray;
        }
        public JUnitData GetUnitData(int unitId)
        {
            List<JUnitData> unitDatas = LoadJsonDatas<JUnitData>(E_JSON_TYPE.JUnitData);

            JUnitData unitData = unitDatas.Find(find => find.index == unitId);

            return unitData;
        }

        public JStageData GetStageData(int stageId)
        {
            List<JStageData> stageDatas = LoadJsonDatas<JStageData>(E_JSON_TYPE.JStageData);

            JStageData stageData = stageDatas.Find(find => find.index == stageId);

            return stageData;
        }
    }
}