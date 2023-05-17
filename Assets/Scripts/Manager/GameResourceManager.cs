using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                LoadAssetDatas();
            }
        }
        [MenuItem("Tools/MakeBundleJson", false, 1000)]
        public static void MakeBundleJson()
        {
            string[] bundleLists = AssetDatabase.GetAllAssetBundleNames();
            BundleListsDic listsDic = new BundleListsDic();
            for (int i=0;i< bundleLists.Length;i++)
            {
                listsDic.AddBundleName(bundleLists[i]);
                string[] bundleobjectlists = AssetDatabase.GetAssetPathsFromAssetBundle(bundleLists[i]);
                for(int j=0;j< bundleobjectlists.Length; j++)
                {
                    string[] bundlepaths = bundleobjectlists[j].Split('/');
                    listsDic.AddObjectRejeon(bundleLists[i], bundlepaths[bundlepaths.Length - 1]);

                }
            }
            string bundleSavePath = $"{Application.dataPath}{FileUtils.GetPlatformString()}";
            FileUtils.SaveFile<BundleListsDic>(bundleSavePath, FileUtils.BUNDLE_LIST_FILE_NAME, listsDic);
        }

#endif
        private Dictionary<string, object> prefabLists = new Dictionary<string, object>();
        private Dictionary<string, ObjectPool> prefabObjectpools = new Dictionary<string, ObjectPool>();

        [MenuItem("Tools/TestReadBundle", false, 1001)]
        public static void LoadAssetDatas()
        {
            //번들매니저 초기화 Android 파일 세팅
            AssetBundleManager.Initialize();

            string bundleSavePath = $"{GetStreamingAssetsPath()}/{FileUtils.GetPlatformString()}{ FileUtils.BUNDLE_LIST_FILE_NAME}";

            BundleListsDic loadDic = FileUtils.LoadFile<BundleListsDic>(bundleSavePath);

            //json파일 읽고, 모든 번들 리스트를 가지고 저장해야한다.
            //
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
        //public T GetResourceObject<T>(string resouceName)
        //{
        //    AssetBundleManager.LoadAssetAsync<
        //    return T;
        //}
        public GameObject GetPoolObject(string type)
        {
            if (!prefabObjectpools.ContainsKey(type))
            {
                prefabObjectpools.Add(type, new ObjectPool());
            }

            //AssetBundleManager.GetLoadedAssetBundle()
            return null;
        }
        public void PushObjectPool(string type, GameObject targetObject)
        {
            if (!prefabObjectpools.ContainsKey(type))
            {
                prefabObjectpools.Add(type, new ObjectPool());
            }

            prefabObjectpools[type].PushPool(targetObject);
        }
        public static void RemovePoolObject(GameObject targetObject)
        {
            Destroy(targetObject);
        }
    }

    public class ObjectPool
    {
        private List<GameObject> poolObjects = new List<GameObject>();
        public void PushPool(GameObject targetObject)
        {
            poolObjects.Add(targetObject);
        }
        public GameObject GetPoolObject(GameObject makeObject)
        {
            for(int i = 0; i < poolObjects.Count; i++)
            {
                if (!poolObjects[i].activeInHierarchy)
                {
                    poolObjects[i].SetActive(true);

                    return poolObjects[i];
                }
            }

            return GameObject.Instantiate(makeObject);
        }
        public void ReleasePool()
        {

        }
        public void DestoryPool()
        {
            for(int i=0;i< poolObjects.Count; i++)
            {
                GameResourceManager.RemovePoolObject(poolObjects[i]);
            }

            poolObjects.Clear();
        }
    }
}
