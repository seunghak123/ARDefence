using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Animations;

public class EffectToolData : ScriptableObject
{
    public List<CommonEvent> listcharacterDatas = new List<CommonEvent>();
}
public class AnimationTool : EditorWindow
{
    private const string SAVE_PATH = "/Resources/effectdata/change";
    private static AnimationTool _animationTool;
    private GameObject _selectedObject;
    private Animator _selectedAnimator;

    private List<string> _stateList = new List<string>();
    private List<AnimatorState> _animatorStateList = new List<AnimatorState>();
    private List<string> _animationState = new List<string>();
    private AnimationClip _selectedAnimationClip;
    private float _clipPlayingTime;
    private int _selectAnimationStateNum = 0;
    private bool _isPlayingLoop = false;
    private static EffectToolData _characterFxData;
    private Vector2 _scrollPos;
    private Editor _gameObjectEditor;
    private float _updateTime = 0.0f;
    [MenuItem("Tools/AnimationTool", false, 2000)]
    private static void Open()
    {
        GC.Collect();
        if (!Application.isPlaying)
        {
            if (UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name != "AnimationTool")
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/Editor/AnimationTool.unity");
            }
        }
        if (_animationTool == null)
        {
            _animationTool = CreateInstance<AnimationTool>();
            _characterFxData = new EffectToolData();
        }

