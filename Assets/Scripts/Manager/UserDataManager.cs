using Seunghak.LoginSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seunghak.Common
{
    public enum PlayerPrefKey
    {
        SaveTest,
    }
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
        public static void SavePlayerPref<T>(PlayerPrefKey saveKey, T saveData)
        {
            PlayerPrefs.SetString(saveKey.ToString(), saveData.ToString());

            PlayerPrefs.Save();
        }
        public static T GetPlayerPref<T>(PlayerPrefKey saveKey)
        {
            string getValue = PlayerPrefs.GetString(saveKey.ToString());
            T convertValue;
            try
            {
                convertValue = (T)Convert.ChangeType(getValue, typeof(T));
            }
            catch(Exception e)
            {
                Debug.Log($"ConvertValue Error {e.Message}");
                convertValue = default(T);
            }
            return convertValue;
        }
        private static string attendString = "SaveBundleHash";
        public static void SaveAssetBundleHash(string assetName,long saveValue)
        {
            string saveKey = $"{attendString}_{assetName}";
            PlayerPrefs.SetString(saveKey, saveValue.ToString());
        }
        public static long GetAssetBundleLocalHash(string assetName)
        {
            long returnValue = 0;
            string loadKey = $"{attendString}_{assetName}";
            string getValue = PlayerPrefs.GetString(loadKey);
            try
            {
                returnValue = (long)Convert.ChangeType(getValue, typeof(long));
            }
            catch (Exception e)
            {
                Debug.Log($"Convert HashValue Error {e.Message}");
            }
            return returnValue;
        }
    }
    class UserOptionData
    {
        [Range(0, 100)] private int masterVolume = 50;
        [Range(0, 100)] private int soundVolume = 50;
        [Range(0, 100)] private int fbxVolume = 50;
        private E_LANGUAGE_TYPE userLangType = E_LANGUAGE_TYPE.KOREAN;
        public int MasterVolume { get { return masterVolume; } set { masterVolume = value; } }
        public int SoundVolume { get { return soundVolume; } set { soundVolume = value; } }
        public int FBXVolume { get { return fbxVolume; } set { fbxVolume = value; } }
        public bool IsMute { get; set; } = false;
        public E_LANGUAGE_TYPE UserLanguageType { get { return userLangType; }set { userLangType = value; } }
    }
}