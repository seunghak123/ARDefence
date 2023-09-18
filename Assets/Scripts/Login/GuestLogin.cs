using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Seunghak.LoginSystem
{
    /// <summary>
    /// 테스트용 로그인이기 때문에, 실제 라이브 환경에서는 제거가 필요합니다.
    /// 게스트 로그인시 디바이스 GUID로 토큰값을 생성하기 때문에 디바이스가 동일하다면 동일한 계정으로 로그인 됩니다.
    /// </summary>
    public class GuestLogin : LoginInterface
    {
        public void InitLogin()
        {
        }
        public void PlatformLogin(Action successAction)
        {
            string userToken = Guid.NewGuid().ToString();


            if (successAction != null)
            {
                successAction();
            }
            //Token 값에 따라 
        }
    }
}
