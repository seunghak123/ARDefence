using System;

namespace Seunghak.LoginSystem
{
    public interface LoginInterface
    {
        void InitLogin();
        void PlatformLogin(Action successResultAct);
    }
}
