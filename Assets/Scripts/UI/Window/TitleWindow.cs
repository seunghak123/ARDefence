using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Seunghak.UIManager
{
    using Seunghak.Common;
    using Seunghak.LoginSystem;

    public class TitleWindow : BaseUIWindow
    {
        [SerializeField] private TextMeshProUGUI versionText;
        [SerializeField] private Button enterButton;

        [SerializeField] private GameObject loginPanel;
        [SerializeField] private Button googleLoginButton;
        [SerializeField] private Button appleLoginButton;
        [SerializeField] private Button guestLoginButton;

        public void Update()
        {


        }

        public override void EnterWindow()
        {
            DeleteRegistedEvent();


            RegistEvent();
        }
        public override void ExitWindow()
        {
            DeleteRegistedEvent();
        }
        public override void RegistEvent()
        {
            guestLoginButton.onClick.AddListener(()=>LoginPlatform(E_LOGIN_TYPE.GUEST_LOGIN));
            appleLoginButton.onClick.AddListener(() => LoginPlatform(E_LOGIN_TYPE.APPLE_LOGIN));
            googleLoginButton.onClick.AddListener(() => LoginPlatform(E_LOGIN_TYPE.GOOGLE_LOGIN));
        }
        public override void DeleteRegistedEvent()
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

            UserDataManager.Instance.LoginPlatform();
        }
    }
}
