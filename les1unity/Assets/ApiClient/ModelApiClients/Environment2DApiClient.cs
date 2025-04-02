using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Environment2DApiClient : MonoBehaviour
{
    public WebClient webClient;

    public async Awaitable<IWebRequestReponse> ReadEnvironment2Ds(string userId)
    {
        string route = $"/Environment2D/user/{userId}"; // ✅ Fetch only the user's worlds

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequest(route);
        return ParseEnvironment2DListResponse(webRequestResponse);
    }

    
    public async Awaitable<IWebRequestReponse> ReadSingleEnvironment(string environmentId)
    {
        string route = $"/Environment2D/world/{environmentId}"; // ✅ Correct API endpoint

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequest(route);
        return ParseEnvironment2DResponse(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> CreateEnvironment(Environment2D environment)
    {
        string route = "/Environment2D/Createworld";
        string data = JsonUtility.ToJson(environment);
        Debug.Log("📡 Verzonden JSON: " + data); // ✅ Log om te checken TBREMOVED


        IWebRequestReponse webRequestResponse = await webClient.SendPostRequest(route, data);
        return ParseEnvironment2DResponse(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> DeleteEnvironment(string environmentId)
    {
        string route = "/Environment2D/" + environmentId;
        return await webClient.SendDeleteRequest(route);
    }

    private IWebRequestReponse ParseEnvironment2DResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data);
                Environment2D environment = JsonUtility.FromJson<Environment2D>(data.Data);
                WebRequestData<Environment2D> parsedWebRequestData = new WebRequestData<Environment2D>(environment);
                return parsedWebRequestData;
            default:
                return webRequestResponse;
        }
    }

    private IWebRequestReponse ParseEnvironment2DListResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data);
                List<Environment2D> environment2Ds = JsonHelper.ParseJsonArray<Environment2D>(data.Data);
                WebRequestData<List<Environment2D>> parsedWebRequestData = new WebRequestData<List<Environment2D>>(environment2Ds);
                return parsedWebRequestData;
            default:
                return webRequestResponse;
        }
    }

}

