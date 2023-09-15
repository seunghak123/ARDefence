using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Seunghak.Common
{
    public class SpriteManager : UnitySingleton<SpriteManager>
    {
        private List<SpriteAtlas> spriteAtlasLists = new List<SpriteAtlas>();
        private Dictionary<string, string> spriteAtlasPathDic = new Dictionary<string, string>();
        protected override void InitSingleton()
        {
            InitAtlasLists();
        }
        public Sprite LoadSprite(string spriteName)
        {
            return null;
        }
        private void InitAtlasLists()
        {
            SpriteAtlasManager.atlasRequested += RequestAtlasCallback;

            spriteAtlasLists.Clear();
            spriteAtlasPathDic = new Dictionary<string, string>();

             AtlasLists atlasLists;
            atlasLists.atlaseLists = new List<AtlasInfo>();

            string atlasfilePath = Application.dataPath;

#if UNITY_EDITOR
            atlasfilePath = $"{FileUtils.ATLAS_SAVE_PATH}/{FileUtils.ATLAS_LIST_FILE_NAME}";
                     
            string bundleFilePath = $"{Application.dataPath}{FileUtils.GetPlatformString()}{ FileUtils.BUNDLE_LIST_FILE_NAME}";
                 
            BundleListsDic bundleLists = FileUtils.LoadFile<BundleListsDic>(bundleFilePath);
            atlasLists = FileUtils.LoadFile<AtlasLists>(atlasfilePath);

            for (int i = 0; i < atlasLists.atlaseLists.Count; i++)
            {
                BundleListInfo listInfo = bundleLists.bundleNameLists.Find(find => find.bundleName == "AtlasList");
                if (!string.IsNullOrEmpty(listInfo.bundleName))
                {
                    List<BundleFileInfo> infos = bundleLists.bundleObjectLists[listInfo.bundleName];
                    for(int j=0;j< infos.Count; j++)
                    {
                        SpriteAtlas atlasSprits = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(infos[j].filePath);

                        if (atlasSprits != null)
                        {
                            spriteAtlasLists.Add(atlasSprits);
                        }
                    }
                }

            }

            for (int i = 0; i < atlasLists.atlaseLists.Count; i++)
            {

                SpriteAtlas atlasSprits = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasLists.atlaseLists[i].atlasName);
            }

            atlasLists = JsonUtility.FromJson<AtlasLists>(GameResourceManager.Instance.LoadObject("AtlasList").ToString());
            //GameResourceManager.Instance.LoadObject()

#else
            //1. 아틀라스 리스트 관련 파일 읽기
            //2. 아틀라스 리스트 저장및 구조저장
#endif
            for (int i = 0; i < atlasLists.atlaseLists.Count; i++)
            {
                //SpriteAtlas atlasSprits = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasLists.atlaseLists[i].atlasName);
            }
        }
        private void RequestAtlasCallback(string tag, System.Action<SpriteAtlas> callback)
        {

        }
    }
}