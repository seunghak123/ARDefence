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
        protected override void InitSingleton()
        {
            InitAtlasLists();
        }
        public Sprite LoadSprite(string spriteName)
        {
            //string atlasPath 
            return null;
            //아틀라스에서 스프라이트 꺼내온다 스프라이트는 반드시 동일한 이름을 사용하지 않을 것!!
        }
        private void InitAtlasLists()
        {
            SpriteAtlasManager.atlasRequested += RequestAtlasCallback;

            //아틀라스 로드
            //모든 아틀라스 리스트를 알아야하고, 그것을 요청해야한다.
            //SpriteAtlas _sprite = AssetBundleManager.Instance..  Resources.Load<SpriteAtlas>("path");

            //아틀라스 데이터 세팅 
            //Sprite _exSprite = _sprite.GetSprite("name");
        }
        private void RequestAtlasCallback(string tag, System.Action<SpriteAtlas> callback)
        {

        }
    }
}