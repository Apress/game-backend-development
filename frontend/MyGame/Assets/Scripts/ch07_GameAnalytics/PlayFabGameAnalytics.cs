using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabGameAnalytics : MonoBehaviour
{
    public void CreatePlayerExitedEvent(int Level)
    {
        Dictionary<string, object> EventBody = new Dictionary<string, object>
            {
                { "Level", Level }
            };

        CreateEvent("player_exited", EventBody);
    }

    void CreateEvent(string EventName, Dictionary<string, object> EventBody)
    {
        PlayFabClientAPI.WritePlayerEvent(new WriteClientPlayerEventRequest
        {
            EventName = EventName,
            Body = EventBody
        },
        (WriteEventResponse result) => Debug.Log("Event sent"),
        (PlayFabError error) => Debug.LogError(error.GenerateErrorReport()));
    }
}
