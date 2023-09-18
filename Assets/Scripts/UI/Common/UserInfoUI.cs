using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Seunghak.Common;
using TMPro;
using Seunghak.UIManager;

public class UserInfoUI : MonoBehaviour
{
    [SerializeField] private Image userIconImage;
    [SerializeField] private Button userInfoPopupButton;
    [SerializeField] private TextMeshProUGUI userNameText;
    [SerializeField] private TextMeshProUGUI userLvText;

    private void OnEnable()
    {
        userInfoPopupButton.onClick.AddListener(OpenUIPopup);
    }
    private void OnDisable()
    {
        userInfoPopupButton.onClick.RemoveListener(OpenUIPopup);      
    }
    private void OpenUIPopup()
    {
       // UIManager.Instance.PushUI(UI_TYPE.BasePopupWindow);
    }
}
