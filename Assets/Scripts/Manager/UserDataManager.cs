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
        UserData,
        UserToken,
    }
    public class UserDataManager : UnitySingleton<UserDataManager>
    {
        private UserDataInfo userDataInfo = new UserDataInfo();
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
#if UNITY_ANDROID || UNITY_IOS
                case E_LOGIN_TYPE.GOOGLE_LOGIN:
                    userLogin = new GoogleLogin();
                    break;
                case E_LOGIN_TYPE.APPLE_LOGIN:
                    userLogin = new AppleLogin();
                    break;
#endif
            }
            userLoginType = loginType;

            userLogin.InitLogin();
        }      
        public void LoginPlatform(Action successResultAction)
        {
            userLogin.PlatformLogin(successResultAction);
        }
        public int GetUserItemCount(int id)
        {
            return 0;
        }
        public void SetUserToken(string userToken)
        {
            userIDToken = userToken;
        }
        public bool AutoLogin()
        {
            userIDToken = GetPlayerPref<string>(PlayerPrefKey.UserToken);
            if(string.IsNullOrEmpty(userIDToken))
            {
                return false;
            }
            //userIDToken 으로 서버쪽에 로그인 메소드 보내고

            return true;
        }
        public UserItemDatas GetUserItemData()
        {
            return userDataInfo.userItemDatas;
        }
        public UserOptionData GetUserOptionData()
        {
            return userDataInfo.userOption;
        }

        #region DB_SAVE
        /// <summary>
        /// 해당 내용들은 로컬 DB를 사용할 경우에만 해당한다
        /// 하나는 PlayerPref를 이용 차후엔 확장해서 직접적으로 호출하는게 아닌 
        /// DB매니저를 통해서 어떤 DB를 통하든 특정 인터페이스를 상속받은 DB의 행동을 실행하도록한다.
        /// </summary>
        public void SaveUserData()
        {
            SavePlayerPref<UserDataInfo>(PlayerPrefKey.UserData, userDataInfo);
        }
        public void LoadUserData()
        {
            userDataInfo = GetPlayerPref<UserDataInfo>(PlayerPrefKey.UserData);
        }
        #endregion DB_SAVE
        #region PlayerPref
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
        #endregion PlayerPref
    }
    public class UserItemDatas
    {
        private Dictionary<int, long> itemDic = new Dictionary<int, long>();

        public bool AddItem(int id, long itemCount)
        {
            if ((int)E_ITEM_TYPE.CRYSTALS == id || (int)E_ITEM_TYPE.VALID_CRYSTALS == id)
            {
                long useritemCount = itemDic[(int)E_ITEM_TYPE.CRYSTALS] + itemDic[(int)E_ITEM_TYPE.VALID_CRYSTALS];

                if (itemCount > 0)
                {
                    itemDic[id] += itemCount;
                    return true;
                }
                else
                {
                    long remainCount = 0;
                    if (useritemCount + itemCount > 0)
                    {
                        itemDic[(int)E_ITEM_TYPE.VALID_CRYSTALS] -= itemCount;
                        if (itemDic[(int)E_ITEM_TYPE.VALID_CRYSTALS] < 0)
                        {
                            remainCount = itemDic[(int)E_ITEM_TYPE.VALID_CRYSTALS];
                            itemDic[(int)E_ITEM_TYPE.VALID_CRYSTALS] = 0;
                        }
                        itemDic[(int)E_ITEM_TYPE.CRYSTALS] -= remainCount;

                        return true;
                    }
                    return false;
                }

            }
            else
            {
                if (itemDic[id] + itemCount > 0)
                {
                    itemDic[id] += itemCount;
                    return true;
                }
                return false;
            }
        }
        public long GetItemCount(E_ITEM_TYPE itemType)
        {
            int id = (int)itemType;
            return GetItemCount(id);
        }
        private void MakeItemDic(int id)
        {
            if (!itemDic.ContainsKey(id))
            {
                itemDic[id] = 0;
            }
        }
        public long GetItemCount(int id)
        {
            MakeItemDic(id);
            if ((int)E_ITEM_TYPE.CRYSTALS == id || (int)E_ITEM_TYPE.VALID_CRYSTALS == id)
            {
                MakeItemDic((int)E_ITEM_TYPE.CRYSTALS);
                MakeItemDic((int)E_ITEM_TYPE.VALID_CRYSTALS);
                return itemDic[(int)E_ITEM_TYPE.CRYSTALS] + itemDic[(int)E_ITEM_TYPE.VALID_CRYSTALS];
            }

            return itemDic[id];

        }
        //유저 아이템 내역
    }
    public class UserOptionData
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
    public class UserDataInfo
    {
        public UserOptionData userOption = new UserOptionData();
        public UserItemDatas userItemDatas = new UserItemDatas();
    }
}