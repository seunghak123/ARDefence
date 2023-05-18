using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Seunghak.UIManager
{
    using Seunghak.SceneManager;
    public class TitleBackGroundWindow : BaseUIWindow
    {
        [SerializeField] private TextMeshProUGUI versionText;
        [SerializeField] private Button enterButton;
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
            enterButton.onClick.AddListener(EnterLobby);
        }
        public override void DeleteRegistedEvent()
        {
            enterButton.onClick.RemoveListener(EnterLobby);
        }
        private void EnterLobby()
        {
            SceneManager.Instance.ChangeScene(E_SCENE_TYPE.LOBBY);
        }
    }
}
