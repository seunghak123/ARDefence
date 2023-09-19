using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seunghak.UIManager
{
    public class ShopWindow : BaseUIWindow
    {
        public void EnterShopInfo()
        {
            UIManager.Instance.PushUI(UI_TYPE.ShopBuyPopup);
        }
        public void EnterBattleWindow()
        {
            UIManager.Instance.PushUI(UI_TYPE.BattleWindow);
        }
        public override void EnterWindow()
        {
            base.EnterWindow();
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