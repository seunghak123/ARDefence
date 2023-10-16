using System;
using UnityEngine;

namespace Seunghak.SceneManager
{
    public abstract class SceneController : MonoBehaviour
    {
        protected virtual void Awake()
        {
            RegistSceneController();
        }
        //현재 씬 컨트롤러 초기화 함수
        public virtual void InitSceneController() { }
        //씬 로드 시 씬 매니저에 현재 컨트롤러 등록하는 함수
        public virtual void RegistSceneController()
        {
            SceneManager.Instance.RegistSceneController(this);
        }
        public virtual void RegistSceneStepAction(E_SCENESTEP_TYPE actionType, Action playAction)
        {
            SceneManager.Instance.AddStepAction(actionType, playAction);
        }
        public void ChangeScene(E_SCENE_TYPE nextScene)
        {
            SceneManager.Instance.ChangeScene(nextScene);
        }
    }
}
