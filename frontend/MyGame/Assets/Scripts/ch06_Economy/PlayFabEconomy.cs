using System.Collections.Generic;
using UnityEngine;
using PlayFab.ClientModels;
using PlayFab;
using UnityEngine.Purchasing;

public class PlayFabEconomy : MonoBehaviour
{
    List<ItemInstance> playerInventoryItems = new List<ItemInstance>();
    List<CatalogItem> catalogItems = new List<CatalogItem>();

    UnityIAPManager unityIAPManager;

    public void GetCatalogItems()
    {
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest()
        {
            CatalogVersion = "MyGame Catalog",
        },
        result =>
        {
            catalogItems.Clear();
            foreach (CatalogItem item in result.Catalog) catalogItems.Add(item);
            ShopUI.UpdateTextArea(catalogItems);
            unityIAPManager = new UnityIAPManager(catalogItems);
        },
        error =>
        {
            Debug.Log(error.GenerateErrorReport());
        });
    }

    public void GetInventory()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
        result =>
        {
            playerInventoryItems.Clear();
            foreach (ItemInstance item in result.Inventory) playerInventoryItems.Add(item);
            ShopUI.UpdateTextArea(playerInventoryItems);
            ShopUI.UpdateVirtualCurrency("MC", result.VirtualCurrency["MC"]);

        },
        error =>
        {
            Debug.Log(error.GenerateErrorReport());
        });
    }

    public void PurchaseItem(string itemToPurchase)
    {
        int itemNumber = int.Parse(itemToPurchase) - 1;

        PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest()
        {
            CatalogVersion = "MyGame Catalog",
            ItemId = catalogItems[itemNumber].ItemId,
            Price = (int)catalogItems[itemNumber].VirtualCurrencyPrices["MC"],
            VirtualCurrency = "MC"
        },
        result =>
        {
            Debug.Log("Purchase completed.");
        },
        error =>
        {
            Debug.Log(error.GenerateErrorReport());
        });
    }

    public void BuyFromStore(string itemToPurchase)
    {
        int itemNumber = int.Parse(itemToPurchase) - 1;
        unityIAPManager.PurchaseItem(catalogItems[itemNumber].ItemId);
    }

}
