using System;
using UnityEngine;
namespace Seunghak.SceneManager
{
    using Seunghak.UIManager;
    public class BattleSceneController : SceneController
    {
        protected override void Awake()
        {
            base.Awake();
        }
        //���� �� ��Ʈ�ѷ� �ʱ�ȭ �Լ�
        public override void InitSceneController() 
        {
            UIManager.Instance.CloseUI();

            //테스트 코드 SceneController에 데이터 넘기고 그걸 인게임 매니저에 전달 필요
            IngameManager.currentManager.CreateGame(0);
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
