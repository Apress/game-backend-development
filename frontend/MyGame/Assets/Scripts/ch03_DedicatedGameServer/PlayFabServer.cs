using System.Collections;
using UnityEngine;
using Mirror;
using PlayFab;
public class PlayFabServer : NetworkManager
{
    private int numberOfConnectedPlayers = 0;
    public override void Start()
    {
        StartPlayFabAPI();
        this.StartServer();
    }
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);
        Debug.Log("Connected client to server, ConnectionId: " + conn.connectionId);
        numberOfConnectedPlayers++;

    }
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        Debug.Log("Client disconnected from server, ConnectionId: " + conn.connectionId);
        numberOfConnectedPlayers--;

        if (numberOfConnectedPlayers == 0)
        {
            StartCoroutine(Shutdown());
        }
    }
    void StartPlayFabAPI()
    {
        PlayFabMultiplayerAgentAPI.Start();
        StartCoroutine(ReadyForPlayers());
    }
    IEnumerator ReadyForPlayers()
    {
        yield return new WaitForSeconds(.5f);
        PlayFabMultiplayerAgentAPI.ReadyForPlayers();
    }
    private IEnumerator Shutdown()
    {
        yield return new WaitForSeconds(5f);
        Application.Quit();
    }
}
