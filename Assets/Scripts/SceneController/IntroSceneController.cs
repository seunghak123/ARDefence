using System;
using UnityEngine;
using UnityEngine.Video;

namespace Seunghak.SceneManager
{
    public class IntroSceneController : MonoBehaviour
    {
        [SerializeField] private VideoPlayer introPlayer;
        protected void Awake()
        {
            //로고 뜨고 뭐하고 
            introPlayer.Play();
        }
        bool isNext = false;
        protected void Update()
        {
            if (!isNext&& introPlayer.length<= introPlayer.clockTime)
            {
                isNext = true;
                ChangeScene();
            }
        }
        public void ChangeScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Title");
        }
    }
}
