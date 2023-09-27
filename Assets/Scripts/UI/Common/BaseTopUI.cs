using Seunghak.Common;
using TMPro;
using UnityEngine;

namespace Seunghak.UIManager
{
    public class BaseTopUI : BaseUI
    {
        [SerializeField] private WealthUI[] wealthUIObjects;
        public void InitTopUI(UI_TYPE curUIType)
        {
            //재화 세팅

            switch (curUIType)
            {
                case UI_TYPE.LobbyWindow:
                    SetWealthUI(E_ITEM_TYPE.GOLD, E_ITEM_TYPE.CRYSTALS);
                    break;
                case UI_TYPE.ShopWindow:
                    SetWealthUI(E_ITEM_TYPE.GOLD, E_ITEM_TYPE.CRYSTALS);
                    break;
                case UI_TYPE.BattleWindow:
                    break;
                default:
                    break;
            }
        }
        private void SetWealthUI(params E_ITEM_TYPE[] wealthId)
        {
            for(int i=0;i< wealthUIObjects.Length; i++)
            {
                wealthUIObjects[i].gameObject.SetActive(false);
            }

            for(int i = 0;   i< wealthId.Length; i++)
            {
                E_ITEM_TYPE itemType = wealthId[i];
                wealthUIObjects[i].gameObject.SetActive(true);
                UserItemDatas itemData = UserDataManager.Instance.GetUserItemData();
                wealthUIObjects[i].SetWealth("boss_bomb", itemData.GetItemCount(itemType));
            }
        }
        //옵션 버튼 클래스
        //재화 세팅 함수

        //재화 탭
        //유저 탭
        //버튼 리스트들

    }
}
