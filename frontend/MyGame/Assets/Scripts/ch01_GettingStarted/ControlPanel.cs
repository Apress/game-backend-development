using UnityEngine;
using Mirror;
using kcp2k;
public class ControlPanel : MonoBehaviour
{
    public const int ROOTMENU = 0;
    public const int PLAYFAB_LOGIN = 1;
    public const int PLAYFAB_LOGINWITHUSERPASS = 2;
    public const int AZURE_LOGIN = 3;
    public const int AZURE_STARTCLIENT = 4;
    public const int PLAYFAB_GETLEADERBOARD = 5;
    public const int AZURE_GETLEADERBOARD = 6;
    public const int PLAYFAB_ECONOMY = 7;
    public const int AZURE_ECONOMY = 8;
    public const int PLAYFAB_CHAT = 9;
    public const int AZURE_CHAT = 10;

    int selection;

    string username = "";
    string password = "";
    string displayName = "";

    string serverIP = "";
    string serverPort = "";

    GameObject playFab;
    GameObject azure;

    GameObject playFabClient;

    GameObject azureClient;

    bool playFabMatchmakingInProgress = false;
    bool azureMatchmakingInProgress = false;

    string chatMessage = "";
    bool isTranslated = false;
    string textArea = "\n\n\n\n";

    string economyTextAreaTitle = "";
    string itemToPurchase = "";
    string virtualCurrencyLabel = "";

    private void Start()
    {
        playFab = GameObject.Find("PlayFab");
        azure = GameObject.Find("Azure");
        playFabClient = GameObject.Find("PlayFabClient");
        azureClient = GameObject.Find("AzureClient");
    }

    void OnGUI()
    {
        if (selection == ROOTMENU)
            GUILayout.Window(0, new Rect(0, 0, 300, 0), OptionsWindow, "Options");

        if (selection == PLAYFAB_LOGIN)
            GUILayout.Window(0, new Rect(0, 0, 300, 0), LoginWithPlayFabWindow, "Login with PlayFab");

        if (selection == AZURE_LOGIN)
            GUILayout.Window(0, new Rect(0, 0, 300, 0), LoginWithAzureWindow, "Login with Azure");

        if (selection == PLAYFAB_LOGINWITHUSERPASS)
            GUILayout.Window(0, new Rect(0, 0, 300, 0), LoginWithPlayFabUserPass, "Login with username/password");

        if (selection == AZURE_STARTCLIENT)
            GUILayout.Window(0, new Rect(0, 0, 300, 0), StartAzureClient, "Start Azure Client");

        if (selection == PLAYFAB_GETLEADERBOARD)
            GUILayout.Window(0, new Rect(0, 0, 300, 0), GetPlayFabLeaderboard, "PlayFab Leaderboard");

        if (selection == AZURE_GETLEADERBOARD)
            GUILayout.Window(0, new Rect(0, 0, 300, 0), GetAzureLeaderboard, "Azure Leaderboard");

        if (selection == PLAYFAB_ECONOMY)
            GUILayout.Window(0, new Rect(0, 0, 300, 0), PlayFabEconomyWindow, "PlayFab Economy");

        if (selection == AZURE_ECONOMY)
            GUILayout.Window(0, new Rect(0, 0, 300, 0), AzureEconomyWindow, "Azure Economy");

        if (selection == PLAYFAB_CHAT)
            GUILayout.Window(0, new Rect(0, 0, 300, 0), PlayFabChatWindow, "PlayFab Chat");

        if (selection == AZURE_CHAT)
            GUILayout.Window(0, new Rect(0, 0, 300, 0), AzureChatWindow, "Azure Chat");

    }

