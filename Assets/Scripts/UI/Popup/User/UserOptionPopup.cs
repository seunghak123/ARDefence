using Seunghak.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seunghak.UIManager
{
    public class UserOptionPopup : BaseUIPopup
    {
        private UserOptionData userOption;
        public override void EnterWindow()
        {
            base.EnterWindow();
        }
        private void InitOptionPopupUI()
        {
            userOption = UserDataManager.Instance.GetUserOptionData();
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