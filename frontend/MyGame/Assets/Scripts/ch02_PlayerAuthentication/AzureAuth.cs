using UnityEngine;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

public class AzureAuth : MonoBehaviour
{
    private string authorizationEndpoint = "";
    private string tokenEndpoint = "";
    private string clientID = "";
    private string clientSecret = "";
    private string redirectURI = "http://localhost:56789/";

    GameObject azure;

    private void Start()
    {
        azure = GameObject.Find("Azure");
    }
    public async void LoginWithAzure()
    {
        string encodedToken = await GetToken();
        azure.GetComponent<AzureSettings>().token = encodedToken;

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadToken(encodedToken) as JwtSecurityToken;
        var name = token.Claims.First(claim => claim.Type == "name").Value;
        azure.GetComponent<AzureSettings>().displayName = name;

        var playerID = token.Claims.First(claim => claim.Type == "sub").Value;
        azure.GetComponent<AzureSettings>().playerID = playerID;

    }

    private async Task<string> GetToken()
    {
        var httpListener = new HttpListener();
        httpListener.Prefixes.Add(redirectURI);
        httpListener.Start();

        string authURL = string.Format("{0}&client_id={1}&redirect_uri={2}&scope=openid%20profile&response_type=code",
            authorizationEndpoint,
            clientID,
            redirectURI);

        Application.OpenURL(authURL);

        HttpListenerContext context = await httpListener.GetContextAsync();

        string authorizationCode = context.Request.QueryString.Get("code");

        byte[] buffer = Encoding.UTF8.GetBytes("<html><body><script>window.close();</script></body></html>");
        await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);

        context.Response.OutputStream.Close();
        httpListener.Stop();

        string tokenRequestData = string.Format("code={0}&client_id={1}&client_secret={2}&redirect_uri={3}&grant_type=authorization_code",
            authorizationCode,
            clientID,
            clientSecret,
            redirectURI);

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(tokenEndpoint);
        request.Method = "POST";

        byte[] data = Encoding.ASCII.GetBytes(tokenRequestData);
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = data.Length;

        Stream requestStream = request.GetRequestStream();
        requestStream.Write(data, 0, data.Length);
        requestStream.Close();

        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());
        string result = reader.ReadToEnd();

        TokenEndpointResponse tokenEndpointResponse = JsonConvert.DeserializeObject<TokenEndpointResponse>(result);

        return tokenEndpointResponse.id_token;
    }

}

public class TokenEndpointResponse
{
    public string id_token;
}


