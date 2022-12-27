using System.Collections.Generic;
using PlayFab.ClientModels;
public static class ShopUI
{
    private static string textArea = "\n\n\n\n\n";
    private static string virtualCurrencyLabel = "";
    public static void UpdateTextArea(List<ItemInstance> items)
    {
        string ta = "";
        int number = 0;
        foreach (ItemInstance item in items)
        {
            number++;
            ta += number + " | " + item.DisplayName + "\n";
        }

        for (int i = items.Count; i < 5; i++)
        {
            ta += "\n";
        }
        textArea = ta;
    }

    public static void UpdateVirtualCurrency(string VC_Name, int VC_Amount)
    {
        virtualCurrencyLabel = "You have " + VC_Amount + " " + VC_Name;
    }

    public static void UpdateTextArea(List<CatalogItem> items)
    {
        string ta = "";
        int number = 0;
        foreach (CatalogItem item in items)
        {
            number++;
            ta += number + " | " + item.DisplayName + ".....";
            if (item.VirtualCurrencyPrices.Count != 0)
            {
                ta += item.VirtualCurrencyPrices["MC"] + " MC";
            }
            else
            {
                ta += "n/a";
            }
            ta += "\n";
        }

        for (int i = items.Count; i < 5; i++)
        {
            ta += "\n";
        }
        textArea = ta;
    }

    public static void UpdateTextArea(List<Item> items)
    {
        string ta = "";
        int number = 0;
        foreach (var item in items)
        {
            number++;
            ta += number + " | " + item.DisplayName + ".....";
            ta += item.Price + " " + item.Currency;
            ta += "\n";
        }

        for (int i = items.Count; i < 5; i++)
        {
            ta += "\n";
        }
        textArea = ta;
    }

    public static string GetVirtualCurrencyLabel()
    {
        return virtualCurrencyLabel;
    }
    public static string GetTextArea()
    {
        return textArea;
    }
}