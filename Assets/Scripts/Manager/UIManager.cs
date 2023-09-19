﻿using System.Collections;
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
        private BaseUIWindow currentWindowUI = null;
        private BaseUIPopup currentPopupUI = null;
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
                //현재 아무것도 없을때 어떤것을 해주어야하는지
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
        public BaseUI PushUI(UI_TYPE uiType)
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

                    if (currentWindowUI != null)
                    {
                        currentWindowUI.ExitWindow();
                    }
                    currentWindowUI = targetUIWindow;
                    //기존 UI window는 잠궈줘야한다.
                }
                else if (targetUI.GetComponent<BaseUIPopup>() != null)
                {
                    layerSortingOrder = SortingLayer.NameToID("Popup");
                    targetUI.transform.parent = UIManager.Instance.baseCanvasObject.popUpUIParent;

                    BaseUIPopup targetPopupWindow = targetUI.GetComponent<BaseUIPopup>();

                    if (currentPopupUI != null)
                    {
                        currentPopupUI.ExitWindow();
                    }
                    currentPopupUI = targetPopupWindow;
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
                if (!windowStack.Contains(uicomponent))
                {
                    windowStack.Push(uicomponent);
                }
                uicomponent.EnterWindow();
            }
            return uicomponent;
        }
    }
}