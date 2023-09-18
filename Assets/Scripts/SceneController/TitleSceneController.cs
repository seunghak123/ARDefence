using System;
using UnityEngine;

namespace Seunghak.SceneManager
{
    public class TitleSceneController : SceneController
    {
        protected void Awake()
        {
            RegistSceneController();
        }
        //���� �� ��Ʈ�ѷ� �ʱ�ȭ �Լ�
        public override void InitSceneController() { }
        //�� �ε� �� �� �Ŵ����� ���� ��Ʈ�ѷ� ����ϴ� �Լ�
        public override void RegistSceneController()
        {
            SceneManager.Instance.RegistSceneController(this);
        }
        public override void RegistSceneStepAction(E_SCENESTEP_TYPE actionType, Action playAction)
        {
            SceneManager.Instance.AddStepAction(actionType, playAction);
        }
        public override void ChangeScene(E_SCENE_TYPE nextScene)
        {
            SceneManager.Instance.ChangeScene(nextScene);
        }
    }
}
