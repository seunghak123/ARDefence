using TMPro;
using UnityEngine;

namespace Seunghak.UIManager
{
    public class BaseTopUI : BaseUI
    {
        [SerializeField] private UserInfoUI userInfoUIObject;
        [SerializeField] private WealthUI[] wealthUIObjects;
        public void InitTopUI(UI_TYPE curUIType)
        {
            //재화 세팅

            switch (curUIType)
            {
                case UI_TYPE.LobbyWindow:

                    break;
                case UI_TYPE.ShopWindow:
                    break;
                case UI_TYPE.BattleWindow:
                    break;
                default:
                    break;
            }
        }
        private void SetWealthUI()
        {

        }
        //옵션 버튼 클래스
        //재화 세팅 함수
        
        //재화 탭
        //유저 탭
        //버튼 리스트들

    }
}
