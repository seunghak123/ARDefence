using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Seunghak.Common;
public class WealthUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI wealthCountText;
    [SerializeField] private Image wealthImage;

    public void SetWealth(string imageName, long wealthCount)
    {
        //Item가져다가 text설정하고, 샵으로 가는 것도 on off하도록 하게한다
        wealthCountText.text = wealthCount.ToString();
        wealthImage.sprite = SpriteManager.Instance.LoadSprite(imageName);
    }
}
