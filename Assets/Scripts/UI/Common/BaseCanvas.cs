using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seunghak.UIManager
{
    public class BaseCanvas : UnitySingleton<BaseCanvas>
    {
        [SerializeField] public Transform windowUIParent;
        [SerializeField] public Transform popUpUIParent;
        [SerializeField] public Transform UtilUIParent;

    }
}
