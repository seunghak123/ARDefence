using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace Seunghak.UIManager
{
    public class BaseAwnserPopup : BaseUI
    {
        [SerializeField] private Button okayButton;
        [SerializeField] private TextMeshProUGUI okayButtonText;

        [SerializeField] private Button noButton;
        [SerializeField] private TextMeshProUGUI noButtonText;

        
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI disciptionText;
        private Action okayAction;
        private Action noAction;

        public void OpenPopup(string okayButtonString,Action okayact,string noButtonString = null, Action noact = null)
        {
            if(noact == null)
            {
                noButton.gameObject.SetActive(false);


            }
            else
            {
                noButton.onClick.AddListener(() =>
                {
                    noact();
                });
                noButtonText.text = noButtonString;
            }
            okayButton.onClick.AddListener(() =>
            {
                if (okayact != null)
                {
                    okayact();
                }
            });
            okayButtonText.text = okayButtonString;

        }
    }
}