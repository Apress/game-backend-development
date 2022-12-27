using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using System;

public class PlayFabLeaderboard : MonoBehaviour
{
    public Transform leaderboard;
    public GameObject leaderboardRow;
    GameObject[] leaderboardEntries;
    public GameObject canvas;

    public void UpdatePlayFabLeaderboard()
    {
        PlayFabClientAPI.UpdatePlayerStatistics(
            new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate>
                {
                    new StatisticUpdate
                    {
                        StatisticName = "HighScore",
                        Value = UnityEngine.Random.Range(0,100)
                    }
                }
            },

            (UpdatePlayerStatisticsResult result) =>
            {
                Debug.Log("Leaderboard updated.");
            },

            (PlayFabError error) =>
            {
                Debug.LogError(error.GenerateErrorReport());
            });

    }

    public void ClosePlayFabLeaderboard()
    {
        for (int i = 0; i < leaderboardEntries.Length; i++)
        {
            Destroy(leaderboardEntries[i]);
        }
        Array.Clear(leaderboardEntries, 0, leaderboardEntries.Length);
        canvas.SetActive(false);
    }
    public void GetPlayFabLeaderboard()
    {
        PlayFabClientAPI.GetLeaderboard(
            new GetLeaderboardRequest
            {
                StatisticName = "HighScore",
                StartPosition = 0,
                MaxResultsCount = 6
            },

            (GetLeaderboardResult result) =>
            {
                canvas.SetActive(true);
                leaderboardEntries = new GameObject[result.Leaderboard.Count];
                for (int i = 0; i < result.Leaderboard.Count; i++)
                {
                    leaderboardEntries[i] = Instantiate(leaderboardRow, leaderboard);
                    Text[] texts = leaderboardEntries[i].GetComponentsInChildren<Text>();
                    texts[0].text = result.Leaderboard[i].Position.ToString();
                    texts[1].text = result.Leaderboard[i].DisplayName;
                    texts[2].text = result.Leaderboard[i].StatValue.ToString();
                }
            },

            (PlayFabError error) =>
            {
                Debug.LogError(error.GenerateErrorReport());
            });
    }
}
