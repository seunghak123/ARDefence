using UnityEngine;

namespace Seunghak.LoginSystem
{
    using GooglePlayGames;
    public class GoogleLogin :  LoginInterface
    {
        public void InitLogin()
        {
            PlayGamesPlatform.DebugLogEnabled = true;
            PlayGamesPlatform.Activate();
        }
        public void PlatformLogin()
        {
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

