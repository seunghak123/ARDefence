using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seunghak.UIManager
{
    using Seunghak.Common;
    public class UIManager : UnitySingleton<UIManager>
    {
        [SerializeField] BaseCanvas baseCanvasObject;
        private Stack<BaseUI> windowStack = new Stack<BaseUI>();
        private Dictionary<string,BaseUI> addedWindowLists = new Dictionary<string, BaseUI>();
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
        }
        public void PopAllWindow()
        {
            while (windowStack.Count > 0)
            {
                PopUI();
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
        public void PushUI(UI_TYPE uiType)
        {
            string targetUIName = uiType.ToString();

            BaseUI uicomponent;

            if (addedWindowLists.ContainsKey(targetUIName))
            {
                uicomponent = addedWindowLists[targetUIName];
            }
            else
            {
                GameObject targetUI = GameResourceManager.Instance.SpawnObject(targetUIName);

                if (targetUI == null)
                {
                    Debug.Log("Error : TargetUI Object is Empty");
                    return;
                }

                targetUI.SetActive(true);
                if (targetUI.GetComponent<BaseUIWindow>() != null)
                {
                    targetUI.transform.parent = BaseCanvas.Instance.windowUIParent;
                }
                else if (targetUI.GetComponent<BaseUIPopup>() != null)
                {
                    targetUI.transform.parent = BaseCanvas.Instance.popUpUIParent;
                }
                else
                {
                    targetUI.transform.parent = BaseCanvas.Instance.UtilUIParent;
                }
                uicomponent = targetUI.GetComponent<BaseUI>();

                addedWindowLists.Add(targetUIName, uicomponent);
            }

            if (uicomponent != null)
            {
                if (!windowStack.Contains(uicomponent))
                {
                    windowStack.Push(uicomponent);
                }
                uicomponent.EnterWindow();
            }

            currentUI = uicomponent;
        }
    }
}