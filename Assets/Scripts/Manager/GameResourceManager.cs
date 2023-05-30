using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Seunghak.UIManager;
using UnityEngine.Networking;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Seunghak.Common
{
    public class BundleListsDic
    {
        [SerializeField]
        public List<string> bundleNameLists = new List<string>();
        [SerializeField]
        public Dictionary<string, List<string>> bundleObjectLists = new Dictionary<string, List<string>>();

        public List<string> GetBundleObjectLists(string bundleName)
        {
            if (bundleObjectLists.ContainsKey(bundleName))
            {
                return bundleObjectLists[bundleName];
            }

            return new List<string>();
        } 
        public void AddBundleName(string bundleName)
        {
            if (!bundleNameLists.Contains(bundleName))
            {
                bundleNameLists.Add(bundleName);
            }
        }
        public void AddObjectRejeon(string bundleName,string objectName)
        {
            if (!bundleObjectLists.ContainsKey(bundleName))
            {
                bundleObjectLists.Add(bundleName, new List<string>());
            }

            bundleObjectLists[bundleName].Add(objectName);
        }
    }
    public class GameResourceManager : UnitySingleton<GameResourceManager>
    {
        private Dictionary<string, UnityEngine.Object> prefabLists = new Dictionary<string, UnityEngine.Object>();
        private Dictionary<string, ObjectPool> prefabObjectpools = new Dictionary<string, ObjectPool>();
        private static string DOWNLOAD_WEB_URL = "C:/Users/dhtmd/ARDefence/Assets/Android";
#if UNITY_EDITOR

        [MenuItem("Tools/MakeBundleJson", false, 1000)]
        public static void MakeBundleJson()
        {
            string[] bundleLists = AssetDatabase.GetAllAssetBundleNames();
            BundleListsDic listsDic = new BundleListsDic();
            for (int i=0;i< bundleLists.Length;i++)
            {
                string[] bundleobjectlists = AssetDatabase.GetAssetPathsFromAssetBundle(bundleLists[i]);
                if (bundleobjectlists.Length <= 0)
                {
                    continue;
                }
                for(int j=0;j< bundleobjectlists.Length; j++)
                {
                    string[] bundlepaths = bundleobjectlists[j].Split('/');
                    listsDic.AddObjectRejeon(bundleLists[i], bundlepaths[bundlepaths.Length - 1]);

                }
                listsDic.AddBundleName(bundleLists[i]);
            }
            string bundleSavePath = $"{Application.dataPath}{FileUtils.GetPlatformString()}";
            FileUtils.SaveFile<BundleListsDic>(bundleSavePath, FileUtils.BUNDLE_LIST_FILE_NAME, listsDic);
        }

#endif
        public void DownLoadAssetDatas()
        {
            AssetBundleManager.BaseDownloadingURL = DOWNLOAD_WEB_URL;

            AssetBundleManager.Initialize();

            AssetBundleManager.Instance.InitAssetBundleManager();

            LoadAssetDatas();       
        }
        public void LoadAssetDatas()
        {
            string bundleLoadPath = $"{GetStreamingAssetsPath()}/{FileUtils.GetPlatformString()}{ FileUtils.BUNDLE_LIST_FILE_NAME}";

            BundleListsDic loadDic = FileUtils.LoadFile<BundleListsDic>(bundleLoadPath);

            for (int i = 0; i < loadDic.bundleNameLists.Count; i++)
            {
                string errorCode;

                LoadedAssetBundle loadedAssets = AssetBundleManager.GetLoadedAssetBundle(loadDic.bundleNameLists[i], out errorCode);

                if (loadedAssets == null)
                {
                    return;
                }

                for (int j = 0; j < loadDic.bundleObjectLists[loadDic.bundleNameLists[i]].Count; j++)
                {
                    string insertObject = loadDic.bundleObjectLists[loadDic.bundleNameLists[i]][j];

                    prefabLists.Add(insertObject, loadedAssets.assetBundle.LoadAsset(insertObject));
                }
            }
        }
        private static string GetStreamingAssetsPath()
        {
            if (Application.isEditor)
            {
                return Application.dataPath; 
            }
            else if (Application.isMobilePlatform || Application.isConsolePlatform)
            {
                return Application.streamingAssetsPath;
            }
            return "file://" + Application.streamingAssetsPath;
        }
        public GameObject GetPoolObject(string type)
        {
            if (!prefabObjectpools.ContainsKey(type))
            {
                prefabObjectpools.Add(type, new ObjectPool());
            }
            return null;
        }
        public void PushObjectPool(string type, GameObject targetObject)
        {
            if (targetObject == null)
            {
                return;
            }
            if (!prefabObjectpools.ContainsKey(type))
            {
                prefabObjectpools.Add(type, new ObjectPool());
            }

            prefabObjectpools[type].PushPool(targetObject);
        }
        private void SetFromAssetBundle(string objectName)
        {

        }
        public void RemovePoolObject(GameObject targetObject)
        {
            Destroy(targetObject);
        }
        public GameObject SpawnObject(string objectName)
        {
            if (!prefabObjectpools.ContainsKey(objectName))
            {
                if (prefabLists.ContainsKey(objectName))
                {
                    PushObjectPool(objectName, prefabLists[objectName] as GameObject);
                }
                else
                {
                    //프리팹 리스트에 없는 상황 이 경우엔 에러 처리
                    Debug.LogError($"PrefabLists have not {objectName}");
                    return null;
                }
            }
            return null;
        }
        public GameObject OpenUI(UI_TYPE openUIType)
        {
            if (prefabObjectpools.ContainsKey(openUIType.ToString()))
            {
                return prefabObjectpools[openUIType.ToString()].GetPoolObject();
            }

            return SpawnObject(openUIType.ToString());
        }
    }

    public class ObjectPool
    {
        private List<GameObject> poolObjects = new List<GameObject>();
        private GameObject poolObject;
        public void PushPool(GameObject targetObject)
        { 
            poolObject = targetObject;
            poolObjects.Add(targetObject);
        }
        public GameObject GetPoolObject()
        {
            for(int i = 0; i < poolObjects.Count; i++)
            {
                if (!poolObjects[i].activeInHierarchy)
                {
                    poolObjects[i].SetActive(true);

                    return poolObjects[i];
                }
            }

            return GameObject.Instantiate(poolObject);
        }
        public void ReleasePool()
        {

        }
        public void DestoryPool()
        {
            for(int i=0;i< poolObjects.Count; i++)
            {
                GameResourceManager.Instance.RemovePoolObject(poolObjects[i]);
            }

            poolObjects.Clear();
        }
    }
}
