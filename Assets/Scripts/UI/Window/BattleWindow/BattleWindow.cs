using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seunghak.UIManager
{
    public class BattleWindow : BaseUIWindow
    {
        public void GoToLobby()
        {
            UIManager.Instance.PushUI(UI_TYPE.LobbyWindow);
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