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
    #region LowCPUBundleInfo
    public struct LowCPUBundleFileInfo
    {
        public string bundleName;
        public string fileName;
        public long fileSize;
        public int hashCode;
        public string filePath;
    }
    public struct LowCPUBundleListInfo
    {
        public string bundleName;
        public long totalBundleSize;
        public int bundleTotalHashCode;
    }
    public class LowCPUBundleListDic
    {
        [SerializeField]
        public List<LowCPUBundleListInfo> bundleNameLists = new List<LowCPUBundleListInfo>();

        //object Name
        [SerializeField]
        public Dictionary<string, LowCPUBundleFileInfo> bundleObjectLists = new Dictionary<string,LowCPUBundleFileInfo>();

        public void AddObjectName(LowCPUBundleFileInfo bundleInfo)
        {
            bundleObjectLists[bundleInfo.fileName] = bundleInfo;
        }
        public void AddObjectsRejeon(string bundleName, List<LowCPUBundleFileInfo> objectInfoLists)
        {
            foreach (var objectInfo in objectInfoLists)
            {
                bundleObjectLists[objectInfo.fileName] = objectInfo;
            }
        }
        public long GetTotalSize()
        {
            long returnValue = 0;
            foreach (var listInfo in bundleNameLists)
            {
                returnValue += listInfo.totalBundleSize;
            }
            return returnValue;
        }
    }
    #endregion LowCPUBundleInfo
    public class BundleListsDic
    {
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
        private BundleListsDic currentBuildListsDic = new BundleListsDic();
        public bool isReady = false;
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
                    if(newFileInfo.fileName.Contains('.'))
                    {
                        string[] fileNameSplit = newFileInfo.fileName.Split('.');
                        if (fileNameSplit.Length > 0)
                        {
                            newFileInfo.fileName = fileNameSplit[0];
                        }
                    }
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
                if (atlasSprits == null)
                {
                    continue;
                }
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
            if (Input.GetKeyDown(KeyCode.B))
            {
                UserDataManager.SavePlayerPref<int>(PlayerPrefKey.SaveTest, 5);
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                int a =  UserDataManager.GetPlayerPref<int>(PlayerPrefKey.SaveTest);
            }
        }
        public IEnumerator SetDownloadDatas()
        {
            isReady = false;

            yield return AssetBundleManager.Instance.GetReadyStatus();

            string bundleLoadPath = $"{Application.persistentDataPath}/{ FileUtils.BUNDLE_LIST_FILE_NAME}";
#if UNITY_EDITOR
            if (AssetBundleManager.SimulateAssetBundleInEditor)
            {
                bundleLoadPath = $"{FileUtils.GetStreamingAssetsPath()}/{FileUtils.GetPlatformString()}{ FileUtils.BUNDLE_LIST_FILE_NAME}";
            }
#endif
            currentBuildListsDic =  FileUtils.LoadFile<BundleListsDic>(bundleLoadPath);

            LoadAssetDatas();

            yield break;
        }
        /// <summary>
        /// 에셋이 적은 환경에 해당
        /// </summary>
        ///LoadAssetDatas로 미리 에셋 데이터들을 저장하는 방식은 에셋 데이터가 적어서 미리 에셋 데이터를
        ///메모리에 올려놓아도 크게 문제가 안되는 경우에 해당합니다. 메모리가 4GB이하등 저 사양의 디바이스에서 구동할 경우
        ///해당 오브젝트를 찾고, 에셋을 로드하고, 오브젝트를 풀에 넣고 다시 로드에셋을 해제하는 방식이 필요합니다.
        ///이는 CPU 부담이 커질 수 있습니다.
        public void LoadAssetDatas()
        {
            string bundleLoadPath = $"{Application.persistentDataPath}/{ FileUtils.BUNDLE_LIST_FILE_NAME}";
#if UNITY_EDITOR
            if (!AssetBundleManager.SimulateAssetBundleInEditor)
#endif
            {
                for (int i = 0; i < currentBuildListsDic.bundleNameLists.Count; i++)
                {
                    string errorCode;
                    LoadedAssetBundle loadedAssets = AssetBundleManager.GetLoadedAssetBundle(currentBuildListsDic.bundleNameLists[i].bundleName, out errorCode);

                    if (loadedAssets == null)
                    {
                        return;
                    }

                    for (int j = 0; j < currentBuildListsDic.bundleObjectLists[currentBuildListsDic.bundleNameLists[i].bundleName].Count; j++)
                    {
                        string insertObject = currentBuildListsDic.bundleObjectLists[currentBuildListsDic.bundleNameLists[i].bundleName][j].fileName;
                        string[] namelists = insertObject.Split('.');
                        if (prefabLists.ContainsKey(namelists[0]))
                        {
                            Resources.UnloadAsset(prefabLists[namelists[0]]);
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
                for (int i = 0; i < currentBuildListsDic.bundleNameLists.Count; i++)
                {
 
                    for (int j = 0; j < currentBuildListsDic.bundleObjectLists[currentBuildListsDic.bundleNameLists[i].bundleName].Count; j++)
                    {
                        string loadPath = currentBuildListsDic.bundleObjectLists[currentBuildListsDic.bundleNameLists[i].bundleName][j].filePath;
                        string insertObject = currentBuildListsDic.bundleObjectLists[currentBuildListsDic.bundleNameLists[i].bundleName][j].fileName;
                        string[] namelists = insertObject.Split('.');
                        if (namelists.Length > 0)
                        {
                            Object loadObject = AssetDatabase.LoadAssetAtPath(loadPath,typeof(Object));
                            prefabLists[namelists[0]] = loadObject;
                        }
                        
                    }
                }
            }
#endif
            isReady = true;
            Debug.Log("Ready To Load");
        }
        /// <summary>
        /// 에셋이 많은 환경에 해당
        /// </summary>
        /// 에셋이 많아서, 한번에 올려놓는 메모리가 일정 크기 이상인 경우
        /// 미리 에셋을 복사할 Dictionary에 올려두는 것은 비효율적일 수 있습니다. 
        /// BundleListsDic 구조가 아닌 오브젝트가 에셋 번들 네임을 들고있는 Dictionary구조가 적합합니다. 
        /// 차후 이러한 구조로 설계할 경우 필요 asset들을 리스트로 받고 한번에 LoadAssetObjects하는 코드가 필요
        public void LoadAssetObject(string assetName)
        {
            //이 코드를 파일에서 읽어오는 코드로 변경
            LowCPUBundleListDic lowCpuBundleList = new LowCPUBundleListDic();
            string errorCode; 
            LoadedAssetBundle loadedAssets = AssetBundleManager.GetLoadedAssetBundle(lowCpuBundleList.bundleObjectLists[assetName].bundleName, out errorCode);

            if (loadedAssets == null)
            {
                return;
            }
            prefabLists[assetName] = loadedAssets.assetBundle.LoadAsset(assetName);
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
                        UpdateAssetBundleObjectMatrial(useritem as GameObject);
                        return useritem as GameObject;
                    }

                    return null;
                }
            }
            GameObject poolObject = prefabObjectpools[objectName].GetPoolObject();

            UpdateAssetBundleObjectMatrial(poolObject);

            return poolObject;
        }
        public void UpdateAssetBundleObjectMatrial(GameObject inGameObject)
        {
#if UNITY_EDITOR
            {
                if (inGameObject == null) return;
            }
            {
                {
                    var lComList = new List<SpriteRenderer>(inGameObject.GetComponentsInChildren<SpriteRenderer>(true));
                    if (lComList != null && lComList.Count > 0)
                    {				
                        lComList.ForEach((fCom) =>
                        {
                            if (fCom != null && fCom.sharedMaterial != null && fCom.sharedMaterial.shader != null)
                            {
                                fCom.sharedMaterial.shader = Shader.Find(fCom.sharedMaterial.shader.name);
                            }
                        });
                    }
                }
                {
                    var lComList = new List<SkinnedMeshRenderer>(inGameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true));
                    if (lComList != null && lComList.Count > 0)
                    {
                        lComList.ForEach((fCom) =>
                        {
                            if (fCom != null && fCom.sharedMaterial != null && fCom.sharedMaterial.shader != null)
                            {
                                fCom.sharedMaterial.shader = Shader.Find(fCom.sharedMaterial.shader.name);
                            }
                        });
                    }
                }
                {
                    var lComList = new List<MeshRenderer>(inGameObject.GetComponentsInChildren<MeshRenderer>(true));
                    if (lComList != null && lComList.Count > 0)
                    {
                        lComList.ForEach((fCom) =>
                        {
                            if (fCom != null && fCom.sharedMaterial != null && fCom.sharedMaterial.shader != null)
                            {
                                fCom.sharedMaterial.shader = Shader.Find(fCom.sharedMaterial.shader.name);
                            }
                        });
                    }
                }
                {
                    var lComList = new List<ParticleSystemRenderer>(inGameObject.GetComponentsInChildren<ParticleSystemRenderer>());
                    if (lComList != null && lComList.Count > 0)
                    {
                        lComList.ForEach((fCom) =>
                        {
                            if (fCom != null && fCom.sharedMaterial != null && fCom.sharedMaterial.shader != null)
                            {
                                fCom.sharedMaterial.shader = Shader.Find(fCom.sharedMaterial.shader.name);
                            }
                        });
                    }
                }
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
                //해당 내용은 최적화가 쥐뿔도 안되어있기때문에, 차후 수정 요망
                 string bundleFilePath = $"{Application.dataPath}{FileUtils.GetPlatformString()}{ FileUtils.BUNDLE_LIST_FILE_NAME}";
                 
                BundleListsDic bundleLists = FileUtils.LoadFile<BundleListsDic>(bundleFilePath);
                for(int i=0;i< bundleLists.bundleNameLists.Count; i++)
                {
                    BundleFileInfo info = bundleLists.bundleObjectLists[bundleLists.bundleNameLists[i].bundleName].Find(find => find.fileName == objectName);

                    if (!string.IsNullOrEmpty(info.fileName))
                    {
                        //찾앗다면
                        return AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(info.filePath);
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
        private System.DateTime currentAccessTime = new System.DateTime();
        private void RemoveLifeTimeSpanObjectPool()
        {
            //오브젝트 풀에 일정 시간 엑세스 하지 않았다면, 씬 넘어갈때 해당 오브젝트는 날아가도록 하게함.
            if (System.DateTime.Now > currentAccessTime.AddSeconds(CommonUtil.PoolRemoveSecTime))
            {
                DestoryPool();
            }
        }
        public void PushPool(Object targetObject)
        {
            currentAccessTime = System.DateTime.Now;
            poolObject = targetObject;
        }
        public GameObject GetPoolObject()
        {
            //PoolObject를 가져가고 각 단계의 초기화는 각 매니저에서 진행한다.
            currentAccessTime = System.DateTime.Now;
            for (int i = 0; i < poolObjects.Count; i++)
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
