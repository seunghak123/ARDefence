using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seunghak.UIManager
{
    public class BaseUIWindow : BaseUI
    {
        public override void EnterWindow()
        {
            base.EnterWindow();
            UIManager.Instance.SetTopUI(ui_Type);
        }
    }
}

