using System;
using UnityEngine;

namespace Seunghak.SceneManager
{
    using Seunghak.Common;
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

            //캐릭터 생성
            CreateLobbyObject();

        }
        private void CreateLobbyObject()
        {
            if (mainCharacter != null)
            {
                Destroy(mainCharacter);
            }

            //테스트용 차후 유저 DB에따라서 생성되는 캐릭터 달리 줄것
            mainCharacter = GameResourceManager.Instance.SpawnObject("DogKnight");
            if (mainCharacter != null)
            {
                mainCharacter.transform.parent = mainCHPos;
                mainCharacter.transform.localPosition = Vector3.zero;
                mainCharacter.transform.localScale = Vector3.one;
                mainCharacter.transform.localEulerAngles = Vector3.zero;
                UnitController controller = mainCharacter.GetComponent<UnitController>();
                if(controller!=null)
                {
                    controller.CreateLobbyAI();
                }
            }


            //아래에 배경 오브젝트 생성하는 코드
        }
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
