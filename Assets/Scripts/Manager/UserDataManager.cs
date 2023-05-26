using Seunghak.LoginSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seunghak.Common
{
    public class UserDataManager : UnitySingleton<UserDataManager>
    {
        private E_LOGIN_TYPE userLoginType = E_LOGIN_TYPE.GUEST_LOGIN;
        private LoginInterface userLogin;
        private string userIDToken = "";
        public string UserIDToken
        {
            get
            {
                return userIDToken;
            }
        }
        public void SetLoginInfo(E_LOGIN_TYPE loginType)
        {
            switch (loginType)
            {
                case E_LOGIN_TYPE.GUEST_LOGIN:
                    userLogin = new GuestLogin();
                    break;
                case E_LOGIN_TYPE.GOOGLE_LOGIN:
                    userLogin = new GoogleLogin();
                    break;
                case E_LOGIN_TYPE.APPLE_LOGIN:
                    userLogin = new AppleLogin();
                    break;
            }
            userLoginType = loginType;

            userLogin.InitLogin();
        }      
        public void LoginPlatform()
        {
            userLogin.PlatformLogin();
        }
    }
}