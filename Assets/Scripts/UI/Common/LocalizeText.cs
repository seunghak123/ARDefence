using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LocalizeText : MonoBehaviour
{
    [SerializeField] private string convertStringEnum;
    private TextMeshProUGUI currentText;
    void Start()
    {
        currentText = GetComponent<TextMeshProUGUI>();
        if(currentText == null)
        {
            Destroy(this);
            return;
        }

        SetStringKey(convertStringEnum);
    }
    public void SetStringKey(string stringEnum)
    {
        string inputtext = "";
        //데이터 파일에서 해당 키값 찾아서 텍스트에 입력
        currentText.text = inputtext;
    }
    public void SetString(string stringEnum, params string[] inputdatas)
    {
        string inputtext = "";
        inputtext = string.Format(inputtext, inputdatas);
        currentText.text = inputtext;
    }
}
