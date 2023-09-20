/*  The AssetBundle Manager */

// [downloading method selection]
// AssetBundleManager(ABM) internally uses either WWW or UnityWebRequest to download AssetBundles.
// By default, ABM will automatically select either one based on the version of the Unity runtime.
//
// - WWW
//   For Unity5.3 and earlier PLUS Unity5.5.
// - UnityWebRequest
//   For Unity5.4 and later versions EXCEPT Unity5.5.
//   UnityWebRequest class is officialy introduced since Unity5.4, it is intended to replace WWW.
//   The primary advantage of UnityWebRequest is memory efficiency. It does not load entire
//   AssetBundle into the memory while WWW does.
//
// For Unity5.5 we let ABM to use WWW since we observed a download failure case.
// (https://bitbucket.org/Unity-Technologies/assetbundledemo/pull-requests/25)
//
// Or you can force ABM to use either method by setting one of the following symbols in
// [Player Settings]-[Other Settings]-[Scripting Define Symbols] of each platform.
//
// - ABM_USE_WWW    (to use WWW)
// - ABM_USE_UWREQ  (to use UnityWebRequest)

#if !ABM_USE_WWW && !ABM_USE_UWREQ
#if UNITY_5_4_OR_NEWER && !UNITY_5_5
#define ABM_USE_UWREQ
#else
#define ABM_USE_WWW
#endif
#endif

using UnityEngine;
#if ABM_USE_UWREQ
using UnityEngine.Networking;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using Seunghak.Common;

namespace Seunghak
{
    /// <summary>
    /// Loaded assetBundle contains the references count which can be used to
    /// unload dependent assetBundles automatically.
    /// </summary>
    public class LoadedAssetBundle
    {
        public AssetBundle assetBundle;
        public int referencedCount;

        internal event Action unload;

        internal void OnUnload()
        {
            assetBundle.Unload(false);
            if (unload != null)
                unload();
        }

        public LoadedAssetBundle(AssetBundle assetBundle)
        {
            this.assetBundle = assetBundle;
            referencedCount = 1;
        }
    }

    /// <summary>
    /// Class takes care of loading assetBundle and its dependencies
    /// automatically, loading variants automatically.
    /// </summary>
    public class AssetBundleManager : UnitySingleton<AssetBundleManager>
    {
        static string baseDownloadingURL = "";
        static string[] activeVariants =  {};
        static AssetBundleManifest assetBundleManifest = null;

#if UNITY_EDITOR
        static int simulateAssetBundleInEditor = -1;
        const string kSimulateAssetBundles = "SimulateAssetBundles";
#endif

        static Dictionary<string, LoadedAssetBundle> loadedAssetBundles = new Dictionary<string, LoadedAssetBundle>();
        static Dictionary<string, string> downloadingErrors = new Dictionary<string, string>();
        static Dictionary<string, int> downloadingBundles = new Dictionary<string, int>();

        public static List<AssetBundleLoadOperation> inProgressOperations = new List<AssetBundleLoadOperation>();
        static Dictionary<string, string[]> dependencies = new Dictionary<string, string[]>();
        /// <summary>
        /// The base downloading url which is used to generate the full
        /// downloading url with the assetBundle names.
        /// </summary>
        public static string BaseDownloadingURL
        {
            get { return baseDownloadingURL; }
            set { baseDownloadingURL = value; }
        }

        /// <summary>
        /// Variants which is used to define the active variants.
        /// </summary>
        public static string[] ActiveVariants
        {
            get { return activeVariants; }
            set { activeVariants = value; }
        }
        public delegate string OverrideBaseDownloadingURLDelegate(string bundleName);

