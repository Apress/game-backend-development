using System.Collections.Generic;
using UnityEngine;
using Mirror;
using PlayFab.MultiplayerModels;
using PlayFab;
using kcp2k;

public class PlayFabClient : NetworkManager
{
    GameObject playFab;
    public void StartPlayFabClient()
    {
        playFab = GameObject.Find("PlayFab");

        RequestMultiplayerServerRequest requestData = new RequestMultiplayerServerRequest();

        requestData.BuildId = playFab.GetComponent<PlayFabSettings>().buildId;
        requestData.PreferredRegions = new List<string>() { "EastUs" };
        requestData.SessionId = playFab.GetComponent<PlayFabSettings>().sessionId;
        if (requestData.SessionId.Equals(""))
        {
            requestData.SessionId = System.Guid.NewGuid().ToString();
            playFab.GetComponent<PlayFabSettings>().sessionId = requestData.SessionId;
        }

        PlayFabMultiplayerAPI.RequestMultiplayerServer(requestData, OnRequestMultiplayerServer, OnRequestMultiplayerServerError);
    }

    private void OnRequestMultiplayerServer(RequestMultiplayerServerResponse response)
    {
        this.networkAddress = response.IPV4Address;
        this.GetComponent<KcpTransport>().Port = (ushort)response.Ports[0].Num;
        Debug.Log("Server found. IP " + this.networkAddress + ":" + this.GetComponent<KcpTransport>().Port);
        Debug.Log("SessionId: " + response.SessionId);
        this.StartClient();
    }
    private void OnRequestMultiplayerServerError(PlayFabError error)
    {
        Debug.Log(error.ErrorMessage);
    }

}