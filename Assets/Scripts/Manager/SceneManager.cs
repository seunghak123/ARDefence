using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SceneManager에 두는게 아니라 Util쪽에 enum으로 씬 종류 나열
public enum E_SCENE_TYPE
{
    INTRO,
    TITLE,
    LOBBY,
    INGAME,
    FIELD,
    END,
}
public enum E_SCENESTEP_TYPE
{
    NONE,
    EXITSTEP,
    LOADSTEP,
    OPENSTEP,
    END,
}

namespace Seunghak.SceneManager
{
    public class SceneManager : UnitySingleton<SceneManager>
    {
        // ------------------------테스트-------------------------------- 
        [SerializeField] private E_SCENE_TYPE testSceneType = E_SCENE_TYPE.INTRO;
        [SerializeField] private bool isWatingSuccessSequence = false;
        // ------------------------테스트-------------------------------- 
        #region Property
        private Dictionary<E_SCENESTEP_TYPE, Action> _addactionList = new Dictionary<E_SCENESTEP_TYPE, Action>();
        private Dictionary<E_SCENESTEP_TYPE, Action> _baseactionList = new Dictionary<E_SCENESTEP_TYPE, Action>();
        private E_SCENESTEP_TYPE _currentStepType = E_SCENESTEP_TYPE.NONE;
        private E_SCENE_TYPE _currentSceneType = E_SCENE_TYPE.INTRO;
        private E_SCENE_TYPE _nextSceneType = E_SCENE_TYPE.INTRO;
        private SceneController _currentSceneController = null;
        #endregion Property

        protected override void InitSingleton()
        {
            InitSceneManager();
        }
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                ChangeScene(testSceneType);
            }
        }
        // ------------------------테스트--------------------------------//
        #region UtilityLogic
        public void RegistSceneController(SceneController addedSceneController)
        {
            _currentSceneController = addedSceneController;
        }
        public void AddStepAction(E_SCENESTEP_TYPE addStepType, Action addAction)
        {
            if (_addactionList.ContainsKey(addStepType))
            {
                _addactionList[addStepType] = addAction;
            }
            else
            {
                _addactionList.Add(addStepType, addAction);
            }
        }
        //공용으로 사용할 씬 나갈때 스텝 
        public void ChangeScene(E_SCENE_TYPE nextScene)
        {
            _nextSceneType = nextScene;
            NextStep(E_SCENESTEP_TYPE.EXITSTEP);
        }
        //베이스가 되는 씬 기반으로 돌아가는 함수
        public static void GoToBase()
        {
            //로비 씬 로드 하고 기본 베이스 화면으로 돌아가는 작업
        }
        #endregion UtilityLogic
        #region BasicLogic
        private void InitSceneManager()
        {
            _baseactionList.Add(E_SCENESTEP_TYPE.EXITSTEP, BaseExitStep);
            _baseactionList.Add(E_SCENESTEP_TYPE.LOADSTEP, BaseLoadStep);
            _baseactionList.Add(E_SCENESTEP_TYPE.OPENSTEP, BaseOpenStep);
        }
        private void BaseExitStep()
        {
            _currentSceneController = null;
            ExecuteAddStepAcion();
            NextStep(E_SCENESTEP_TYPE.LOADSTEP);
        }
        private void BaseLoadStep()
        {
            StartCoroutine(LoadData());
        }
        private void BaseOpenStep()
        {
            ExecuteAddStepAcion();
            NextStep(E_SCENESTEP_TYPE.END);
        }
        private WaitForEndOfFrame waitframe = new WaitForEndOfFrame();
        private IEnumerator LoadData()
        {
            ExecuteAddStepAcion();
            AsyncOperation taskLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(_nextSceneType.ToString());
            while (!taskLoad.isDone || isWatingSuccessSequence)
            {
                //m_TaskLoad.progress progress 표시 또는
                yield return waitframe;
            }
            while (_currentSceneController == null)
            {
                yield return waitframe;
            }
            //여기에 데이터 로드 할떄까지 대기
            //씬 로드시 완료될떄까지 대기탄다
            NextStep(E_SCENESTEP_TYPE.OPENSTEP);
            yield break;
        }
        private void ExecuteStepAction()
        {
            if (_baseactionList.ContainsKey(_currentStepType))
            {
                _baseactionList[_currentStepType]();
            }
        }
        private void ExecuteAddStepAcion()
        {
            if (_addactionList.ContainsKey(_currentStepType))
            {
                _addactionList[_currentStepType]();

                _addactionList.Remove(_currentStepType);
            }
        }
        private void NextStep(E_SCENESTEP_TYPE nextStepType)
        {
            if (nextStepType == E_SCENESTEP_TYPE.END)
            {
                //모든 씬 로드 액션 끝
                _currentStepType = E_SCENESTEP_TYPE.NONE;
                _currentSceneType = _nextSceneType;
                //로딩 화면 종료
                _currentSceneController.InitSceneController();
                //각 씬 컨트롤러 여기서 초기화 함수 호출 씬 컨트롤러는 BaseSceneController 상속 받는다.
                // 각 씬에서 반드시 해줘야 할 행동 리스트들을 나열해놓은 인터페이스를 상속받는다
                //인터페이스 상속 받은것을 찾아서 실행만 시켜주면 된다.
                return;
            }
            _currentStepType = nextStepType;
            ExecuteStepAction();
        }
        #endregion BasicLogic
    }
}
