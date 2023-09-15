using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Seunghak.UIManager;
using UnityEngine.Networking;
using System.IO;
using UnityEngine.U2D;
using TMPro;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Seunghak.Common
{
    public struct BundleListInfo
    {
        public string bundleName;
        public long totalBundleSize;
        public int bundleTotalHashCode;
    }
    public struct BundleFileInfo
    {
        public string fileName;
        public long fileSize;
        public int hashCode;
        public string filePath;
    }
    public struct AtlasLists
    {
        public List<AtlasInfo> atlaseLists;
    }
    public struct AtlasInfo
    {
        public string atlasName;
        public List<string> spriteLists;
    }
    public class BundleListsDic
    {
        public bool isEndDownload = false;
        [SerializeField]
        public List<BundleListInfo> bundleNameLists = new List<BundleListInfo>();
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
        public void AddBundleName(BundleListInfo bundleInfo)
        {
            if (!bundleNameLists.Contains(bundleInfo))
            {
                bundleNameLists.Add(bundleInfo);
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
        public void AddObjectsRejeon(string bundleName, List<BundleFileInfo> objectInfoLists)
        {
            if (!bundleObjectLists.ContainsKey(bundleName))
            {
                bundleObjectLists.Add(bundleName, new List<BundleFileInfo>());
            }

            foreach(var objectInfo in objectInfoLists)
            {
                BundleFileInfo finedObject = bundleObjectLists[bundleName].Find(find => find.fileName == objectInfo.fileName);
                if (!string.IsNullOrEmpty(finedObject.fileName))
                {
                    bundleObjectLists[bundleName].Add(finedObject);
                }
                else
                {
                    finedObject = objectInfo;
                }
            }
        }
        public long GetTotalSize()
        {
            long returnValue = 0;
            foreach(var listInfo in bundleNameLists)
            {
                returnValue += listInfo.totalBundleSize;
            }
            return returnValue;
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
                BundleListInfo bundleListInfo;

                string[] bundleobjectlists = AssetDatabase.GetAssetPathsFromAssetBundle(bundleLists[i]);
                if (bundleobjectlists.Length <= 0)
                {
                    continue;
                }
                long totalBundleSize = 0;
                int totalHashCode = 0;
                for(int j=0;j< bundleobjectlists.Length; j++)
                {
                    BundleFileInfo newFileInfo;
                    long bundleSize = GetAssetBundleSize(bundleobjectlists[j]);
                    string[] bundlepaths = bundleobjectlists[j].Split('/');

                    newFileInfo.fileSize = bundleSize;
                    newFileInfo.filePath = bundleobjectlists[j];
                    totalBundleSize += bundleSize;
                    newFileInfo.fileName = bundlepaths[bundlepaths.Length - 1];
                    AssetImporter importer = AssetImporter.GetAtPath(bundleobjectlists[j]);
                    newFileInfo.hashCode = 0;

                    if (importer != null)
                    {
                        newFileInfo.hashCode = importer.GetHashCode();
                    }
                    totalHashCode ^= newFileInfo.hashCode;
                    listsDic.AddObjectRejeon(bundleLists[i], newFileInfo);
                }
                bundleListInfo.bundleName = bundleLists[i];
                bundleListInfo.totalBundleSize = totalBundleSize;
                bundleListInfo.bundleTotalHashCode = totalHashCode;
                listsDic.AddBundleName(bundleListInfo);
            }
            string bundleSavePath = $"{Application.dataPath}{FileUtils.GetPlatformString()}";
            FileUtils.SaveFile<BundleListsDic>(bundleSavePath, FileUtils.BUNDLE_LIST_FILE_NAME, listsDic);

            AtlasLists atlasLists;
            atlasLists.atlaseLists = new List<AtlasInfo>();
            string jsonSavePath = $"{FileUtils.ATLAS_SAVE_PATH}";
            for (int i=0;i< listsDic.bundleObjectLists["atlas"].Count; i++)
            {
                AtlasInfo newAtlasInfo;
                newAtlasInfo.atlasName = listsDic.bundleObjectLists["atlas"][i].fileName;
                int lastDotIndex = newAtlasInfo.atlasName.LastIndexOf('.');

                if (lastDotIndex >= 0)
                {
                    newAtlasInfo.atlasName = newAtlasInfo.atlasName.Substring(0, lastDotIndex);
                }
                List<string> fileNames = new List<string>();
                SpriteAtlas atlasSprits = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(listsDic.bundleObjectLists["atlas"][i].filePath);
                Object[] lists = UnityEditor.U2D.SpriteAtlasExtensions.GetPackables(atlasSprits);

                foreach (var atlasObject in lists)
                {
                    switch (atlasObject)
                    {
                        case DefaultAsset asset:
                            string assetPath = AssetDatabase.GetAssetPath(atlasObject.GetInstanceID());
                            List<string> objectlists = GetPathObjectNameLists(assetPath);
                            fileNames.AddRange(objectlists);
                            break;
                        case Texture texture:
                        case Sprite sprite:
                            int objectNameDotPos = atlasObject.name.LastIndexOf('.');
                            string addObjectName = atlasObject.name;
                            if (objectNameDotPos >= 0)
                            {
                                addObjectName = addObjectName.Substring(0, objectNameDotPos);
                            }
                            fileNames.Add(addObjectName);
                            break;
                    }
                }
                newAtlasInfo.spriteLists = fileNames;

                atlasLists.atlaseLists.Add(newAtlasInfo);
            }
            FileUtils.SaveFile<AtlasLists>(jsonSavePath, FileUtils.ATLAS_LIST_FILE_NAME, atlasLists);
        }
        private static List<string> GetPathObjectNameLists(string assetPath)
        {
            List<string> findedAssets = new List<string>();
            string[] findedAssetArrays = AssetDatabase.FindAssets(null,new[] { assetPath }).Distinct().ToArray();
            for(int i = 0; i < findedAssetArrays.Length; i++)
            {
                string findedAssetPath = AssetDatabase.GUIDToAssetPath(findedAssetArrays[i]);
                if (!string.IsNullOrEmpty(findedAssetPath))
                {
                    string[] pathSplit = findedAssetPath.Split('/');
                    string addObjectName = pathSplit[pathSplit.Length - 1];
                    int objectNameDotPos = addObjectName.LastIndexOf('.');

                    if (objectNameDotPos >= 0)
                    {
                        addObjectName = addObjectName.Substring(0, objectNameDotPos);
                    }
                    findedAssets.Add(addObjectName);
                }
            }
            return findedAssets;
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
                StartCoroutine(SetPreDownloadDatas());
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                UserDataManager.SavePlayerPref<int>(PlayerPrefKey.SaveTest, 5);
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                int a =  UserDataManager.GetPlayerPref<int>(PlayerPrefKey.SaveTest);
            }
        }
        public IEnumerator SetPreDownloadDatas()
        {
            string preDownloadPath = Application.persistentDataPath;

            string preCheckDownloadFile = $"{Application.persistentDataPath}/{ FileUtils.BUNDLE_LIST_FILE_NAME}";

            BundleListsDic loadDic = FileUtils.LoadFile<BundleListsDic>(preCheckDownloadFile);

            if (loadDic == null)
            {
                string bundleLoadPath = $"{GetStreamingAssetsPath()}/{FileUtils.GetPlatformString()}{ FileUtils.BUNDLE_LIST_FILE_NAME}";

                IEnumerator coroutine = FileUtils.RequestTextFile<BundleListsDic>(bundleLoadPath);
                while (coroutine.MoveNext())
                {
                    object current = coroutine.Current;
                    if (current is BundleListsDic bundleData)
                    {
                        loadDic = bundleData;
                    }
                    yield return null;
                }

                //여기에 다운로드 받을건지 말건지 하시오~ 라는 작업필요함 여기다가는 미리 action이나 특정 행동을 저장한
                //delegate를 실행하게 해주자

                yield return StartCoroutine(DownLoadAssetDatas());

                loadDic.isEndDownload = true;

                FileUtils.SaveFile<BundleListsDic>(preDownloadPath, FileUtils.BUNDLE_LIST_FILE_NAME, loadDic);
            }
            else
            {
                string bundleLoadPath = $"{GetStreamingAssetsPath()}/{FileUtils.GetPlatformString()}{ FileUtils.BUNDLE_LIST_FILE_NAME}";
                //번들 경로를 이런것 처럼 넣어 줄것 변경된게 있으면 다운로드 받을 경로 없으면 기존 경로로 세팅
                BundleListsDic downloadDic = FileUtils.LoadFile<BundleListsDic>(bundleLoadPath);

                List<string> downloadNeedLists = new List<string>();


                //downloadDic과 loadDic 을 비교해서 다운로드 받아야할 번들 리스트 정리 
                //비교 대상 - hashcode, 번들 총 사이즈, 

                AssetBundleManager.overrideBaseDownloadingURL += (string aaaa)=>{
                    //번들리스트에 들어가 잇는경우엔 원래경로, 아니면 변경된 경로
                    return "AAA";
                };


                yield return StartCoroutine(DownLoadAssetDatas());

                downloadDic.isEndDownload = true;

                FileUtils.SaveFile<BundleListsDic>(preDownloadPath, FileUtils.BUNDLE_LIST_FILE_NAME, downloadDic);
            }

            //해당 json을 읽어서 미리 데이터 세팅 및 해시 세팅
            //(미리 다운로드 받은것들 에셋번들에 넣어놓을것
            //json을 다운로드 받고 용량 .. 말고 다른것도 추가해야할듯
            //데이터 세팅이 끝나면 해시 비교하여, 어떤 에셋 번들을 받아야하는지 리스트 나열
            //번들 다운로드 받아야할 총 용량을 확인 및, 다운로드 받을때마다 데이터 카운팅           
            //파일 내역을 받고, 기존 데이터 세팅

            yield return null;
        }
        public IEnumerator DownLoadAssetDatas()
        {
            AssetBundleManager.BaseDownloadingURL = DOWNLOAD_WEB_URL;
#if UNITY_EDITOR
            if (!AssetBundleManager.SimulateAssetBundleInEditor)
            {
                yield return AssetBundleManager.Initialize().IsDone();
            }
#endif

            AssetBundleManager.Instance.InitAssetBundleManager();

            //AssetBundleManager.BaseDownloadingURL = DOWNLOAD_WEB_URL;

            //AssetBundleManager.Instance.InitAssetBundleManager();

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

                    LoadedAssetBundle loadedAssets = AssetBundleManager.GetLoadedAssetBundle(loadDic.bundleNameLists[i].bundleName, out errorCode);

                    if (loadedAssets == null)
                    {
                        return;
                    }

                    for (int j = 0; j < loadDic.bundleObjectLists[loadDic.bundleNameLists[i].bundleName].Count; j++)
                    {
                        string insertObject = loadDic.bundleObjectLists[loadDic.bundleNameLists[i].bundleName][j].fileName;
                        string[] namelists = insertObject.Split('.');
                        if (prefabLists.ContainsKey(namelists[0]))
                        {
                            prefabLists[namelists[0]]=loadedAssets.assetBundle.LoadAsset(insertObject);
                        }
                        else
                        {
                            prefabLists.Add(namelists[0], loadedAssets.assetBundle.LoadAsset(insertObject));
                        }
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
                        Debug.LogWarning($"PrefabLists have not {objectName}");
                        Object useritem = Resources.Load(objectName);
                        
                        if (useritem != null)
                        {
                            return useritem as GameObject;
                        }

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
        public Object LoadObject(string objectName)
        {
#if UNITY_EDITOR
            if (!AssetBundleManager.SimulateAssetBundleInEditor)
#endif
            {
                if (!prefabObjectpools.ContainsKey(objectName))
                {
                    if (prefabLists.ContainsKey(objectName))
                    {
                        if(prefabLists[objectName] is GameObject)
                        {
                            PushObjectPool(objectName, prefabLists[objectName]);
                        }
                        return prefabLists[objectName];
                    }
                    else
                    {
                        Debug.LogError($"ObjectLists have not {objectName}");
                        return null;
                    }
                }
                return prefabObjectpools[objectName].GetPoolObject();
            }
#if UNITY_EDITOR
            else
            {
                 string bundleFilePath = $"{Application.dataPath}{FileUtils.GetPlatformString()}{ FileUtils.BUNDLE_LIST_FILE_NAME}";
                 
                BundleListsDic bundleLists = FileUtils.LoadFile<BundleListsDic>(bundleFilePath);
                for(int i=0;i< bundleLists.bundleNameLists.Count; i++)
                {
                    BundleFileInfo info = bundleLists.bundleObjectLists[bundleLists.bundleNameLists[i].bundleName].Find(find => find.fileName == objectName);

                    if (!string.IsNullOrEmpty(info.fileName))
                    {
                        //찾앗다면
                        return Resources.Load(info.filePath);
                    }
                }

                //번들 리스트 파일 읽고, 오브젝트 찾아서 직접 생산 이미지 파일 경우는 아틀라스 읽고 참조하는 코드 만들것
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
