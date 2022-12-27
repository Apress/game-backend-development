using UnityEngine;
using System.Net;
using System.IO;
using System.Text;
using System.Collections;
using Mirror;
using kcp2k;

public class AzureMatchmaking : MonoBehaviour
{
    private IEnumerator coroutine;
    private string ticketID;

    GameObject azureClient;


    public void StartAzureMatchmaking()
    {
        azureClient = GameObject.Find("AzureClient");

        ticketID = CreateTicket("advanced");
        Debug.Log("New matchmaking ticket submitted, TicketID : " + ticketID);

        coroutine = WaitAndGetMatchingTicket(6.0f);
        StartCoroutine(coroutine);
    }

    string CreateTicket(string skill_level)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://matchmaker-apim.azure-api.net/Ticket");
        request.Method = "POST";

        request.Headers.Add("Authorization", "Bearer " + GetComponent<AzureSettings>().token);

        byte[] data = Encoding.ASCII.GetBytes("{Skill_level: '" + skill_level + "'}");
        request.ContentType = "application/json";
        request.ContentLength = data.Length;

        Stream requestStream = request.GetRequestStream();
        requestStream.Write(data, 0, data.Length);
        requestStream.Close();

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string result = reader.ReadToEnd();
        return result;
    }

    string DeleteTicket(string ticketID)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://matchmaker-apim.azure-api.net/Ticket?TicketID=" + ticketID);
        request.Method = "DELETE";

        request.Headers.Add("Authorization", "Bearer " + GetComponent<AzureSettings>().token);

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string result = reader.ReadToEnd();

        return result;
    }
    string GetServerAddress(string ticketID)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://matchmaker-apim.azure-api.net/Ticket?TicketID=" + ticketID);
        request.Method = "GET";

        request.Headers.Add("Authorization", "Bearer " + GetComponent<AzureSettings>().token);

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string result = reader.ReadToEnd();
        return result;
    }
    private IEnumerator WaitAndGetMatchingTicket(float waitTime)
    {
        while (true)
        {
            Debug.Log("Waiting for mathing player...");
            GetMatchingTicket();
            yield return new WaitForSeconds(waitTime);
        }
    }
    void GetMatchingTicket()
    {
        string serverAddress = GetServerAddress(ticketID);
        if (!serverAddress.Equals("not available") && (!serverAddress.Equals("TicketID is not available.")))
        {
            Debug.Log("Matched! Server Address: " + serverAddress);
            StopCoroutine(coroutine);
            StartAzureClient(serverAddress);
        }
    }

    void StartAzureClient(string serverAddress)
    {
        string serverIP = serverAddress.Substring(0, serverAddress.IndexOf(':'));
        string serverPort = serverAddress.Substring(serverAddress.IndexOf(':') + 1, 4);
        azureClient.GetComponent<NetworkManager>().networkAddress = serverIP;
        azureClient.GetComponent<KcpTransport>().Port = (ushort)int.Parse(serverPort);
        azureClient.GetComponent<NetworkManager>().StartClient();
    }
    public void CancelAzureMatchmaking()
    {
        Debug.Log(DeleteTicket(ticketID));
        StopCoroutine(coroutine);
    }
    void OnApplicationQuit()
    {
        CancelAzureMatchmaking();
        Debug.Log("Game ending after " + Time.time + " seconds");
    }
}
