using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Seunghak.UIManager;
using UnityEngine.Networking;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace Seunghak.Common
{
    public struct BundleFileInfo
    {
        public string FileName;
        public long FileSize;
    }
    public class BundleListsDic
    {
        [SerializeField]
        public List<string> bundleNameLists = new List<string>();
        [SerializeField]
        public Dictionary<string, List<BundleFileInfo>> bundleObjectLists = new Dictionary<string, List<BundleFileInfo>>();

        public List<BundleFileInfo> GetBundleObjectLists(string bundleName)
        {
            if (bundleObjectLists.ContainsKey(bundleName))
            {
                return bundleObjectLists[bundleName];
            }

            return new List<BundleFileInfo>();
        } 
        public void AddBundleName(string bundleName)
        {
            if (!bundleNameLists.Contains(bundleName))
            {
                bundleNameLists.Add(bundleName);
            }
        }
        public void AddObjectRejeon(string bundleName, BundleFileInfo objectInfo)
        {
            if (!bundleObjectLists.ContainsKey(bundleName))
            {
                bundleObjectLists.Add(bundleName, new List<BundleFileInfo>());
            }

            bundleObjectLists[bundleName].Add(objectInfo);
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
                    BundleFileInfo newFileInfo;
                    long bundlesize = GetAssetBundleSize(bundleobjectlists[j]);
                    string[] bundlepaths = bundleobjectlists[j].Split('/');

                    newFileInfo.FileSize = bundlesize;
                    newFileInfo.FileName = bundlepaths[bundlepaths.Length - 1];
                    listsDic.AddObjectRejeon(bundleLists[i], newFileInfo);
                }
                listsDic.AddBundleName(bundleLists[i]);
            }
            string bundleSavePath = $"{Application.dataPath}{FileUtils.GetPlatformString()}";
            FileUtils.SaveFile<BundleListsDic>(bundleSavePath, FileUtils.BUNDLE_LIST_FILE_NAME, listsDic);
        }
        private static long GetAssetBundleSize(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            return fileInfo.Length;
        }
#endif
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                StartCoroutine( DownLoadAssetDatas());
            }
        }
        public IEnumerator DownLoadAssetDatas()
        {
            AssetBundleManager.BaseDownloadingURL = "https://drive.google.com/drive/folders/1jgrxT4h0dn75GFuVMkW4vokGmpga6T4m?usp=sharing";
            //DOWNLOAD_WEB_URL;

#if UNITY_EDITOR
            if (!AssetBundleManager.SimulateAssetBundleInEditor)
            {
                yield return AssetBundleManager.Initialize().IsDone();
            }
#endif

            AssetBundleManager.Instance.InitAssetBundleManager();

            yield return AssetBundleManager.Instance.GetReadyStatus(); 

            LoadAssetDatas();       
        }
        public void LoadAssetDatas()
        {
            string bundleLoadPath = $"{GetStreamingAssetsPath()}/{FileUtils.GetPlatformString()}{ FileUtils.BUNDLE_LIST_FILE_NAME}";

            BundleListsDic loadDic = FileUtils.LoadFile<BundleListsDic>(bundleLoadPath);
#if UNITY_EDITOR
            if (!AssetBundleManager.SimulateAssetBundleInEditor)
#endif
            {
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
                        string insertObject = loadDic.bundleObjectLists[loadDic.bundleNameLists[i]][j].FileName;
                        string[] namelists = insertObject.Split('.');
                        prefabLists.Add(namelists[0], loadedAssets.assetBundle.LoadAsset(insertObject));
                    }
                }
            }
#if UNITY_EDITOR
            else
            {
                string basicPath = Application.dataPath;
                //for (int i = 0; i < loadDic.bundleNameLists.Count; i++)
                //{
                //    basicPath.
                //}
            }
#endif
            Debug.Log("Ready To Load");
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
        public void PushObjectPool(string type, Object targetObject)
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
#if UNITY_EDITOR
            if (!AssetBundleManager.SimulateAssetBundleInEditor)
#endif
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
                return prefabObjectpools[objectName].GetPoolObject();
            }
#if UNITY_EDITOR
            else
            {
                //AssetBundle.LoadFromFile()
                return null;
            }
#endif
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
        private Object poolObject;
        public void PushPool(Object targetObject)
        { 
            poolObject = targetObject;
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

            return GameObject.Instantiate(poolObject) as GameObject;
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
