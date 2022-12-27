using UnityEngine;
using PlayFab.Party;
using PlayFab;
using PlayFab.ServerModels;

public class PlayFabChat : MonoBehaviour
{
    public bool doTranslateMessage { get; set; }
    public void StartParty()
    {
        PlayFabMultiplayerManager.Get().CreateAndJoinNetwork();

        PlayFabMultiplayerManager.Get().OnNetworkJoined += OnNetworkJoined;
        PlayFabMultiplayerManager.Get().OnChatMessageReceived += OnChatMessageReceived;
        PlayFabMultiplayerManager.Get().OnRemotePlayerJoined += OnRemotePlayerJoined;
        PlayFabMultiplayerManager.Get().OnRemotePlayerLeft += OnRemotePlayerLeft;
        PlayFabMultiplayerManager.Get().OnError += OnError;
    }

    private void OnNetworkJoined(object sender, string networkId)
    {
        Debug.Log("Joined to network: " + networkId);
        SetTitleData(networkId);
    }
    private void OnChatMessageReceived(object sender, PlayFabPlayer from, string message, ChatMessageType type)
    {
        PlayFabMultiplayerManager.Get().TranslateChat = doTranslateMessage;

        var displayName = this.GetComponent<PlayFabCloudScript>().GetDisplayNameFromCache(from.EntityKey.Id);
        if (displayName != null)
        {
            ChatUI.UpdateTextArea(displayName, message);
        }
        else
        {
            ChatUI.UpdateTextArea(from.EntityKey.Id, message);
            this.GetComponent<PlayFabCloudScript>().UpdateDisplayNameCache(from.EntityKey.Id);
        }
    }
    private void OnRemotePlayerJoined(object sender, PlayFabPlayer player)
    {
        Debug.Log("Joined new player: " + player.EntityKey.Id);
    }
    private void OnRemotePlayerLeft(object sender, PlayFabPlayer player)
    {
        Debug.Log("Player left: " + player.EntityKey.Id);
    }
    public void OnError(object sender, PlayFabMultiplayerManagerErrorArgs args)
    {
        Debug.Log("An error occured: " + args.Message);
    }

    public void SetTitleData(string networkId)
    {
        PlayFabServerAPI.SetTitleData(
            new SetTitleDataRequest
            {
                Key = "PartyNetworkId",
                Value = networkId
            },
            result => Debug.Log("Setting TitleData was successful."),
            error =>
            {
                Debug.Log("Error setting TitleData: " + error.GenerateErrorReport());
            }
        );
    }

    public void JoinParty()
    {
        PlayFabClientAPI.GetTitleData(new PlayFab.ClientModels.GetTitleDataRequest(),
            result =>
            {
                PlayFabMultiplayerManager.Get().JoinNetwork(result.Data["PartyNetworkId"]);
                PlayFabMultiplayerManager.Get().OnNetworkJoined += OnNetworkJoined;
                PlayFabMultiplayerManager.Get().OnChatMessageReceived += OnChatMessageReceived;
                PlayFabMultiplayerManager.Get().OnRemotePlayerJoined += OnRemotePlayerJoined;
                PlayFabMultiplayerManager.Get().OnRemotePlayerLeft += OnRemotePlayerLeft;
                PlayFabMultiplayerManager.Get().OnError += OnError;
            },
            error =>
            {
                Debug.Log("Error getting TitleData: " + error.GenerateErrorReport());
            }
        );
    }

    public void SendChatMessage(string message)
    {
        ChatUI.UpdateTextArea(this.gameObject.GetComponent<PlayFabSettings>().displayName, message);
        PlayFabMultiplayerManager.Get().SendChatMessageToAllPlayers(message);

    }

}


