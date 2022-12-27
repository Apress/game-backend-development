using UnityEngine;
using UnityEngine.Purchasing;
using System.Collections.Generic;
using PlayFab.ClientModels;

public class UnityIAPManager : IStoreListener
{
    private IStoreController controller;
    public UnityIAPManager(List<CatalogItem> catalog)
    {
        InitializeUnityIAP(catalog);
    }
    public UnityIAPManager(Catalog catalog)
    {
        InitializeUnityIAP(catalog.Items);
    }

    public void InitializeUnityIAP(List<CatalogItem> catalog)
    {
        var module = StandardPurchasingModule.Instance();
        var builder = ConfigurationBuilder.Instance(module);

        foreach (var item in catalog)
        {
            builder.AddProduct(item.ItemId, ProductType.NonConsumable);
        }
        UnityPurchasing.Initialize(this, builder);
    }

    public void InitializeUnityIAP(List<Item> catalog)
    {
        var module = StandardPurchasingModule.Instance();
        var builder = ConfigurationBuilder.Instance(module);

        foreach (var item in catalog)
        {
            builder.AddProduct(item.ItemId, ProductType.NonConsumable);
        }
        UnityPurchasing.Initialize(this, builder);
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("Unity IAP is initialized. It is ready to make purchases.");
        this.controller = controller;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("Initialization error: " + error);
    }

    public void PurchaseItem(string productId)
    {
        controller.InitiatePurchase(productId);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        Debug.Log("Purchase completed. ProductId: " + e.purchasedProduct.definition.id + " Receipt: " + e.purchasedProduct.receipt);

        // TODO : receipt validation + grant item (only with real store / receipt)

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        Debug.Log("Purchase failed. Reason: " + p);
    }
}