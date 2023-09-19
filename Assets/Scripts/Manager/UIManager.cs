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
                    popUI.ExitWindow();
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
                //현재 아무것도 없을때 어떤것을 해주어야하는지
            }
        }
        public void PopUI()
        {
            if (windowStack.Count > 0)
            {
                if(currentUI is BaseUIWindow)
                {
                    PopupWindow();
                }
                else if(currentUI is BaseUIPopup)
                {
                    GetUI(popupStack.Pop()).ExitWindow();
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
            return PushUI(targetUIName);
        }
        public BaseUI PushUI(string uiType)
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
                    layerSortingOrder = SortingLayer.NameToID("Window");

                    BaseUIWindow targetUIWindow = targetUI.GetComponent<BaseUIWindow>();
                    targetUI.transform.parent = UIManager.Instance.baseCanvasObject.windowUIParent;
                    if (currentUI != null)
                    {
                        currentUI.ExitWindow();
                    }
                    currentUI = targetUIWindow;
                    popupStack.Clear();
                }
                else if (targetUI.GetComponent<BaseUIPopup>() != null)
                {
                    layerSortingOrder = SortingLayer.NameToID("Popup");
                    targetUI.transform.parent = UIManager.Instance.baseCanvasObject.popUpUIParent;

                    BaseUIPopup targetPopupWindow = targetUI.GetComponent<BaseUIPopup>();

                    if (currentUI != null)
                    {
                        currentUI.ExitWindow();
                    }
                    currentUI = targetPopupWindow;
                }
                else
                {
                    layerSortingOrder = SortingLayer.NameToID("Utils");
                    targetUI.transform.parent = UIManager.Instance.baseCanvasObject.UtilUIParent;
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

            if (uicomponent != null)
            {
                if (uicomponent is BaseUIWindow)
                {
                    BaseUIWindow pushWindow = uicomponent as BaseUIWindow;
                    if (!windowStack.Contains(targetUIName))
                    {
                        windowStack.Push(targetUIName);
                    }
                }
                else if (uicomponent is BaseUIPopup)
                {
                    BaseUIPopup pushPopup = uicomponent as BaseUIPopup;
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
                    layerSortingOrder = SortingLayer.NameToID("Window");
                    targetUI.transform.parent = UIManager.Instance.baseCanvasObject.windowUIParent;  
                }
                else if (targetUI.GetComponent<BaseUIPopup>() != null)
                {
                    layerSortingOrder = SortingLayer.NameToID("Popup");
                    targetUI.transform.parent = UIManager.Instance.baseCanvasObject.popUpUIParent;
                }
                else
                {
                    layerSortingOrder = SortingLayer.NameToID("Utils");
                    targetUI.transform.parent = UIManager.Instance.baseCanvasObject.UtilUIParent;
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
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {

            }
        }
    }
}