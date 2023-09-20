using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seunghak.UIManager
{
    using Seunghak.Common;
    public class UIManager : UnitySingleton<UIManager>
    {
        [SerializeField] BaseCanvas baseCanvasObject;
        private Stack<string> windowStack = new Stack<string>();
        private Stack<string> popupStack = new Stack<string>();
        private Dictionary<string,BaseUI> addedWindowLists = new Dictionary<string, BaseUI>();
        private string currentUIString = string.Empty;
        private BaseUI currentUI = null;

        #region UI_FlowLogic
        public void PopupWindow()
        {
            //윈도우 뺴주는 작업 연계되어있는 것들을 다뺸다.
            while (windowStack.Count > 0)
            {
                BaseUI popUI = GetUI(windowStack.Pop());
                if(popUI is BaseUIWindow)
                {
                    if(popUI is LobbyWindow)
                    {
                        break;
                    }
                    popUI.ExitWindow();
                    currentUI = GetUI(windowStack.Peek());
                    break;
                }
                else
                {
                    popupStack.Pop();
                    popUI.ExitWindow();
                }
            }


            if (windowStack.Count == 0)
            {
                PushUI(UI_TYPE.LobbyWindow);
            }
        }
        public void PopUI()
        {
            if (windowStack.Count > 0)
            {
                if (currentUI == null)
                {
                    currentUI = GetUI(currentUIString);
                }
                if(currentUI is BaseUIWindow)
                {
                    PopupWindow();
                }
                else if(currentUI is BaseUIPopup)
                {
                    popupStack.Clear();
                    currentUI = GetUI(windowStack.Peek());
                }
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
            while (windowStack.Count > 0)
            {
                BaseUI popuserUI = GetUI(windowStack.Peek());
                if (popuserUI is BaseUIWindow)
                {
                    popuserUI.RestoreWindow();
                    break;
                }
                else
                {
                    PopUI();
                }
                popuserUI.RestoreWindow();
            }
        }
        public BaseUI PushUI(UI_TYPE uiType)
        {
            string targetUIName = uiType.ToString();
            BaseUI uicomponent = null;

            if (addedWindowLists.ContainsKey(targetUIName))
            {
                uicomponent = addedWindowLists[targetUIName];
            }

            if (uicomponent == null)
            {
                GameObject targetUI = GameResourceManager.Instance.SpawnObject(targetUIName);

                if (targetUI == null)
                {
                    Debug.Log("Error : TargetUI Object is Empty");
                    return null;
                }
                targetUI.SetActive(true);
                uicomponent = targetUI.GetComponent<BaseUI>();
                addedWindowLists.Add(targetUIName, uicomponent);
            }

            if (uicomponent != null)
            {
                uicomponent.CUR_UI_TYPE = uiType; 
                int layerSortingOrder = 0;
                if (uicomponent.GetComponent<BaseUIWindow>() != null)
                {
                    layerSortingOrder = 10;

                    BaseUIWindow targetUIWindow = uicomponent.GetComponent<BaseUIWindow>();
                    uicomponent.transform.parent = UIManager.Instance.baseCanvasObject.windowUIParent;
                    if (currentUI is BaseUIPopup)
                    {
                        currentUI.ExitWindow();
                        GetUI(windowStack.Peek()).gameObject.SetActive(false);
                    }
                    else if (currentUI is BaseUIWindow)
                    {
                        currentUI.gameObject.SetActive(false);
                    }
                    currentUI = targetUIWindow;
                    currentUIString = targetUIName;
                    popupStack.Clear();
                }
                else if (uicomponent.GetComponent<BaseUIPopup>() != null)
                {
                    layerSortingOrder = 100;
                    uicomponent.transform.parent = UIManager.Instance.baseCanvasObject.popUpUIParent;

                    BaseUIPopup targetPopupWindow = uicomponent.GetComponent<BaseUIPopup>();

                    if (currentUI is BaseUIPopup)
                    {
                        currentUI.ExitWindow();
                    }
                    currentUI = targetPopupWindow;
                    currentUIString = targetUIName;
                }
                else
                {
                    layerSortingOrder = 5000;
                    uicomponent.transform.parent = UIManager.Instance.baseCanvasObject.utilUIParent;
                }
                uicomponent.transform.position = Vector3.zero;
                Canvas targetUICanvas = uicomponent.GetComponent<Canvas>();
                if (targetUICanvas != null)
                {
                    targetUICanvas.sortingOrder = layerSortingOrder;
                }
                else
                {
                    Debug.Log("Cavas is Null ");
                }
                if (uicomponent.GetComponent<RectTransform>() != null)
                {
                    uicomponent.GetComponent<RectTransform>().localPosition = Vector2.zero;
                }
            }

            if (uicomponent != null)
            {
                if (uicomponent is BaseUIWindow)
                {
                    if (!windowStack.Contains(targetUIName))
                    {
                        windowStack.Push(targetUIName);
                    }
                    else
                    {
                        while (windowStack.Peek() != targetUIName)
                        {
                            GetUI(windowStack.Peek()).ExitWindow();
                        }
                    }
                }
                else if (uicomponent is BaseUIPopup)
                {
                    if (!popupStack.Contains(targetUIName))
                    {
                        popupStack.Push(targetUIName);
                    }
                }
                uicomponent.EnterWindow();
            }
            return uicomponent;
        }
        public BaseUI GetUI(string uiType)
        {
            string targetUIName = uiType;
            BaseUI uicomponent = null;

            if (addedWindowLists.ContainsKey(targetUIName))
            {
                uicomponent = addedWindowLists[targetUIName];
            }
            if (uicomponent == null)
            {
                GameObject targetUI = GameResourceManager.Instance.SpawnObject(targetUIName);

                if (targetUI == null)
                {
                    Debug.Log("Error : TargetUI Object is Empty");
                    return null;
                }
                targetUI.SetActive(true);
                uicomponent = targetUI.GetComponent<BaseUI>();

                int layerSortingOrder = 0;

                if (targetUI.GetComponent<BaseUIWindow>() != null)
                {
                    layerSortingOrder = 10;
                    targetUI.transform.parent = UIManager.Instance.baseCanvasObject.windowUIParent;  
                }
                else if (targetUI.GetComponent<BaseUIPopup>() != null)
                {
                    layerSortingOrder = 100;
                    targetUI.transform.parent = UIManager.Instance.baseCanvasObject.popUpUIParent;
                }
                else
                {
                    layerSortingOrder = 5000;
                    targetUI.transform.parent = UIManager.Instance.baseCanvasObject.utilUIParent;
                }
                targetUI.transform.position = Vector3.zero;
                Canvas targetUICanvas = targetUI.GetComponent<Canvas>();
                if (targetUICanvas != null)
                {
                    targetUICanvas.sortingLayerID = layerSortingOrder;
                }
                else
                {
                    Debug.Log("Cavas is Null ");
                }
                if (targetUI.GetComponent<RectTransform>() != null)
                {
                    targetUI.GetComponent<RectTransform>().localPosition = Vector2.zero;
                }

                addedWindowLists.Add(targetUIName, uicomponent);
            }
            return uicomponent;
        }
        #endregion UI_FlowLogic
        #region InGame_Logic
        public BaseUIWindow GetCurWindow()
        {
            return GetUI(windowStack.Peek()) as BaseUIWindow;
        }
        public void SetTopUI(UI_TYPE curUIType)
        {
            baseCanvasObject.OpenBaseTopUI(curUIType);
        }
        #endregion InGame_Logic
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {

            }
        }
    }
}