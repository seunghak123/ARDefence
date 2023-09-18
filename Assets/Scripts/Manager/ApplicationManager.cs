﻿using Seunghak.UIManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID
using UnityEngine.Android;
#else
using UnityEngine.iOS;
#endif
namespace Seunghak.Common
{
    public class ApplicationManager : UnitySingleton<ApplicationManager>
    {
        [SerializeField] public static string cdnAddressPath = "https://d2fvmix8egxgsw.cloudfront.net/ARDefenceBundle/";
        [SerializeField] private bool isUseBundle;
        [Header("Init UI")]
        [SerializeField] TitleWindow usertitleWindow;
        [SerializeField] DownloadBundlePopup userDownloadBundlePopup;
        //[SerializeField] private 
        private E_APPLICATION_STATE applicationState = E_APPLICATION_STATE.APPLICATION_START;
        private void InitApplication()
        {
            if (usertitleWindow == null)
            {
                usertitleWindow = GameObject.Find("TitleWindow").GetComponent<TitleWindow>();
            }
            if (userDownloadBundlePopup == null)
            {
                userDownloadBundlePopup = GameObject.Find("DownloadBundlePopup").GetComponent<DownloadBundlePopup>();
            }

            if (usertitleWindow != null)
            {
                usertitleWindow.InitTitleWindow();
            }
            if (userDownloadBundlePopup != null)
            {
                userDownloadBundlePopup.gameObject.SetActive(false);
            }

            ApplicationWork(E_APPLICATION_STATE.APPLICATION_START);
        }
        private void Start()
        {
            InitApplication();
        }
        private void ApplicationWork(E_APPLICATION_STATE nextType)
        {
            applicationState = nextType;
            switch (applicationState)
            {
                case E_APPLICATION_STATE.APPLICATION_START:
                    MoveNextState(E_APPLICATION_STATE.REQUEST_PERMISSION);
                    break;
                case E_APPLICATION_STATE.REQUEST_PERMISSION:
                    StartCoroutine(RequestPermission());
                    break;
                case E_APPLICATION_STATE.APPLICATION_UPDATE:
                    StartCoroutine(ApplicationUpdate());
                    break;
                case E_APPLICATION_STATE.USER_LOGIN:
                    StartCoroutine(UserLogin());
                    break;
                case E_APPLICATION_STATE.BUNDLE_UPDATE:
                    StartCoroutine(BundleUpdate());
                    break;
                case E_APPLICATION_STATE.INAPP_UPDATE:

                    break;
                case E_APPLICATION_STATE.TITLE:
                    break;
            }

        }
        private void MoveNextState(E_APPLICATION_STATE nextType)
        {
            //State사이 시간 부여 등등 action들을 등록하고 대기하는 시간을 갖도록 변경
            ApplicationWork(nextType);

        }
        private IEnumerator ApplicationUpdate()
        {
            MoveNextState(E_APPLICATION_STATE.USER_LOGIN);
            yield break;
        }
        public void UserLoginSuccess()
        {
            MoveNextState(E_APPLICATION_STATE.BUNDLE_UPDATE);
        }
        private IEnumerator UserLogin()
        {
            usertitleWindow.SetUserLogin();
            yield break;
        }
        private IEnumerator BundleUpdate()
        {
            string jsonPath;//= $"{cdnAddressPath}/Version/{Application.version}{FileUtils.VERSION_INFO_FILE_NAME}";
            //우선 CDN 테스트가 완료되었기 떄문에, 임시적으로 로컬에서 다운로드
            jsonPath = $"C:/Users/dhtmd/Desktop/TestLocalStorage/Version/{Application.version}/{FileUtils.VERSION_INFO_FILE_NAME}";
            UpdateVersionInfo userData = new UpdateVersionInfo();

            IEnumerator jsonCoroutine = FileUtils.RequestTextFile<UpdateVersionInfo>(jsonPath);
            while (jsonCoroutine.MoveNext())
            {
                object current = jsonCoroutine.Current;
                string currentText = current.ToString();
                if (current is UpdateVersionInfo bundleData)
                {
                    userData = bundleData;
                }
                yield return null;
            }
            //CdnPath강제로 가라 처리
            userData.cdnAddressInfoPath = "C://Users/dhtmd/Desktop/TestLocalStorage/Android/09111200";
            string preCheckDownloadFile = $"{Application.persistentDataPath}/{ FileUtils.BUNDLE_LIST_FILE_NAME}";

            BundleListsDic preLoadDic = FileUtils.LoadFile<BundleListsDic>(preCheckDownloadFile);
            //만약 해당 파일이 없을 경우엔 모두 받는것으로 판정
            if (preLoadDic == null)
            {
                preLoadDic = new BundleListsDic();
            }

            BundleListsDic compareLoadDic = new BundleListsDic();

            string downloadCheckDownloadPath = $"{userData.cdnAddressInfoPath}/{ FileUtils.BUNDLE_LIST_FILE_NAME}";
            IEnumerator checkDownloadCoroutine = FileUtils.RequestTextFile<BundleListsDic>(downloadCheckDownloadPath);
            while (checkDownloadCoroutine.MoveNext())
            {
                object current = checkDownloadCoroutine.Current;
                if (current is BundleListsDic bundleData)
                {
                    compareLoadDic = bundleData;
                }
                yield return null;
            }

            BundleListsDic finalDownloadDic = FileUtils.CompareDicData(preLoadDic, compareLoadDic);

            long totalDownloadSize = finalDownloadDic.GetTotalSize();

            if (totalDownloadSize <= 0)
            {
                //다운로드 받을 것이 없을경우 바로 인앱 업데이트로 이동
                MoveNextState(E_APPLICATION_STATE.INAPP_UPDATE);
                yield break;
            }
            float waitDownloadTime = 0.0f;

            if (userDownloadBundlePopup == null)
            {
                yield break;
            }
            userDownloadBundlePopup.gameObject.SetActive(true);
            //popupUI.
            bool isAction = false;
            bool isDownload = false;
            userDownloadBundlePopup.SetButtonAction(
                () =>
            {
                isAction = true;
                isDownload = true;
            },
            () =>
            {
                isAction = true;
            },
            FileUtils.GetFileSizeString(totalDownloadSize));
            while (!isAction)
            {
                yield return WaitTimeManager.WaitForEndFrame();
            }
            if (isDownload)
            {
                //다운로드 경로 강제로 가라 처리
                AssetBundleManager.BaseDownloadingURL = "C:/Users/dhtmd/Desktop/TestLocalStorage/Android/09111200";
                //if (!AssetBundleManager.SimulateAssetBundleInEditor)
                {
                    yield return AssetBundleManager.Initialize().IsDone();
                }
                while (AssetBundleManager.inProgressOperations.Count > 0)
                {
                    yield return WaitTimeManager.WaitForEndFrame();
                }

                AssetBundleManager.Instance.InitAssetBundleManager(finalDownloadDic);
            }
            while (AssetBundleManager.inProgressOperations.Count > 0)
            {
                waitDownloadTime += Time.deltaTime;

                //만약 waitDownloadTime가 일정 시간을 지나면
                //어플리케이션용 미니게임 또는 서브게임이 출력 또는 가라 게임이 출력
                yield return WaitTimeManager.WaitForEndFrame();
            }

            FileUtils.SaveFile<BundleListsDic>(Application.persistentDataPath, FileUtils.BUNDLE_LIST_FILE_NAME, compareLoadDic);
           
            StartCoroutine( GameResourceManager.Instance.SetDownloadDatas());

            while (!GameResourceManager.Instance.isReady)
            {
                yield return WaitTimeManager.WaitForEndFrame();
            }
            Object a = GameResourceManager.Instance.LoadObject("jshop");

            MoveNextState(E_APPLICATION_STATE.INAPP_UPDATE);
            yield return null;
        }
        //private void 
        private IEnumerator RequestPermission()
        {
#if UNITY_EDITOR

            //iOS의 경우 퍼미션
#elif UNITY_ANDROID && !UNITY_EDITOR
            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            {
                Permission.RequestUserPermission(Permission.ExternalStorageWrite);
            }
#elif UNITY_iOS && !UNITY_EDITOR
            

#endif 
            MoveNextState(E_APPLICATION_STATE.APPLICATION_UPDATE);
            yield return null;
        }
        private IEnumerator InAppUpdate()
        {
            MoveNextState(E_APPLICATION_STATE.TITLE);
            yield break;
        }
        //어플리케이션 업데이트 로직
        //어플리케이션 권한 로직
        //어플리케이션 각종 권한 껏다 켯다 하는 로직
    }
    [SerializeField]
    public class UpdateVersionInfo
    {
        public string cdnAddressInfoPath;
        //최신 버젼
        public string currentVersion;
        //특정 버젼 이하 강제 업데이트
        public string forcedUpdateVersion;
    }
    [SerializeField]
    public class CDNUpdateAddressInfo
    {
        //업데이트 CDN 경로
        public string updateCDNPath;
        //CDN Path는 cdnAddressPath + 플랫폼타입 + updateCDNPath
    }
}
