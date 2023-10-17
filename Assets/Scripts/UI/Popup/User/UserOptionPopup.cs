using Seunghak.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Seunghak.UIManager
{
    public class UserOptionPopup : BaseUIPopup
    {
        [Header("Sound")]
        [SerializeField] private Slider masterVolumnSlider;
        [SerializeField] private TextMeshProUGUI masterVolumnlbl;

        [SerializeField] private Slider soundVolumnSlider;
        [SerializeField] private TextMeshProUGUI soundVolumnlbl;
        [SerializeField] private Slider fbxVolumnSlider;
        [SerializeField] private TextMeshProUGUI fbxVolumnlbl;

        [SerializeField] private Toggle isSoundMute;
        [Header("Lang")]
        [SerializeField] private TMP_Dropdown langTypeDropdown;


        private UserOptionData userOption;
        public override void EnterWindow()
        {
            base.EnterWindow();

            InitOptionPopupUI();
        }
        private void InitOptionPopupUI()
        {
            userOption = UserDataManager.Instance.GetUserOptionData();

            ChangeUI();
            InitUIEvent();
        }
        private void InitUIEvent()
        {
            masterVolumnSlider.onValueChanged.AddListener( (float change)=> {
                userOption.MasterVolume = (int)change;
                ChangeUI();
            });
            soundVolumnSlider.onValueChanged.AddListener((float change) => { 
                userOption.SoundVolume = (int)change;
                ChangeUI();
            });
            fbxVolumnSlider.onValueChanged.AddListener((float change) => { 
                userOption.FBXVolume = (int)change;
                ChangeUI();
            });
            langTypeDropdown.onValueChanged.AddListener((int changeType) =>
            {
                userOption.UserLanguageType = (E_LANGUAGE_TYPE)changeType;
                ChangeUI();
            });
            isSoundMute.onValueChanged.AddListener((bool isChangeMute) =>
            {
                userOption.IsMute = isChangeMute;
                //여기엔 차후 0 으로 변경해주는게 필요
                ChangeUI();
            });
        }
        private void ChangeUI()
        {
            masterVolumnlbl.text = userOption.MasterVolume.ToString();
            soundVolumnlbl.text = userOption.SoundVolume.ToString();
            fbxVolumnlbl.text = userOption.FBXVolume.ToString();

            masterVolumnSlider.value = userOption.MasterVolume;
            soundVolumnSlider.value = userOption.SoundVolume;
            fbxVolumnSlider.value = userOption.FBXVolume;

            isSoundMute.isOn = userOption.IsMute;

            langTypeDropdown.value = (int)userOption.UserLanguageType;
        }
        public override void ExitWindow()
        {
            base.ExitWindow();
        }

        public override void RestoreWindow()
        {
            base.RestoreWindow();
        }

        public override void StartWindow()
        {
            base.StartWindow();
        }
        public override void RegistEvent()
        {
            base.RegistEvent();
        }
        public override void DeleteRegistedEvent()
        {
            base.DeleteRegistedEvent();
        }
    }
}