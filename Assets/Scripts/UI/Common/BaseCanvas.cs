using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seunghak.UIManager
{
    public class BaseCanvas : UnitySingleton<BaseCanvas>
    {
        [SerializeField] public Transform windowUIParent;
        [SerializeField] public Transform popUpUIParent;
        [SerializeField] public Transform utilUIParent;
        [SerializeField] public BaseTopUI baseTopUI;

        public void OpenBaseTopUI(UI_TYPE curUIType)
        {
            if(baseTopUI != null)
            {
                baseTopUI.gameObject.SetActive(true);
                baseTopUI.InitTopUI(curUIType);
            }
        }
    }
}
