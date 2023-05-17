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
        public void EnterWindow()
        {
            //Stack에 EnterWindow 사용하는 함수
        }

        public void ExitWindow()
        {
            //Window 종료할 때 사용하는 함수
        }

        public void RestoreWindow()
        {
            //다른 Window에서 해당 Window로 되돌아 올때 실행 시켜주는 함수
        }

        public void StartWindow()
        {
            //window 시작할때 실행 시켜주는 함수
        }
    }
}