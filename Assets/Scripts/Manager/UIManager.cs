using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seunghak.UIManager
{
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

                    break;
                }
                else
                {
                    //popupUI일떄 

                }
                popUI.ExitWindow();
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
            //게임 씬이나, 타 씬에서 복귀했을떄 불러줄 함수 로비로 가지않는한
            //해당 함수가 불린다. 팝업과 윈도우가 둘다 복구 윈도우 초기화 함수 필요

        }
        public void PushUI(BaseUI stackUI)
        {
            windowStack.Push(stackUI);
        }
    }
}