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
            Object target = GameResourceManager.Instance.LoadObject(spriteName);
            if (target != null)
            {
                return target as Sprite;
            }

            if (spriteAtlasPathDic.ContainsKey(spriteName))
            {
                SpriteAtlas targetAtlas = spriteAtlasLists.Find(find => find.name == spriteAtlasPathDic[spriteName]);
                if (targetAtlas == null)
                {
                    Debug.Log($"{spriteAtlasPathDic[spriteName]} atlas no there");
                    return null;
                }
                return targetAtlas.GetSprite(spriteName);
            }
            else
            {
                Debug.Log($"{spriteName} sprite nothing");
                //없어요 없어!
                return null;
            }
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
                BundleListInfo listInfo = bundleLists.bundleNameLists.Find(find => find.bundleName == "atlas");
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
                for(int j = 0; j < atlasLists.atlaseLists[i].spriteLists.Count; j++)
                {
                    spriteAtlasPathDic[atlasLists.atlaseLists[i].spriteLists[j]] = atlasLists.atlaseLists[i].atlasName;
                }
            }
#else
            atlasLists = JsonUtility.FromJson<AtlasLists>(GameResourceManager.Instance.LoadObject("AtlasList").ToString());

            for (int i = 0; i < atlasLists.atlaseLists.Count; i++)
            {
                SpriteAtlas atlas =  GameResourceManager.Instance.LoadObject(atlasLists.atlaseLists[i].atlasName) as SpriteAtlas;
                if (atlas != null)
                {
                    spriteAtlasLists.Add(atlas);

                    for (int j = 0; j < atlasLists.atlaseLists[i].spriteLists.Count; j++)
                    {
                        spriteAtlasPathDic[atlasLists.atlaseLists[i].spriteLists[j]] = atlasLists.atlaseLists[i].atlasName;
                    }
                }
            }
#endif
        }
        private void RequestAtlasCallback(string tag, System.Action<SpriteAtlas> callback)
        {

        }
    }
}