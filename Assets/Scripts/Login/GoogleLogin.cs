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
            Debug.Log("LoginSuccess");
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

