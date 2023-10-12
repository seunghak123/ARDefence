using System;
using UnityEngine;

namespace Seunghak.SceneManager
{
    using Seunghak.UIManager;
    public class TitleSceneController : SceneController
    {
        protected override void Awake()
        {
            base.Awake();
        }
        //���� �� ��Ʈ�ѷ� �ʱ�ȭ �Լ�
        public override void InitSceneController() 
        {
            UIManager.Instance.OpenUI();
        }
        //�� �ε� �� �� �Ŵ����� ���� ��Ʈ�ѷ� ����ϴ� �Լ�
        public override void RegistSceneController()
        {
            base.RegistSceneController();
        }
        public override void RegistSceneStepAction(E_SCENESTEP_TYPE actionType, Action playAction)
        {
            base.RegistSceneStepAction(actionType, playAction);
        }
        public override void ChangeScene(E_SCENE_TYPE nextScene)
        {
            base.ChangeScene(nextScene);
        }
    }
}