        _animationTool.ShowUtility();
    }
    private void OnDestroy()
    {
        if (!Application.isPlaying)
        {
            if (UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name == "AnimationTool")
            {
                UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/Editor/AnimationTool.unity");
            }
        }
    }
    public void OnSelectionChange()
    {
        if (Selection.activeGameObject == null)
        {
            return;
        }
        if(_selectedObject != Selection.activeGameObject)
        {
            _selectedAnimator = null;
        }

        _selectedAnimator = Selection.activeGameObject.GetComponentInChildren<Animator>();
        if (_selectedAnimator != null)
        {
            _selectedObject = _selectedAnimator.gameObject;
        }
        SelectAni();
    }
    public void OnGUI()
    {
        if (_selectedAnimator == null)
        {
            EditorGUILayout.HelpBox("Please select a GameObject in Animator", MessageType.Info);
            return;
        }
        if (_animatorStateList.Count <= 0)
        {
            return;
        }
        if (_characterFxData == null)
        {
            _characterFxData = ScriptableObject.CreateInstance<EffectToolData>();
        }
        EditorGUILayout.Space();
        if (GUILayout.Button("Load"))
        {
            string path = $"{Application.dataPath}{SAVE_PATH}{_selectedObject.transform.parent.gameObject.name}";

            _characterFxData = FileUtils.LoadFile<EffectToolData>(path);

            for (int i = 0; i < _characterFxData.listcharacterDatas.Count; i++)
            {
                _characterFxData.listcharacterDatas[i].objTarget = Resources.Load(("effects/CH/" + _characterFxData.listcharacterDatas[i].prefabName)) as GameObject;
            }
        }
        if (GUILayout.Button("Save"))
        {
            for(int i=0;i< _animatorStateList.Count; i++)
            {

            }
            if (_characterFxData.listcharacterDatas.Count <= 0)
            {
                return;
            }
            string path = $"{Application.dataPath}{SAVE_PATH}";

            string output = JsonConvert.SerializeObject(_characterFxData);
            //FileUtil.SetEventDataToClip(); 클립마다 이벤트 셀렉트해서 넣어줄것
            FileUtils.SaveFile(path, _selectedObject.transform.parent.gameObject.name,_characterFxData);
            AssetDatabase.Refresh();
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        _selectAnimationStateNum = EditorGUILayout.Popup("AnimationState", _selectAnimationStateNum, _animationState.ToArray());
        _selectedAnimationClip = _animatorStateList[_selectAnimationStateNum].motion as AnimationClip;
        if (_selectedAnimationClip != null)
        {
            _clipPlayingTime = EditorGUILayout.Slider("slider", _clipPlayingTime, 0f, _selectedAnimationClip.length);
        }
        if (GUILayout.Button("PlayingAnimation"))
        {
            _selectedAnimator.Play(_animationState[_selectAnimationStateNum]);

            _updateTime = _selectedAnimationClip.length;
        }

        if (GUILayout.Button("Add"))
        {
            CommonEvent cData = new CommonEvent();
            cData.beginTime = 0;
            cData.stateName = _animationState[_selectAnimationStateNum];
            _characterFxData.listcharacterDatas.Add(cData);
        }
        AnimationClip animClip = _animatorStateList[_selectAnimationStateNum].motion as AnimationClip;
        if (_selectedObject != null)
        {
            GUIStyle bgColor2 = new GUIStyle();
            if (_gameObjectEditor == null)
            {
                _gameObjectEditor = Editor.CreateEditor(_selectedObject);
            }
            if (_gameObjectEditor != null)
            {
                _gameObjectEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(256, 256), bgColor2);
            }
        }

        if (_updateTime > 0)
        {
            Focus();
            _updateTime -= 0.01f;
            if (_selectedObject != null)
            {
                GameObject child = _selectedObject.GetComponentInChildren<Animator>().gameObject;
                _selectedAnimationClip.SampleAnimation(child, _selectedAnimationClip.length - _updateTime);
                _gameObjectEditor.ReloadPreviewInstances();
            }
        }
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        GUIStyle styleBox = new GUIStyle("box");
        GUIStyle styleBoxInner = new GUIStyle("box");
        styleBox.padding = new RectOffset(10, 10, 20, 20);
        styleBoxInner.padding = new RectOffset(10, 10, 5, 5);

        styleBoxInner.normal.background = MakeTex(2, 2, Color.gray);

        EditorGUILayout.BeginVertical(styleBox);

        for (int i = 0; i < _characterFxData.listcharacterDatas.Count; i++)
        {
            if (!_characterFxData.listcharacterDatas[i].stateName.Equals(_animationState[_selectAnimationStateNum]))
            {
                continue;
            }
            EditorGUILayout.BeginVertical(styleBoxInner);

            GUIStyle txtSty = new GUIStyle();
            txtSty.normal.textColor = Color.yellow;
            txtSty.alignment = TextAnchor.MiddleCenter;
            txtSty.fontSize = 20;

            GUILayout.Label($"[ {_characterFxData.listcharacterDatas[i].stateName} ]", txtSty);

            if (GUILayout.Button("Delete"))
            {
                _characterFxData.listcharacterDatas.Remove(_characterFxData.listcharacterDatas[i]);
                break;
            }

            var cData = _characterFxData.listcharacterDatas[i];

            EditorGUI.BeginChangeCheck();
            AnimatorState animState = _animatorStateList.Find(find => find.name == _characterFxData.listcharacterDatas[i].stateName);
            AnimationClip animationClip = animState.motion as AnimationClip;
            _characterFxData.listcharacterDatas[i].beginTime = EditorGUILayout.IntSlider((int)Mathf.Round(_characterFxData.listcharacterDatas[i].beginTime * 30), 0, (int)animationClip.length * 30) / 30.0f;

            if (EditorGUI.EndChangeCheck())
            {
                _characterFxData.listcharacterDatas[i].beginTime = EditorGUILayout.IntSlider((int)Mathf.Round(_characterFxData.listcharacterDatas[i].beginTime * 30), 0, (int)animationClip.length * 30) / 30.0f;
            }
            //여기다 타입 선택에 따라 UI가 다르게 변경되도록 작업
            //settings.ProfilerAlignment = (PinAlignment)EditorGUILayout.EnumPopup((ProfilerAlignment)settings.ProfilerAlignment);
            E_ANIMATION_EVENT selectedenum = _characterFxData.listcharacterDatas[i].animationType;

            GUIStyle bgColor = new GUIStyle();
            bgColor.normal.background = EditorGUIUtility.whiteTexture;

            selectedenum = (E_ANIMATION_EVENT)EditorGUILayout.EnumPopup(selectedenum);
            if (EditorGUI.EndChangeCheck())
            {
                if (_characterFxData.listcharacterDatas[i].animationType != selectedenum)
                {
                    _characterFxData.listcharacterDatas[i].eventFunctionNameString = null;
                    _characterFxData.listcharacterDatas[i].objTarget = null;
                    _characterFxData.listcharacterDatas[i].prefabName = null;
                    _characterFxData.listcharacterDatas[i].arguString = null;
                    _characterFxData.listcharacterDatas[i].arguInt = 0;
                    _characterFxData.listcharacterDatas[i].arguFloat = 0;
                }
                _characterFxData.listcharacterDatas[i].animationType = selectedenum;
            }
            switch (selectedenum)
            {
                case E_ANIMATION_EVENT.CREATE_OBJECT:
                    _characterFxData.listcharacterDatas[i].objTarget = EditorGUILayout.ObjectField("TargetObject", _characterFxData.listcharacterDatas[i].objTarget, typeof(GameObject), false) as GameObject;
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (_characterFxData.listcharacterDatas[i].objTarget != null)
                        {
                            _characterFxData.listcharacterDatas[i].prefabName = _characterFxData.listcharacterDatas[i].objTarget.name;
                        }
                    }
                    break;
                case E_ANIMATION_EVENT.CREATE_PARTICLE:
                    _characterFxData.listcharacterDatas[i].eventFunctionNameString = EditorGUILayout.TextField("TargetFunctionEvent", _characterFxData.listcharacterDatas[i].eventFunctionNameString);
                    EditorGUILayout.Space();
                    _characterFxData.listcharacterDatas[i].arguString = EditorGUILayout.TextField("ArguString", _characterFxData.listcharacterDatas[i].arguString);
                    _characterFxData.listcharacterDatas[i].arguInt = EditorGUILayout.IntField("ArguInt", _characterFxData.listcharacterDatas[i].arguInt);
                    _characterFxData.listcharacterDatas[i].arguFloat = EditorGUILayout.FloatField("ArguFloat", _characterFxData.listcharacterDatas[i].arguFloat);
                    break;
                case E_ANIMATION_EVENT.PLAY_SOUND:
                    _characterFxData.listcharacterDatas[i].eventFunctionNameString = $"PlaySound";
                    _characterFxData.listcharacterDatas[i].arguString = EditorGUILayout.TextField("ArguString", _characterFxData.listcharacterDatas[i].arguString);
                    break;

                case E_ANIMATION_EVENT.CAMERA_ACATION:
                    _characterFxData.listcharacterDatas[i].eventFunctionNameString = $"PlayEffect";
                    _characterFxData.listcharacterDatas[i].objTarget = EditorGUILayout.ObjectField("TargetObject", _characterFxData.listcharacterDatas[i].objTarget, typeof(GameObject), false) as GameObject;
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (_characterFxData.listcharacterDatas[i].objTarget != null)
                        {
                            _characterFxData.listcharacterDatas[i].prefabName = _characterFxData.listcharacterDatas[i].objTarget.name;
                        }
                    }
                    break;
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndScrollView();
    }
    public void SelectAni()
    {
        _stateList.Clear();
        _animationState.Clear();
        _animatorStateList.Clear();

        _selectedAnimationClip = null;
        if (_selectedAnimator == null)
        {
            return;
        }
        AnimatorController animationController = _selectedAnimator.runtimeAnimatorController as AnimatorController;
        if (animationController == null)
        {
            return;
        }
        AnimatorStateMachine animationStateMachine = animationController.layers[0].stateMachine;

        for (int idx = 0; idx < animationStateMachine.states.Length; ++idx)
        {
            if (animationStateMachine.states[idx].state.motion == null)
            {
                continue;
            }
            _animationState.Add(animationStateMachine.states[idx].state.name);
            _animatorStateList.Add(animationStateMachine.states[idx].state);
        }
    }
    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}

