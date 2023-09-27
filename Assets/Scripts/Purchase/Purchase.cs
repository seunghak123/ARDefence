using Seunghak.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

public class Purchase : MonoBehaviour, IStoreListener
{
    private static IStoreController storeController;
    private static IExtensionProvider storeProvider;
    private CrossPlatformValidator validator = null;

    #region UserPurchaseMethod
    public string GetProductPrice(string productId)
    {
        if (IsInitialized() == false)
            return string.Empty;

        Product product = storeController.products.WithID(productId);
        if (product == null)
            return string.Empty;

        return product.metadata.localizedPriceString;
    }
    public void RestorePurchase()
    {
        if (!IsInitialized())
        {
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
        {
            Debug.Log("RestorePurchases started ...");

            var apple = storeProvider.GetExtension<IAppleExtensions>();

            apple.RestoreTransactions
                (
                    (result) => { Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore."); }
                );
        }
        else
        {
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }
    Action<string> resultAction = null;
    public void BuyProductID(string productId, Action<string> actionProduct)
    {
        try
        {
            if (IsInitialized())
            {
                resultAction = actionProduct;

                Product p = storeController.products.WithID(productId);

                if (p != null && p.availableToPurchase)
                {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", p.definition.id));
                    storeController.InitiatePurchase(p);
                }
                else
                {
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            else
            {
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }
        catch (Exception e)
        {
            Debug.Log("BuyProductID: FAIL. Exception during purchase. " + e);
        }
    }
    #endregion
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        storeController = controller;
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

        //Json파일의 상품 데이터를 긁어와서, Consumable 상품 등록(패키지 형태)
        //// Add a product to sell / restore by way of its identifier, associating the general identifier with its store-specific identifiers.
        //builder.AddProduct(kProductIDConsumable, ProductType.Consumable, new IDs() { { kProductNameAppleConsumable, AppleAppStore.Name }, { kProductNameGooglePlayConsumable, GooglePlay.Name }, });// Continue adding the non-consumable product.
        //builder.AddProduct(kProductIDNonConsumable, ProductType.NonConsumable, new IDs() { { kProductNameAppleNonConsumable, AppleAppStore.Name }, { kProductNameGooglePlayNonConsumable, GooglePlay.Name }, });// And finish adding the subscription product.
        //builder.AddProduct(kProductIDSubscription, ProductType.Subscription, new IDs() { { kProductNameAppleSubscription, AppleAppStore.Name }, { kProductNameGooglePlaySubscription, GooglePlay.Name }, });// Kick off the remainder of the set-up with an asynchrounous call, passing the configuration and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
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
        //결제 실패 팝업 띄우기
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
    {
        bool bValidPurchase = true;
#if !UNITY_EDITOR
        try
        {
            var result = validator.Validate(purchaseEvent.purchasedProduct.receipt);

            bool bValidData = false;

            Debug.Log("Receipt is valid. Contents:");
            foreach (IPurchaseReceipt productReceipt in result)
            {
                //Debug.Log(productReceipt.productID);
                //Debug.Log(productReceipt.purchaseDate);
                //Debug.Log(productReceipt.transactionID);

                if (productReceipt.productID == purchaseEvent.purchasedProduct.definition.id)
                {
                    bValidData = true;
                    break;
                }

            }

            bValidPurchase = bValidData;
        }
        catch (IAPSecurityException)
        {
            Debug.Log("Invalid receipt, not unlocking content");
            bValidPurchase = false;
        }
#endif

        if (resultAction != null)
        {
            if (bValidPurchase == true)
                resultAction(purchaseEvent.purchasedProduct.receipt);
            else
                resultAction(string.Empty);
        }
        return PurchaseProcessingResult.Complete;
    }


}
