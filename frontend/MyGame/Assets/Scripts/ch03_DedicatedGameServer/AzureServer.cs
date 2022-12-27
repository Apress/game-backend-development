using UnityEngine;
using Agones;

public class AzureServer : MonoBehaviour
{
    private AgonesSdk agones = null;

    async void Start()
    {
        agones = GetComponent<AgonesSdk>();
        bool ok = await agones.Connect();
        if (ok)
        {
            Debug.Log("Server is connected.");
        }
        else
        {
            Debug.Log("Server failed to connect.");
            Application.Quit();
        }
        ok = await agones.Ready();
        if (ok)
        {
            Debug.Log("Server is ready.");
        }
        else
        {
            Debug.Log("Server ready failed.");
            Application.Quit();
        }
    }
}