    void OptionsWindow(int windowID)
    {
        if (GUILayout.Button("Login with PlayFab"))
            selection = PLAYFAB_LOGIN;

        if (GUILayout.Button("Login with Azure"))
            selection = AZURE_LOGIN;

        GUILayout.Space(10);

        if (GUILayout.Button("Start PlayFab Client"))
            playFabClient.GetComponent<PlayFabClient>().StartPlayFabClient();

        if (GUILayout.Button("Start Azure Client"))
            selection = AZURE_STARTCLIENT;

        GUILayout.Space(10);

        if (!playFabMatchmakingInProgress)
        {
            if (GUILayout.Button("Start PlayFab Matchmaking"))
            {
                playFab.GetComponent<PlayFabMatchmaking>().StartPlayFabMatchmaking();
                playFabMatchmakingInProgress = true;
            }
        }
        else if (playFabMatchmakingInProgress)
        {
            if (GUILayout.Button("Cancel PlayFab Matchmaking"))
            {
                playFab.GetComponent<PlayFabMatchmaking>().CancelPlayFabMatchmaking();
                playFabMatchmakingInProgress = false;
            }
        }

        if (!azureMatchmakingInProgress)
        {
            if (GUILayout.Button("Start Azure Matchmaking"))
            {
                azure.GetComponent<AzureMatchmaking>().StartAzureMatchmaking();
                azureMatchmakingInProgress = true;
            }
        }
        else if (azureMatchmakingInProgress)
        {
            if (GUILayout.Button("Cancel Azure Matchmaking"))
            {
                azure.GetComponent<AzureMatchmaking>().CancelAzureMatchmaking();
                azureMatchmakingInProgress = false;
            }
        }

        GUILayout.Space(10);



        if (GUILayout.Button("Get PlayFab Leaderboard"))
        {
            playFab.GetComponent<PlayFabLeaderboard>().GetPlayFabLeaderboard();
            selection = PLAYFAB_GETLEADERBOARD;
        }

        if (GUILayout.Button("Get Azure Leaderboard"))
        {
            azure.GetComponent<AzureLeaderboard>().GetAzureLeaderboard();
            selection = AZURE_GETLEADERBOARD;
        }

        GUILayout.Space(10);


        if (GUILayout.Button("PlayFab Economy"))
        {
            selection = PLAYFAB_ECONOMY;
        }


        if (GUILayout.Button("Azure Economy"))
        {
            selection = AZURE_ECONOMY;
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Send PlayFab Event"))
        {
            playFab.GetComponent<PlayFabGameAnalytics>().CreatePlayerExitedEvent(UnityEngine.Random.Range(1, 10));
        }

        if (GUILayout.Button("Send Azure Event"))
        {
            azure.GetComponent<AzureGameAnalytics>().CreatePlayerExitedEvent(UnityEngine.Random.Range(1, 10));
        }


        GUILayout.Space(10);


        if (GUILayout.Button("PlayFab Chat"))
        {
            selection = PLAYFAB_CHAT;
        }

        if (GUILayout.Button("Azure Chat"))
        {
            azure.GetComponent<AzureChat>().ConnectToChat();
            selection = AZURE_CHAT;


        }
    }

    void PlayFabEconomyWindow(int windowID)
    {
        if (economyTextAreaTitle != "")
            GUILayout.Label(economyTextAreaTitle);

        textArea = ShopUI.GetTextArea();
        textArea = GUILayout.TextArea(textArea, 200);

        virtualCurrencyLabel = ShopUI.GetVirtualCurrencyLabel();
        if (virtualCurrencyLabel != "")
            GUILayout.Label(virtualCurrencyLabel);

        GUILayout.Space(10);

        if (GUILayout.Button("Get Catalog Items"))
        {
            playFab.GetComponent<PlayFabEconomy>().GetCatalogItems();
            economyTextAreaTitle = "Catalog Items";
        }

        if (GUILayout.Button("Get Player Inventory + Virtual Currency"))
        {
            playFab.GetComponent<PlayFabEconomy>().GetInventory();
            economyTextAreaTitle = "Player Inventory";
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Purchase this item (number):"))
        {
            playFab.GetComponent<PlayFabEconomy>().PurchaseItem(itemToPurchase);

        }

        itemToPurchase = GUILayout.TextField(itemToPurchase, 100);

        GUILayout.Space(10);

        if (GUILayout.Button("Buy from store"))
        {
            playFab.GetComponent<PlayFabEconomy>().BuyFromStore(itemToPurchase);
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Cancel"))
        {
            selection = ROOTMENU;
        }
    }

    void AzureEconomyWindow(int windowID)
    {
        if (economyTextAreaTitle != "")
            GUILayout.Label(economyTextAreaTitle);

        textArea = ShopUI.GetTextArea();
        textArea = GUILayout.TextArea(textArea, 200);

        virtualCurrencyLabel = ShopUI.GetVirtualCurrencyLabel();
        if (virtualCurrencyLabel != "")
            GUILayout.Label(virtualCurrencyLabel);

        GUILayout.Space(10);

        if (GUILayout.Button("Get Catalog Items"))
        {
            azure.GetComponent<AzureEconomy>().GetCatalogItems();
            economyTextAreaTitle = "Catalog Items";
        }

        if (GUILayout.Button("Get Player Inventory + Virtual Currency"))
        {
            azure.GetComponent<AzureEconomy>().GetInventory();
            economyTextAreaTitle = "Player Inventory";
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Grant Virtual Currency"))
        {
            azure.GetComponent<AzureEconomy>().SetVirtualCurrency("MC", 100);
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Purchase this item (number):"))
        {
            azure.GetComponent<AzureEconomy>().PurchaseItem(itemToPurchase);

        }

        itemToPurchase = GUILayout.TextField(itemToPurchase, 100);

        GUILayout.Space(10);

        if (GUILayout.Button("Buy from store"))
        {
            azure.GetComponent<AzureEconomy>().BuyFromStore(itemToPurchase);
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Cancel"))
        {
            selection = ROOTMENU;
        }
    }

    void PlayFabChatWindow(int windowID)
    {
        textArea = ChatUI.GetTextArea();
        textArea = GUILayout.TextArea(textArea, 200);

        chatMessage = GUILayout.TextField(chatMessage, 100);

        if (GUILayout.Button("Send Message"))
        {
            playFab.GetComponent<PlayFabChat>().SendChatMessage(chatMessage);
            chatMessage = "";
        }

        isTranslated = GUILayout.Toggle(isTranslated, "Translate Messages");
        playFab.GetComponent<PlayFabChat>().doTranslateMessage = isTranslated;

        GUILayout.Space(10);

        if (GUILayout.Button("Create PlayFab Party"))
            playFab.GetComponent<PlayFabChat>().StartParty();

        if (GUILayout.Button("Join PlayFab Party"))
            playFab.GetComponent<PlayFabChat>().JoinParty();

        GUILayout.Space(10);

        if (GUILayout.Button("Cancel"))
            selection = ROOTMENU;
    }


    void AzureChatWindow(int windowID)
    {
        textArea = ChatUI.GetTextArea();
        textArea = GUILayout.TextArea(textArea, 200);

        chatMessage = GUILayout.TextField(chatMessage, 100);

        if (GUILayout.Button("Send Message"))
        {
            azure.GetComponent<AzureChat>().SendChatMessage(chatMessage);
            chatMessage = "";
        }

        isTranslated = GUILayout.Toggle(isTranslated, "Translate Messages");
        azure.GetComponent<AzureChat>().doTranslateMessage = isTranslated;

        GUILayout.Space(10);

        if (GUILayout.Button("Cancel"))
            selection = ROOTMENU;
    }


    void GetPlayFabLeaderboard(int windowID)
    {
        if (GUILayout.Button("Update PlayFab Leaderboard"))
            playFab.GetComponent<PlayFabLeaderboard>().UpdatePlayFabLeaderboard();

        if (GUILayout.Button("Cancel"))
        {
            playFab.GetComponent<PlayFabLeaderboard>().ClosePlayFabLeaderboard();
            selection = ROOTMENU;
        }
    }


    void GetAzureLeaderboard(int windowID)
    {
        if (GUILayout.Button("Update Azure Leaderboard"))
            azure.GetComponent<AzureLeaderboard>().UpdateAzureLeaderboard();

        if (GUILayout.Button("Cancel"))
        {
            azure.GetComponent<AzureLeaderboard>().CloseAzureLeaderboard();
            selection = ROOTMENU;
        }
    }

    void LoginWithPlayFabWindow(int windowID)
    {
        GUILayout.Label("Display name:");
        displayName = GUILayout.TextField(displayName, 20);

        if (!displayName.Equals(""))
        {
            if (GUILayout.Button("Login as Guest"))
            {
                playFab.GetComponent<PlayFabAuth>().PlayFabLoginWithCustomID(displayName);
                selection = ROOTMENU;
            }

            if (GUILayout.Button("Login with username/password"))
            {
                selection = PLAYFAB_LOGINWITHUSERPASS;
            }
        }

        if (GUILayout.Button("Cancel"))
            selection = ROOTMENU;
    }

    void LoginWithPlayFabUserPass(int windowID)
    {
        GUILayout.Label("User name:");
        username = GUILayout.TextField(username, 10);
        GUILayout.Label("Password:");
        password = GUILayout.PasswordField(password, '*', 10);

        if (GUILayout.Button("Register and/or Login"))
            playFab.GetComponent<PlayFabAuth>().PlayFabLoginWithUsernameAndPassword(username, password, displayName);

        if (GUILayout.Button("Cancel"))
        {
            displayName = playFab.GetComponent<PlayFabSettings>().displayName;
            selection = ROOTMENU;
        }
    }

    void LoginWithAzureWindow(int windowID)
    {

        if (GUILayout.Button("Login with Azure"))
            azure.GetComponent<AzureAuth>().LoginWithAzure();

        if (GUILayout.Button("Cancel"))
            selection = ROOTMENU;
    }

    void StartAzureClient(int windowID)
    {
        GUILayout.Label("Server IP:");
        serverIP = GUILayout.TextField(serverIP, 20);
        GUILayout.Label("Server Port:");
        serverPort = GUILayout.TextField(serverPort, 10);

        if (GUILayout.Button("Start Azure Client"))
        {
            azureClient.GetComponent<NetworkManager>().networkAddress = serverIP;
            azureClient.GetComponent<KcpTransport>().Port = (ushort)int.Parse(serverPort);
            azureClient.GetComponent<NetworkManager>().StartClient();
        }

        if (GUILayout.Button("Cancel"))
            selection = ROOTMENU;
    }
}
