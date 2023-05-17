using System;
using UnityEngine;

namespace Seunghak.SceneManager
{
    public abstract class SceneController : MonoBehaviour
    {
        protected void Awake()
        {
            RegistSceneController();
        }
        //현재 씬 컨트롤러 초기화 함수
        public void InitSceneController() { }
        //씬 로드 시 씬 매니저에 현재 컨트롤러 등록하는 함수
        public void RegistSceneController()
        {
            SceneManager.Instance.RegistSceneController(this);
        }
        public static void RegistSceneStepAction(E_SCENESTEP_TYPE actionType, Action playAction)
        {
            SceneManager.Instance.AddStepAction(actionType, playAction);
        }
        public static void ChangeScene(E_SCENE_TYPE nextScene)
        {
            SceneManager.Instance.ChangeScene(nextScene);
        }
    }
}
