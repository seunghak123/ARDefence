using System;
using UnityEngine;

namespace Seunghak.UIManager
{
    public enum UI_TYPE_PROPERTY
    {
        None,
        Window,
        Popup,
        Utils,
    }
    public enum UI_TYPE
    {
        //1~100까진 기본 Window
        LobbyWindow,
        TitleWindow,
        ShopWindow,
        BattleWindow,

        BasePopupWindow = 1000,
        UserInfoPopup ,
        ShopBuyPopup,
    }
    public interface IBaseUIController
    {
        void EnterWindow();
        void StartWindow();
        void ExitWindow();
        void RestoreWindow();
    }

    public class BaseUI : MonoBehaviour, IBaseUIController
    {
        private Action exitAction = null;
        
        public void SetAction(Action exit)
        {
            exitAction = exit;
        }
        public virtual void EnterWindow()
        {
            this.gameObject.SetActive(true);
        }

        public virtual void ExitWindow()
        {
            UIManager.Instance.PopUI();
            if (exitAction != null)
            {
                exitAction();
            }

            this.gameObject.SetActive(false);
        }

        public virtual void RestoreWindow()
        {
            //파티클 긁어다가 갱신 
            this.gameObject.SetActive(true);
        }

        public virtual void StartWindow()
        {

        }
        public virtual void RegistEvent()
        {
            //버튼 이벤트 등을 등록해주는 함수
        }
        public virtual void DeleteRegistedEvent()
        {
            //버튼 이벤트를 제거해주는 함수
        }
    }
}