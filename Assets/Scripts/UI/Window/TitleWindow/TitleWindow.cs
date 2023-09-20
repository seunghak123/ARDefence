using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Seunghak.UIManager
{
    using Seunghak.Common;
    using Seunghak.LoginSystem;

    public class TitleWindow : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI versionText;
        [SerializeField] private GameObject loginPanel;
        [SerializeField] private Button googleLoginButton;
        [SerializeField] private Button appleLoginButton;
        [SerializeField] private Button guestLoginButton;
        [SerializeField] public DownloadBundlePopup downloadPopup;
        [SerializeField] public BaseAwnserPopup baseAwnserPopup;
        private void Awake()
        {
            EnterWindow();
        }

        public void InitTitleWindow()
        {
            if (loginPanel != null)
            {
                loginPanel.SetActive(false);
            }

            versionText.text = Application.version;
        }
        public void SetUserLogin()
        {
            if (loginPanel != null)
            {
                loginPanel.SetActive(true);
            } 
        }

        public void EnterWindow()
        {
            DeleteRegistedEvent();


            RegistEvent();

            InitTitleWindow();
        }
        public void ExitWindow()
        {
            DeleteRegistedEvent();
        }
        public void RegistEvent()
        {
            guestLoginButton.onClick.AddListener(()=>LoginPlatform(E_LOGIN_TYPE.GUEST_LOGIN));
            appleLoginButton.onClick.AddListener(() => LoginPlatform(E_LOGIN_TYPE.APPLE_LOGIN));
            googleLoginButton.onClick.AddListener(() => LoginPlatform(E_LOGIN_TYPE.GOOGLE_LOGIN));
        }
        public void DeleteRegistedEvent()
        {
            guestLoginButton.onClick.RemoveAllListeners();
            appleLoginButton.onClick.RemoveAllListeners();
            googleLoginButton.onClick.RemoveAllListeners();
        }
        private void EnterLobby()
        {

        }
        private void LoginPlatform(E_LOGIN_TYPE loginType)
        {
            UserDataManager.Instance.SetLoginInfo(loginType);

            UserDataManager.Instance.LoginPlatform(()=>
            {
                ApplicationManager.Instance.UserLoginSuccess();

                InitTitleWindow();
            }
            );
        }
    }
}
