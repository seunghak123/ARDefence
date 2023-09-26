using Seunghak.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
public class Purchase : MonoBehaviour, IStoreListener
{
    private static IStoreController storeController;
    private static IExtensionProvider storeProvider;
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {

        // Overall Purchasing system, configured with products for this application.
        storeController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        storeProvider = extensions;
    }
    public void InitializePurchasing()
    {
        if (IsInitialized())
        {
            return;
        }
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

#if !UNITY_EDITOR
        return;
        //유니티 에디터일때는 초기화를 안한다.
#elif UNITY_ANDROID || UNITY_IOS

#endif
        List<JShopData> shopData = JsonDataManager.LoadJsonDatas<JShopData>(E_JSON_TYPE.JShopData);
        

        UnityPurchasing.Initialize(this, builder);
    }
    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        return storeController != null && storeProvider != null;
    }
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        //결제 불가하다 어쩌구 띄우기
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        //상품 실패 관련 팝업띄우기
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {

        return PurchaseProcessingResult.Complete;
    }


}
