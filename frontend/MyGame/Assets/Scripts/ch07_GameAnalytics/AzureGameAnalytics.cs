using UnityEngine;
using System.Net;
using System.IO;
using System.Text;

public class AzureGameAnalytics : MonoBehaviour
{
    GameObject azure;
    private void Start()
    {
        azure = GameObject.Find("Azure");
    }
    public void CreatePlayerExitedEvent(int Level)
    {
        string Eventbody = "\"eventname\":\"player_exited\", \"Level\": " + Level;
        CreateAzureEvent(Eventbody);
    }

    public void CreateAzureEvent(string Eventbody)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://shared-apim.azure-api.net/gameanalytics/GameAnalyticsFunc");
        request.Method = "POST";

        request.Headers.Add("Authorization", "Bearer " + GetComponent<AzureSettings>().token);

        string playerID = azure.GetComponent<AzureSettings>().playerID;

        byte[] data = Encoding.ASCII.GetBytes("{\"PlayerID\": \"" + playerID + "\", " + Eventbody + "}");
        request.ContentType = "application/json";
        request.ContentLength = data.Length;

        Stream requestStream = request.GetRequestStream();
        requestStream.Write(data, 0, data.Length);
        requestStream.Close();

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string result = reader.ReadToEnd();
        Debug.Log("Event sent " + result);
    }
}



