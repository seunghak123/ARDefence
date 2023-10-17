using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Seunghak.UIManager
{
    public class ShopWindow : BaseUIWindow
    {
        [SerializeField] private OHScrollView shopitemScroll;
        private List<ShopItemScrollData> shopItemLists = new List<ShopItemScrollData>();
        //타입에 따라 분리,
        //
        public override void EnterWindow()
        {
            base.EnterWindow();
            shopItemLists.Clear();
            for (int i = 0; i < 30; i++)
            {
                ShopItemScrollData scrolData = new ShopItemScrollData();
                scrolData.eventText = i.ToString();
                shopItemLists.Add(scrolData);
            }
            shopitemScroll.InitScrollView(shopItemLists);
            //샵 정보 초기화
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

    public class ShopItemScrollData : CommonScrollItemData
    {
        public string eventText;

    }
}