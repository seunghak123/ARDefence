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

    public void SetWealth(string imageName, int wealthCount)
    {
        wealthCountText.text = wealthCount.ToString();
        wealthImage.sprite = SpriteManager.Instance.LoadSprite(imageName);
    }
}
