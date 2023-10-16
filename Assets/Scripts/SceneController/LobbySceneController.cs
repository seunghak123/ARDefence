using System;
using UnityEngine;

namespace Seunghak.SceneManager
{
    using Seunghak.UIManager;
    public class LobbySceneController : SceneController
    {
        //메인 캐릭터 위치
        [SerializeField] private Transform mainCHPos;

        private GameObject mainCharacter;
        protected override void Awake()
        {
            base.Awake();

            InitSceneController();
        }
        //���� �� ��Ʈ�ѷ� �ʱ�ȭ �Լ�
        public override void InitSceneController() 
        {
            UIManager.Instance.OpenUI();
            UIManager.Instance.PushUI(UI_TYPE.LobbyWindow);
            //로그인 보상등등 이벤트 팝업 띄워주고, 

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
    }
}
