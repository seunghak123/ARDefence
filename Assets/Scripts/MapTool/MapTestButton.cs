using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapTestButton : MonoBehaviour
{
    [SerializeField] private Image unitImage;
    [SerializeField] private Text buttonText;

    public void SetInfo(Sprite infounitImage,string buttonString)
    {
        unitImage.sprite = infounitImage;
        buttonText.text = buttonString;
    }
}
