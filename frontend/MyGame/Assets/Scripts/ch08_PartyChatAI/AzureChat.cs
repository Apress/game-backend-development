using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using NativeWebSocket;

public class AzureChat : MonoBehaviour
{

    WebSocket websocket;
    public bool doTranslateMessage { get; set; }

    private bool isConnected = false;

    public void ConnectToChat()
    {
        ConnectToWebSocket(GetClientAccessUrl());
    }

    public string GetClientAccessUrl()
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://shared-apim.azure-api.net/chat/Subscribe");
        request.Method = "GET";

        request.Headers.Add("Authorization", "Bearer " + GetComponent<AzureSettings>().token);

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string result = reader.ReadToEnd();

        var accessData = JsonConvert.DeserializeObject<AccessData>(result);
        return accessData.url;
    }

    void Update()
    {

#if !UNITY_WEBGL || UNITY_EDITOR
        if (isConnected)
            websocket.DispatchMessageQueue();
#endif
    }

    async void ConnectToWebSocket(string clientAccessUrl)
    {
        websocket = new WebSocket(clientAccessUrl);

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
            isConnected = true;
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
            isConnected = false;
        };

        websocket.OnMessage += (bytes) =>
        {
            ChatData chatData = JsonConvert.DeserializeObject<ChatData>(System.Text.Encoding.UTF8.GetString(bytes));

            if (doTranslateMessage)
            {
                string json = TranslateMessage(chatData.Message);
                Root[] azureTranslatorResponse = JsonConvert.DeserializeObject<Root[]>(json);
                chatData.Message = azureTranslatorResponse[0].translations[0].text;
            }

            ChatUI.UpdateTextArea(chatData.DisplayName, chatData.Message);

        };

        await websocket.Connect();
    }

    public async void SendChatMessage(string message)
    {
        string jsonMessage = "{\"DisplayName\": \"" + this.gameObject.GetComponent<AzureSettings>().displayName + "\", \"Message\": \"" + message + "\"}";
        await websocket.SendText(jsonMessage);
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }

    private string TranslateMessage(string message)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://shared-apim.azure-api.net/chat/Translate?to=en");
        request.Method = "POST";

        request.Headers.Add("Authorization", "Bearer " + GetComponent<AzureSettings>().token);

        byte[] data = Encoding.ASCII.GetBytes("[{\"text\": \"" + message + "\"}]");
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


}

public class ChatData
{
    public string DisplayName { get; set; }
    public string Message { get; set; }

}

public class AccessData
{
    public string baseUrl { get; set; }
    public string url { get; set; }
    public string accessToken { get; set; }

}

public class DetectedLanguage
{
    public string language { get; set; }
    public double score { get; set; }
}

public class Translation
{
    public string text { get; set; }
    public string to { get; set; }
}

public class Root
{
    public DetectedLanguage detectedLanguage { get; set; }
    public List<Translation> translations { get; set; }
}