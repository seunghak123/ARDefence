using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Seunghak.UIManager
{
    public class LobbyWindow : BaseUIWindow
    {
        [SerializeField] private UserInfoUI userInfoDataUI;
        
        [SerializeField] private Texture backgroundTexture;
        //이벤트 슬라이더

        public override void EnterWindow()
        {
            base.EnterWindow();

            //userInfoDataUI.SetData();
        }
        public void EnterShopWindow()
        {
            UIManager.Instance.PushUI(UI_TYPE.ShopWindow);
        }
        public void EnterBattleWindow() 
        {
            UIManager.Instance.PushUI(UI_TYPE.BattleWindow);
        }
        public void OpenEventPopup()
        {
            UIManager.Instance.PushUI(UI_TYPE.UserEventPopup);
            //현재 이벤트 팝업을 띄우고, 이벤트 데이터 갱신
        }
        public override void ExitWindow()
        {
            base.ExitWindow();
        }

        public override void RestoreWindow()
        {
            base.RestoreWindow();
        }

        public override void StartWindow()
        {
            base.StartWindow();
        }
        public override void RegistEvent()
        {
            base.RegistEvent();
        }
        public override void DeleteRegistedEvent()
        {
            base.DeleteRegistedEvent();
        }
    }
}