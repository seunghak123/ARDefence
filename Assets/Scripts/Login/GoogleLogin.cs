using UnityEngine;

namespace Seunghak.LoginSystem
{
    using GooglePlayGames;
    using System;

    public class GoogleLogin :  LoginInterface
    {
        private Action loginSuccessAction = null;
        public void InitLogin()
        {
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();
        }
        public void PlatformLogin(Action successAction)
        {
            loginSuccessAction = successAction;
            PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
        }

        internal void ProcessAuthentication(GooglePlayGames.BasicApi.SignInStatus status)
        {
            if (status == GooglePlayGames.BasicApi.SignInStatus.Success)
            {
                GoogleLoginSuccess();
            }
            else
            {
                GoogleLoginFail();
            }
        }
        private void GoogleLoginSuccess()
        {
            loginSuccessAction();
            //서버에 해당 UserID를 보낼것
            //PlayGamesPlatform.Instance.GetUserId();
        }
        private void GoogleLoginFail()
        {
            Debug.Log("LoginFaile");
        }

        public void LogOut()
        {
            PlayGamesPlatform.Activate();
        }
    }
}

