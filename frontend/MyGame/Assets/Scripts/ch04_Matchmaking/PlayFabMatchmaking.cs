using System.Collections;
using UnityEngine;
using PlayFab;
using PlayFab.MultiplayerModels;
using kcp2k;

public class PlayFabMatchmaking : MonoBehaviour
{

    string ticketId;
    string myQueueName = "mygamematchmakingqueue";
    private IEnumerator coroutine;
    GameObject playFabClient;

    public void StartPlayFabMatchmaking()
    {
        CreateMatchMakingTicket(GetComponent<PlayFabSettings>().entityId);
        playFabClient = GameObject.Find("PlayFabClient");
    }
    public void CreateMatchMakingTicket(string entityId)
    {
        PlayFabMultiplayerAPI.CreateMatchmakingTicket(
        new CreateMatchmakingTicketRequest
        {
            Creator = new MatchmakingPlayer
            {
                Entity = new EntityKey
                {
                    Id = entityId,
                    Type = "title_player_account",
                },

                Attributes = new MatchmakingPlayerAttributes
                {
                    DataObject = new
                    {
                        latencies = new object[]
                            {
                                new {
                                    region = "NorthEurope",
                                    latency = 100
                                },
                                new {
                                    region = "EastUs",
                                    latency = 150
                                }
                            }
                    },
                },
            },

            QueueName = myQueueName,
        },

        this.OnMatchmakingTicketCreated,
        this.OnMatchmakingError);
    }

    private void OnMatchmakingTicketCreated(CreateMatchmakingTicketResult obj)
    {
        Debug.Log("Matchmaking Ticket Created: " + obj.TicketId);
        ticketId = obj.TicketId;
        coroutine = WaitAndGetMatchingTicket(6.0f);
        StartCoroutine(coroutine);
    }
    private void OnMatchmakingError(PlayFabError obj)
    {
        Debug.Log("Matchmaking Error: " + obj.Error);
    }

    private IEnumerator WaitAndGetMatchingTicket(float waitTime)
    {
        while (true)
        {
            GetMatchingTicket();
            yield return new WaitForSeconds(waitTime);
        }
    }
    public void GetMatchingTicket()
    {

        PlayFabMultiplayerAPI.GetMatchmakingTicket(
        new GetMatchmakingTicketRequest
        {
            TicketId = ticketId,
            QueueName = "mygamematchmakingqueue",
        },
        this.OnGetMatchmakingTicket,
        this.OnMatchmakingError);
    }

    private void OnGetMatchmakingTicket(GetMatchmakingTicketResult obj)
    {
        Debug.Log("GetMatchmakingTicket: " + obj.Status);

        if (obj.Status.Equals("Matched"))
        {
            StopCoroutine(coroutine);
            PlayFabMultiplayerAPI.GetMatch(new GetMatchRequest
            {
                QueueName = myQueueName,
                MatchId = obj.MatchId

            },
            this.OnGetMatchResult,
            this.OnMatchmakingError);
        }
    }

    private void OnGetMatchResult(GetMatchResult result)
    {
        playFabClient.GetComponent<PlayFabClient>().networkAddress = result.ServerDetails.IPV4Address;
        playFabClient.GetComponent<KcpTransport>().Port = (ushort)result.ServerDetails.Ports[0].Num;
        playFabClient.GetComponent<PlayFabClient>().StartClient();
    }

    public void CancelPlayFabMatchmaking()
    {
        StopCoroutine(coroutine);
        CancelMatchmakingTicket();
    }
    private void CancelMatchmakingTicket()
    {
        PlayFabMultiplayerAPI.CancelMatchmakingTicket(
        new CancelMatchmakingTicketRequest
        {
            QueueName = myQueueName,
            TicketId = ticketId,
        },
        this.OnTicketCanceled,
        this.OnMatchmakingError);
    }

    private void OnTicketCanceled(CancelMatchmakingTicketResult obj)
    {
        Debug.Log("Ticket cancelled.");
    }

}
