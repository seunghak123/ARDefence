using UnityEngine;

namespace Seunghak.UIManager
{
    public enum UI_TYPE
    {
        //1~100까진 기본 Window
        LobbyWindow,
        TitleWindow,

    }
    public interface IBaseUIController
    {
        void EnterWindow();
        void StartWindow();
        void ExitWindow();
        void RestoreWindow();
    }

    public class BaseUI : MonoBehaviour, IBaseUIController
    {
        public virtual void EnterWindow()
        {
            this.gameObject.SetActive(true);
        }

        public virtual void ExitWindow()
        {
            this.gameObject.SetActive(false);
        }

        public virtual void RestoreWindow()
        {
            this.gameObject.SetActive(true);
        }

        public virtual void StartWindow()
        {

        }
        public virtual void RegistEvent()
        {
            //버튼 이벤트 등을 등록해주는 함수
        }
        public virtual void DeleteRegistedEvent()
        {
            //버튼 이벤트를 제거해주는 함수
        }
    }
}