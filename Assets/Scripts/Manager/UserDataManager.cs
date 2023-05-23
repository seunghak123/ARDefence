using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seunghak.Common
{
    public class UserDataManager : UnitySingleton<UserDataManager>
    {
        private string userIDToken = "";
        public string UserIDToken
        {
            get
            {
                return userIDToken;
            }
        }
        

    }
}