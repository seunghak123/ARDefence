﻿using UnityEngine;

namespace Seunghak.LoginSystem
{
    public class GoogleLogin : AndroidGoogleSignIn, LoginInterface
    {
        private string webCliendId = "444202219035-3b9rutbd7pcrfpjvhmtck62e1pi4tn6k.apps.googleusercontent.com";
        private string googleLoginId;
        private AndroidGoogleSignInAccount googleUserData;
        public void InitLogin()
        {
            Init(new GameObject());
        }

        public void PlatformLogin()
        {
            Debug.Log("SignIn Google ");
            SignIn(webCliendId, GoogleLoginSuccess, GoogleLoginFail);
        }
        private void GoogleLoginSuccess(AndroidGoogleSignInAccount userAccount)
        {
            googleUserData = userAccount;

            Debug.Log("UserToken " + userAccount.Token);
            //여기에서
            //googleUserData.Token 값에 따라서 로그인
        }
        private void GoogleLoginFail(string errorCallback)
        {
            Debug.Log($"Error Message : {errorCallback}");
        }
    }
}

