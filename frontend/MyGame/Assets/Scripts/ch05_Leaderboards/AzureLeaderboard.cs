using UnityEngine;
using System.Net;
using System.IO;
using System.Text;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.UI;

public class AzureLeaderboard : MonoBehaviour
{
    public Transform leaderboard;
    public GameObject leaderboardRow;
    GameObject[] leaderboardEntries;
    public GameObject canvas;

    GameObject azure;
    private void Start()
    {
        azure = GameObject.Find("Azure");
    }

    public void UpdateAzureLeaderboard()
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://shared-apim.azure-api.net/leaderboard/Leaderboard");
        request.Method = "POST";

        request.Headers.Add("Authorization", "Bearer " + GetComponent<AzureSettings>().token);

        string displayName = azure.GetComponent<AzureSettings>().displayName;
        string playerID = azure.GetComponent<AzureSettings>().playerID;

        byte[] data = Encoding.ASCII.GetBytes("{PlayerID: '" + playerID + "', Value: '" + UnityEngine.Random.Range(0, 100) + "', DisplayName: '" + displayName + "'}");
        request.ContentType = "application/json";
        request.ContentLength = data.Length;

        Stream requestStream = request.GetRequestStream();
        requestStream.Write(data, 0, data.Length);
        requestStream.Close();

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string result = reader.ReadToEnd();
        Debug.Log("Update finished. " + result);
    }

    public void CloseAzureLeaderboard()
    {
        for (int i = 0; i < leaderboardEntries.Length; i++)
        {
            Destroy(leaderboardEntries[i]);
        }
        Array.Clear(leaderboardEntries, 0, leaderboardEntries.Length);
        canvas.SetActive(false);
    }
    public void GetAzureLeaderboard()
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://shared-apim.azure-api.net/leaderboard/Leaderboard?MaxResultsCount=6");
        request.Method = "GET";

        request.Headers.Add("Authorization", "Bearer " + GetComponent<AzureSettings>().token);

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string result = reader.ReadToEnd();

        List<LeaderboardEntry> list = JsonConvert.DeserializeObject<List<LeaderboardEntry>>(result);

        canvas.SetActive(true);
        leaderboardEntries = new GameObject[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            leaderboardEntries[i] = Instantiate(leaderboardRow, leaderboard);
            Text[] texts = leaderboardEntries[i].GetComponentsInChildren<Text>();
            texts[0].text = i.ToString();
            texts[1].text = list[i].DisplayName;
            texts[2].text = list[i].Value.ToString();
        }
    }
}

public class LeaderboardEntry
{
    public string PlayerID { get; set; }
    public int Value { get; set; }

    public string DisplayName { get; set; }
}

