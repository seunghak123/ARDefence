using Seunghak.Common;
using UnityEngine;

namespace Seunghak
{
    public class UnitySingleton<T> : MonoBehaviour where T : UnityEngine.Component
    {
        private static T s_Instance = null;
        protected virtual void InitSingleton() { }
        private void Awake()
        {
            if (s_Instance == null)
            {
                s_Instance = this.gameObject.GetComponent<T>();
                if (s_Instance != null)
                {
                    DontDestroyOnLoad(s_Instance.gameObject);
                    InitSingleton();
                    GameObject parentObj = GameObject.Find("Managers");
                    if (parentObj != null)
                    {
                        this.transform.parent = parentObj.transform;
                    }
                }
            }
        }
        private void OnDestroy()
        {
            s_Instance = null;
        }

        public static T Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    string objectName = (typeof(T)).ToString();

                    s_Instance = new GameObject(objectName).AddComponent<T>();
                    DontDestroyOnLoad(s_Instance.gameObject);
                }

                return s_Instance;
            }
        }

    }

}