﻿using UnityEngine;

namespace Seunghak
{
    public class UnitySingleton<T> : MonoBehaviour where T : UnityEngine.Component
    {
        private static T s_Instance = null;
        private void Awake()
        {
            if (s_Instance == null)
            {
                s_Instance = this.gameObject.GetComponent<T>();
                if (s_Instance != null)
                {
                    DontDestroyOnLoad(s_Instance.gameObject);
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