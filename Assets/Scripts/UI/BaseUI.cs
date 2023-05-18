using UnityEngine;

namespace Seunghak.UIManager
{
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
            //Stack에 EnterWindow 사용하는 함수
        }

        public virtual void ExitWindow()
        {
            //Window 종료할 때 사용하는 함수
        }

        public virtual void RestoreWindow()
        {
            //다른 Window에서 해당 Window로 되돌아 올때 실행 시켜주는 함수
        }

        public virtual void StartWindow()
        {
            //window 시작할때 실행 시켜주는 함수
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