        /// <summary>
        /// Implements per-bundle base downloading URL override.
        /// The subscribers must return null values for unknown bundle names;
        /// </summary>
        public static event OverrideBaseDownloadingURLDelegate overrideBaseDownloadingURL;
        /// <summary>
        /// AssetBundleManifest object which can be used to load the dependecies
        /// and check suitable assetBundle variants.
        /// </summary>
        public static AssetBundleManifest AssetBundleManifestObject
        {
            set {assetBundleManifest = value; }
            get { return assetBundleManifest; }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Flag to indicate if we want to simulate assetBundles in Editor without building them actually.
        /// </summary>
        public static bool SimulateAssetBundleInEditor
        {
            get
            {
                if (simulateAssetBundleInEditor == -1)
                    simulateAssetBundleInEditor = EditorPrefs.GetBool(kSimulateAssetBundles, true) ? 1 : 0;

                return simulateAssetBundleInEditor != 0;
            }
            set
            {
                int newValue = value ? 1 : 0;
                if (newValue != simulateAssetBundleInEditor)
                {
                    simulateAssetBundleInEditor = newValue;
                    EditorPrefs.SetBool(kSimulateAssetBundles, value);
                }
            }
        }
#endif
        public void InitAssetBundleManager()
        {
            string bundleLoadPath = $"{GetStreamingAssetsPath()}/{FileUtils.GetPlatformString()}{ FileUtils.BUNDLE_LIST_FILE_NAME}";

            BundleListsDic loadDic = FileUtils.LoadFile<BundleListsDic>(bundleLoadPath);

            for(int i=0;i< loadDic.bundleNameLists.Count; i++)
            {
                LoadAssetBundle(loadDic.bundleNameLists[i].bundleName);
            }
        }
        public void InitAssetBundleManager(BundleListsDic initBundleLists)
        {
            for (int i = 0; i < initBundleLists.bundleNameLists.Count; i++)
            {
                LoadAssetBundle(initBundleLists.bundleNameLists[i].bundleName);
            }
        }
        private static string GetStreamingAssetsPath()
        {
            if (Application.isEditor)
                return Application.dataPath;
            else if (Application.isMobilePlatform || Application.isConsolePlatform)
                return Application.streamingAssetsPath;
            else // For standalone player.
                return "file://" + Application.streamingAssetsPath;
        }

        /// <summary>
        /// Sets base downloading URL to a directory relative to the streaming assets directory.
        /// Asset bundles are loaded from a local directory.
        /// </summary>
        public static void SetSourceAssetBundleDirectory(string relativePath)
        {
            BaseDownloadingURL = GetStreamingAssetsPath() + relativePath;
        }

        /// <summary>
        /// Sets base downloading URL to a web URL. The directory pointed to by this URL
        /// on the web-server should have the same structure as the AssetBundles directory
        /// in the demo project root.
        /// </summary>
        /// <example>For example, AssetBundles/iOS/xyz-scene must map to
        /// absolutePath/iOS/xyz-scene.
        /// <example>
        public static void SetSourceAssetBundleURL(string absolutePath)
        {
            if (absolutePath.StartsWith("/"))
            {
                absolutePath = "file://" + absolutePath;
            }
            if (!absolutePath.EndsWith("/"))
            {
                absolutePath += "/";
            }

            BaseDownloadingURL = absolutePath;// + "/";
        }

        /// <summary>
        /// Retrieves an asset bundle that has previously been requested via LoadAssetBundle.
        /// Returns null if the asset bundle or one of its dependencies have not been downloaded yet.
        /// </summary>
        static public LoadedAssetBundle GetLoadedAssetBundle(string assetBundleName, out string error)
        {
            if (downloadingErrors.TryGetValue(assetBundleName, out error))
                return null;

            LoadedAssetBundle bundle = null;
            loadedAssetBundles.TryGetValue(assetBundleName, out bundle);
            if (bundle == null)
            {
                return null;
            }
            // No dependencies are recorded, only the bundle itself is required.
            string[] dependencies = null;
            if (!AssetBundleManager.dependencies.TryGetValue(assetBundleName, out dependencies))
                return bundle;

            // Make sure all dependencies are loaded
            foreach (var dependency in dependencies)
            {
                if (downloadingErrors.TryGetValue(dependency, out error))
                    return null;

                // Wait all the dependent assetBundles being loaded.
                LoadedAssetBundle dependentBundle;
                loadedAssetBundles.TryGetValue(dependency, out dependentBundle);
                if (dependentBundle == null)
                    return null;
            }

            return bundle;
        }

        /// <summary>
        /// Returns true if certain asset bundle has been downloaded without checking
        /// whether the dependencies have been loaded.
        /// </summary>
        static public bool IsAssetBundleDownloaded(string assetBundleName)
        {
            return loadedAssetBundles.ContainsKey(assetBundleName);
        }

        /// <summary>
        /// Initializes asset bundle namager and starts download of manifest asset bundle.
        /// Returns the manifest asset bundle downolad operation object.
        /// </summary>
        static public AssetBundleLoadManifestOperation Initialize()
        {
            return Initialize(Utility.GetPlatformName());
        }

        /// <summary>
        /// Initializes asset bundle namager and starts download of manifest asset bundle.
        /// Returns the manifest asset bundle downolad operation object.
        /// </summary>
        static public AssetBundleLoadManifestOperation Initialize(string manifestAssetBundleName)
        {
#if UNITY_EDITOR
            // If we're in Editor simulation mode, we don't need the manifest assetBundle.
            if (SimulateAssetBundleInEditor)
                return null;
#endif

            LoadAssetBundle(manifestAssetBundleName, true);
            var operation = new AssetBundleLoadManifestOperation(manifestAssetBundleName, "AssetBundleManifest", typeof(AssetBundleManifest));
            inProgressOperations.Add(operation);
            return operation;
        }

        // Temporarily work around a il2cpp bug
        static protected void LoadAssetBundle(string assetBundleName)
        {
            LoadAssetBundle(assetBundleName, false);
        }
            
        // Starts the download of the asset bundle identified by the given name, and asset bundles
        // that this asset bundle depends on.
        static protected void LoadAssetBundle(string assetBundleName, bool isLoadingAssetBundleManifest)
        {
#if UNITY_EDITOR
            // If we're in Editor simulation mode, we don't have to really load the assetBundle and its dependencies.
            if (SimulateAssetBundleInEditor)
                return;
#endif

            if (!isLoadingAssetBundleManifest)
            {
                if (assetBundleManifest == null)
                {
                    return;
                }
            }

            // Check if the assetBundle has already been processed.
            bool isAlreadyProcessed = LoadAssetBundleInternal(assetBundleName, isLoadingAssetBundleManifest);

            // Load dependencies.
            if (!isAlreadyProcessed && !isLoadingAssetBundleManifest)
            {
                LoadDependencies(assetBundleName);
            }
        }

        // Returns base downloading URL for the given asset bundle.
        // This URL may be overridden on per-bundle basis via overrideBaseDownloadingURL event.
        protected static string GetAssetBundleBaseDownloadingURL(string bundleName)
        {
            if (overrideBaseDownloadingURL != null)
            {
                foreach (OverrideBaseDownloadingURLDelegate method in overrideBaseDownloadingURL.GetInvocationList())
                {
                    string res = method(bundleName);
                    if (res != null)
                        return res;
                }
            }
            return baseDownloadingURL;
        }

        // Checks who is responsible for determination of the correct asset bundle variant
        // that should be loaded on this platform. 
        //
        // On most platforms, this is done by the AssetBundleManager itself. However, on
        // certain platforms (iOS at the moment) it's possible that an external asset bundle
        // variant resolution mechanism is used. In these cases, we use base asset bundle 
        // name (without the variant tag) as the bundle identifier. The platform-specific 
        // code is responsible for correctly loading the bundle.
        static protected bool UsesExternalBundleVariantResolutionMechanism(string baseAssetBundleName)
        {
#if ENABLE_IOS_APP_SLICING
            var url = GetAssetBundleBaseDownloadingURL(baseAssetBundleName);
            if (url.ToLower().StartsWith("res://") ||
                url.ToLower().StartsWith("odr://"))
                return true;
#endif
            return false;
        }

        // Remaps the asset bundle name to the best fitting asset bundle variant.
        static protected string RemapVariantName(string assetBundleName)
        {
            string[] bundlesWithVariant = assetBundleManifest.GetAllAssetBundlesWithVariant();

            // Get base bundle name
            string baseName = assetBundleName.Split('.')[0];

            if (UsesExternalBundleVariantResolutionMechanism(baseName))
                return baseName;

            int bestFit = int.MaxValue;
            int bestFitIndex = -1;
            // Loop all the assetBundles with variant to find the best fit variant assetBundle.
            for (int i = 0; i < bundlesWithVariant.Length; i++)
            {
                string[] curSplit = bundlesWithVariant[i].Split('.');
                string curBaseName = curSplit[0];
                string curVariant = curSplit[1];

                if (curBaseName != baseName)
                    continue;

                int found = System.Array.IndexOf(activeVariants, curVariant);

                // If there is no active variant found. We still want to use the first
                if (found == -1)
                    found = int.MaxValue - 1;

                if (found < bestFit)
                {
                    bestFit = found;
                    bestFitIndex = i;
                }
            }

            if (bestFit == int.MaxValue - 1)
            {
            }

            if (bestFitIndex != -1)
            {
                return bundlesWithVariant[bestFitIndex];
            }
            else
            {
                return assetBundleName;
            }
        }

        // Sets up download operation for the given asset bundle if it's not downloaded already.
        static protected bool LoadAssetBundleInternal(string assetBundleName, bool isLoadingAssetBundleManifest)
        {
            // Already loaded.
            LoadedAssetBundle bundle = null;
            loadedAssetBundles.TryGetValue(assetBundleName, out bundle);
            if (bundle != null)
            {
                ++bundle.referencedCount;
                return true;
            }

            // @TODO: Do we need to consider the referenced count of WWWs?
            // In the demo, we never have duplicate WWWs as we wait LoadAssetAsync()/LoadLevelAsync() to be finished before calling another LoadAssetAsync()/LoadLevelAsync().
            // But in the real case, users can call LoadAssetAsync()/LoadLevelAsync() several times then wait them to be finished which might have duplicate WWWs.
            if (downloadingBundles.ContainsKey(assetBundleName))
            {
                int nPrevCount = downloadingBundles[assetBundleName];
                nPrevCount += 1;
                downloadingBundles[assetBundleName] = nPrevCount;
                return true;
            }

            string bundleBaseDownloadingURL = GetAssetBundleBaseDownloadingURL(assetBundleName);

            if (bundleBaseDownloadingURL.ToLower().StartsWith("odr://"))
            {
#if ENABLE_IOS_ON_DEMAND_RESOURCES
                Log(LogType.Info, "Requesting bundle " + assetBundleName + " through ODR");
                m_InProgressOperations.Add(new AssetBundleDownloadFromODROperation(assetBundleName));
#else
                new ApplicationException("Can't load bundle " + assetBundleName + " through ODR: this Unity version or build target doesn't support it.");
#endif
            }
            else if (bundleBaseDownloadingURL.ToLower().StartsWith("res://"))
            {
#if ENABLE_IOS_APP_SLICING
                Log(LogType.Info, "Requesting bundle " + assetBundleName + " through asset catalog");
                m_InProgressOperations.Add(new AssetBundleOpenFromAssetCatalogOperation(assetBundleName));
#else
                new ApplicationException("Can't load bundle " + assetBundleName + " through asset catalog: this Unity version or build target doesn't support it.");
#endif
            }
            else
            {
                if (!bundleBaseDownloadingURL.EndsWith("/"))
                {
                    bundleBaseDownloadingURL += "/";
                }

                string url = bundleBaseDownloadingURL + assetBundleName;
#if ABM_USE_UWREQ
                // If url refers to a file in StreamingAssets, use AssetBundle.LoadFromFileAsync to load.
                // UnityWebRequest also is able to load from there, but we use the former API because:
                // - UnityWebRequest under Android OS fails to load StreamingAssets files (at least Unity5.50 or less)
                // - or UnityWebRequest anyway internally calls AssetBundle.LoadFromFileAsync for StreamingAssets files
                if (url.StartsWith(Application.streamingAssetsPath)){ 
                    inProgressOperations.Add(new AssetBundleDownloadFileOperation(assetBundleName, url));
                } 
                else if (url.StartsWith(Application.persistentDataPath))
                {
                    inProgressOperations.Add(new AssetBundleLoadPersistentOperation(assetBundleName,url));
                }
                else {
                    
                    UnityWebRequest request = null;
                    if (isLoadingAssetBundleManifest) {
                        // For manifest assetbundle, always download it as we don't have hash for it.
                        request = UnityWebRequest.Get(url);
                    } else {
                        request = UnityWebRequest.Get(url);
                    }
                    inProgressOperations.Add(new AssetBundleDownloadWebRequestOperation(assetBundleName, request));
                }
#else
                WWW download = null;
                if (isLoadingAssetBundleManifest) {
                    // For manifest assetbundle, always download it as we don't have hash for it.
                    download = new WWW(url);
                } else {
                    download = WWW.LoadFromCacheOrDownload(url, m_AssetBundleManifest.GetAssetBundleHash(assetBundleName), 0);
                }
                m_InProgressOperations.Add(new AssetBundleDownloadFromWebOperation(assetBundleName, download));
#endif
            }
            downloadingBundles.Add(assetBundleName, 1);

            return false;
        }

        // Where we get all the dependencies and load them all.
        static protected void LoadDependencies(string assetBundleName)
        {
            if (assetBundleManifest == null)
            {
                return;
            }

            // Get dependecies from the AssetBundleManifest object..
            string[] dependencies = assetBundleManifest.GetAllDependencies(assetBundleName);
            if (dependencies.Length == 0)
                return;

            for (int i = 0; i < dependencies.Length; i++)
                dependencies[i] = RemapVariantName(dependencies[i]);

            // Record and load all dependencies.
            AssetBundleManager.dependencies.Add(assetBundleName, dependencies);
            for (int i = 0; i < dependencies.Length; i++)
                LoadAssetBundleInternal(dependencies[i], false);
        }

        /// <summary>
        /// Unloads assetbundle and its dependencies.
        /// </summary>
        static public void UnloadAssetBundle(string assetBundleName)
        {
#if UNITY_EDITOR
            // If we're in Editor simulation mode, we don't have to load the manifest assetBundle.
            if (SimulateAssetBundleInEditor)
                return;
#endif
            assetBundleName = RemapVariantName(assetBundleName);

            UnloadAssetBundleInternal(assetBundleName);
            UnloadDependencies(assetBundleName);
        }

        static protected void UnloadDependencies(string assetBundleName)
        {
            string[] dependencies = null;
            if (!AssetBundleManager.dependencies.TryGetValue(assetBundleName, out dependencies))
                return;

            // Loop dependencies.
            foreach (var dependency in dependencies)
            {
                UnloadAssetBundleInternal(dependency);
            }

            AssetBundleManager.dependencies.Remove(assetBundleName);
        }

        static protected void UnloadAssetBundleInternal(string assetBundleName)
        {
            string error;
            LoadedAssetBundle bundle = GetLoadedAssetBundle(assetBundleName, out error);
            if (bundle == null)
                return;

            if (--bundle.referencedCount == 0)
            {
                bundle.OnUnload();
                loadedAssetBundles.Remove(assetBundleName);
            }
        }
        void Update()
        {
            // Update all in progress operations
            for (int i = 0; i < inProgressOperations.Count;)
            {
                var operation = inProgressOperations[i];
                if (operation.Update())
                {
                    i++;
                }
                else
                {
                    inProgressOperations.RemoveAt(i);
                    ProcessFinishedOperation(operation);
                }
            }
        }
        public bool GetReadyStatus()
        {
            if (inProgressOperations.Count > 0)
            {
                return false;
            }
            return true;
        }

        void ProcessFinishedOperation(AssetBundleLoadOperation operation)
        {
            AssetBundleDownloadOperation download = operation as AssetBundleDownloadOperation;
            AssetBundleDownloadWebRequestOperation subdownload = operation as AssetBundleDownloadWebRequestOperation;
            AssetBundleLoadPersistentOperation localdownload = operation as AssetBundleLoadPersistentOperation;
 
            if (download == null&& subdownload == null&& localdownload==null)
            {
                return;
            }
            string error = string.Empty;
            string assetbundleName = string.Empty;
            LoadedAssetBundle loadedAssets = default;
            if (subdownload != null)
            {
                assetbundleName =  string.IsNullOrEmpty(download.assetBundleName)? subdownload.assetBundleName : download.assetBundleName;
                loadedAssets = download.assetBundle == null? subdownload.assetBundle: download.assetBundle;
                error = string.IsNullOrEmpty(download.error) ? subdownload.error : download.error;
            }
            else if (localdownload != null)
            {
                assetbundleName = localdownload.assetBundleName;
                loadedAssets = localdownload.loadedAssetBundle;
                error = localdownload.err; 
            }

            if ( downloadingBundles.ContainsKey(assetbundleName) == true )
            {
                int nRefCount = downloadingBundles[assetbundleName];
                if (loadedAssets != null)
                {
                    if (loadedAssets.assetBundle == null)
                    {
                        return;
                    }

                    loadedAssets.referencedCount = nRefCount;
                    downloadingBundles.Remove(assetbundleName);
                }
            }
            else
            {
                Debug.Log("m_DownloadingBundles not contain " + assetbundleName);
            }

            if (string.IsNullOrEmpty(error))
            {
                if(loadedAssets.assetBundle!=null)
                {
                    Debug.LogError("Add Asset" + assetbundleName);
                    loadedAssetBundles.Add(assetbundleName, loadedAssets);
                }
            }
            else
            {
                string msg = string.Format("Failed downloading bundle {0} from  {1}",
                       assetbundleName, error);
                downloadingErrors.Add(assetbundleName, msg);
            }       
        }

        /// <summary>
        /// Starts a load operation for an asset from the given asset bundle.
        /// </summary>
        static public AssetBundleLoadAssetOperation LoadAssetAsync(string assetBundleName, string assetName, System.Type type)
        {
            AssetBundleLoadAssetOperation operation = null;
#if UNITY_EDITOR
            if (SimulateAssetBundleInEditor)
            {
                string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, assetName);
                if (assetPaths.Length == 0)
                {
                    return null;
                }

                // @TODO: Now we only get the main object from the first asset. Should consider type also.
                UnityEngine.Object target = AssetDatabase.LoadMainAssetAtPath(assetPaths[0]);
                operation = new AssetBundleLoadAssetOperationSimulation(target);
            }
            else
#endif
            {
                assetBundleName = RemapVariantName(assetBundleName);
                LoadAssetBundle(assetBundleName);
                operation = new AssetBundleLoadAssetOperationFull(assetBundleName, assetName, type);

                inProgressOperations.Add(operation);
            }

            return operation;
        }

        /// <summary>
        /// Starts a load operation for a level from the given asset bundle.
        /// </summary>
        static public AssetBundleLoadOperation LoadLevelAsync(string assetBundleName, string levelName, bool isAdditive)
        {
            AssetBundleLoadOperation operation = null;
#if UNITY_EDITOR
            if (SimulateAssetBundleInEditor)
            {
                operation = new AssetBundleLoadLevelSimulationOperation(assetBundleName, levelName, isAdditive);
            }
            else
#endif
            {
                assetBundleName = RemapVariantName(assetBundleName);
                LoadAssetBundle(assetBundleName);
                operation = new AssetBundleLoadLevelOperation(assetBundleName, levelName, isAdditive);

                inProgressOperations.Add(operation);
            }

            return operation;
        }
    } // End of AssetBundleManager.
}
