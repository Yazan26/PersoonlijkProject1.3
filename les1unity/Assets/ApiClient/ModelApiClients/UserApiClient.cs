using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class UserApiClient : MonoBehaviour
{
    public WebClient webClient;

    public async Awaitable<IWebRequestReponse> Register(User user)
    {
        string route = "/account/register";
        string data = JsonUtility.ToJson(user);

        return await webClient.SendPostRequest(route, data);
    }
    
    public async Awaitable<IWebRequestReponse> GetCurrentUser()
    {
        string route = "/me"; // ✅ Endpoint voor User ID ophalen

        IWebRequestReponse response = await webClient.SendGetRequest(route);

        if (response is WebRequestData<string> jsonResponse)
        {
            try
            {
                // ✅ Parse de JSON om alleen de userId te extraheren
                UserResponse user = JsonUtility.FromJson<UserResponse>(jsonResponse.Data);
                return new WebRequestData<string>(user.userId);
            }
            catch (Exception ex)
            {
                Debug.LogError($"❌ Fout bij JSON-parsen: {ex.Message}");
                return new WebRequestError("Fout bij verwerken van gebruikersdata.");
            }
        }

        return response;
    }

// ✅ Model voor de JSON-structuur van /me response
    [Serializable]
    public class UserResponse
    {
        public string userId;
    }


    public async Awaitable<IWebRequestReponse> Login(User user)
    {
        string route = "/account/login";
        string data = JsonUtility.ToJson(user);

        IWebRequestReponse response = await webClient.SendPostRequest(route, data);
        return ProcessLoginResponse(response);
    }

    private IWebRequestReponse ProcessLoginResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data);
                string token = JsonHelper.ExtractToken(data.Data);
                webClient.SetToken(token);
                return new WebRequestData<string>(token);
            default:
                return webRequestResponse;
        }
        
        
    }

}

