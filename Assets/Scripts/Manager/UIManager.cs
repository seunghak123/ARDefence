using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seunghak.UIManager
{
    using Seunghak.Common;
    public class UIManager : UnitySingleton<UIManager>
    {
        private Stack<BaseUI> windowStack = new Stack<BaseUI>();
        private BaseUI currentUI = null;
        public void PopupWindow()
        {
            //윈도우 뺴주는 작업 연계되어있는 것들을 다뺸다.
            while (windowStack.Count > 0)
            {
                BaseUI popUI = windowStack.Pop();
                if(popUI is BaseUIWindow)
                {
                    popUI.ExitWindow();
                    break;
                }
                else
                {
                    popUI.ExitWindow();
                }
            }


            if (windowStack.Count == 0)
            {
                //push lobbywindow
            }
        }
        public void PopUI()
        {
            if (windowStack.Count > 0)
            {
                BaseUI popUI = windowStack.Pop();

                popUI.ExitWindow();
            }
            //상위 UI하나만 뺴주는 작업
        }
        public void PopAllWindow()
        {
            while (windowStack.Count > 0)
            {
                BaseUI popUI = windowStack.Pop();

                //데이터 세팅된 값들 제거, 
            }
        }
        public void RestoreWindow()
        {
            Stack<BaseUI> removedPopup = new Stack<BaseUI>();
            while (windowStack.Count > 0)
            {
                BaseUI popUI = windowStack.Peek();
                if (popUI is BaseUIWindow)
                {
                    popUI.RestoreWindow();
                    break;
                }
                else
                {
                    popUI.ExitWindow();
                }
                popUI.RestoreWindow();
            }
        }
        public void PushUI(BaseUI stackUI)
        {
            windowStack.Push(stackUI);
        }
        public void PushUI(UI_TYPE uiType)
        {
            //GameResourceManager.Instance.s
        }
    }
}