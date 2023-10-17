using Seunghak.UIManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ShopScrollItem : MonoBehaviour , OHScrollView.IInfiniteScrollSetup<ShopItemScrollData>
{
    [SerializeField] private TextMeshProUGUI shopItemlbl;

    public void OnPostSetupItems()
    {

    }

    public void OnUpdateItem( GameObject obj, ShopItemScrollData infos)
    {
        shopItemlbl.text = infos.eventText;
    }
}
