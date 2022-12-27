using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Text;

public class AzureEconomy : MonoBehaviour
{
    Catalog catalog = new Catalog();
    PlayerVirtualCurrency playerVirtualCurrency = new PlayerVirtualCurrency();
    UnityIAPManager unityIAPManager;

    public void GetCatalogItems()
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://shared-apim.azure-api.net/economy/Catalog?CatalogId=MyGame%20Catalog");
        request.Method = "GET";

        request.Headers.Add("Authorization", "Bearer " + GetComponent<AzureSettings>().token);

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string result = reader.ReadToEnd();

        catalog = JsonConvert.DeserializeObject<Catalog>(result);

        unityIAPManager = new UnityIAPManager(catalog);

        ShopUI.UpdateTextArea(catalog.Items);
    }

    public void GetInventory()
    {
        var playerId = this.gameObject.GetComponent<AzureSettings>().playerID;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://shared-apim.azure-api.net/economy/Inventory?PlayerId=" + playerId);
        request.Method = "GET";

        request.Headers.Add("Authorization", "Bearer " + GetComponent<AzureSettings>().token);

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string result = reader.ReadToEnd();
        Debug.Log(result);

        if (!result.Equals("This player has no items so far."))
        {
            var inventory = JsonConvert.DeserializeObject<PlayerInventory>(result);

            ShopUI.UpdateTextArea(inventory.Items);
        }
        GetVirtualCurrency();

    }

    private void GetVirtualCurrency()
    {
        var playerId = this.gameObject.GetComponent<AzureSettings>().playerID;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://shared-apim.azure-api.net/economy/VirtualCurrency?PlayerId=" + playerId);
        request.Method = "GET";

        request.Headers.Add("Authorization", "Bearer " + GetComponent<AzureSettings>().token);

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string result = reader.ReadToEnd();
        Debug.Log(result);

        if (!result.Equals("This player has no virtual currency."))
        {
            playerVirtualCurrency = JsonConvert.DeserializeObject<PlayerVirtualCurrency>(result);

            ShopUI.UpdateVirtualCurrency(playerVirtualCurrency.VirtualCurrencies[0].VC_Name, playerVirtualCurrency.VirtualCurrencies[0].VC_Amount);
        }
    }

    public void SetVirtualCurrency(string virtualCurrencyName, int virtualCurrencyAmount)
    {
        var playerId = this.gameObject.GetComponent<AzureSettings>().playerID;

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://shared-apim.azure-api.net/economy/VirtualCurrency");
        request.Method = "POST";

        request.Headers.Add("Authorization", "Bearer " + GetComponent<AzureSettings>().token);

        VirtualCurrency virtualCurrency = new VirtualCurrency()
        {
            VC_Name = virtualCurrencyName,
            VC_Amount = virtualCurrencyAmount
        };

        PlayerVirtualCurrency playerVirtualCurrency = new PlayerVirtualCurrency()
        {
            PlayerId = playerId,
            VirtualCurrencies = new List<VirtualCurrency>() { virtualCurrency }
        };

        var json = JsonConvert.SerializeObject(playerVirtualCurrency);

        byte[] data = Encoding.ASCII.GetBytes(json);

        request.ContentType = "application/json";
        request.ContentLength = data.Length;

        Stream requestStream = request.GetRequestStream();
        requestStream.Write(data, 0, data.Length);
        requestStream.Close();

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());

        string result = reader.ReadToEnd();

        GetVirtualCurrency();

        Debug.Log(result);
    }

    public void PurchaseItem(string itemToPurchase)
    {
        int itemNumber = int.Parse(itemToPurchase) - 1;
        var playerId = this.gameObject.GetComponent<AzureSettings>().playerID;

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://shared-apim.azure-api.net/economy/Inventory");
        request.Method = "POST";

        request.Headers.Add("Authorization", "Bearer " + GetComponent<AzureSettings>().token);

        PlayerInventory playerInventory = new PlayerInventory()
        {
            PlayerId = playerId,
            Items = new List<Item>() { catalog.Items[itemNumber] }
        };

        var json = JsonConvert.SerializeObject(playerInventory);

        byte[] data = Encoding.ASCII.GetBytes(json);

        request.ContentType = "application/json";
        request.ContentLength = data.Length;

        Stream requestStream = request.GetRequestStream();
        requestStream.Write(data, 0, data.Length);
        requestStream.Close();

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());

        string result = reader.ReadToEnd();
        Debug.Log(result);

        if (result != "Item is already existing in the inventory.")
        {
            SetVirtualCurrency("MC", playerVirtualCurrency.VirtualCurrencies[0].VC_Amount - catalog.Items[itemNumber].Price);
        }
    }
    public void BuyFromStore(string itemToPurchase)
    {
        int itemNumber = int.Parse(itemToPurchase) - 1;
        unityIAPManager.PurchaseItem(catalog.Items[itemNumber].ItemId);
    }
}

public class Catalog
{
    public string CatalogId { get; set; }
    public List<Item> Items { get; set; }
}

public class PlayerInventory
{
    public string PlayerId { get; set; }
    public List<Item> Items { get; set; }
}

public class Item
{
    public string ItemId { get; set; }
    public string DisplayName { get; set; }
    public int Price { get; set; }
    public string Currency { get; set; }
}
public class PlayerVirtualCurrency
{
    public string PlayerId { get; set; }
    public List<VirtualCurrency> VirtualCurrencies { get; set; }

}

public class VirtualCurrency
{
    public int VC_Amount { get; set; }
    public string VC_Name { get; set; }
}

