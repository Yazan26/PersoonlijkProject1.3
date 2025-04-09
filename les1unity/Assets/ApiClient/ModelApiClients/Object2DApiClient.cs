using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Object2DApiClient : MonoBehaviour
{
    public WebClient webClient;

    public async Awaitable<IWebRequestReponse> ReadObject2Ds(string environmentId)
    {
        string route = "/Object2D/" + environmentId;

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequest(route);
        return ParseObject2DListResponse(webRequestResponse);
    }

    public async Awaitable<IWebRequestReponse> CreateObject2D(Object2D object2D)
    {
        string route = "/Object2D";
        string json = JsonUtility.ToJson(object2D);
        json = RemoveIdFieldFromJson(json); // 👈 hier filteren we id eruit

        Debug.Log("📤 POST JSON zonder id:\n" + json);

        IWebRequestReponse webRequestResponse = await webClient.SendPostRequest(route, json);
        return ParseObject2DResponse(webRequestResponse);
    }

    private string RemoveIdFieldFromJson(string json)
    {
        json = json.Replace("\"id\":\"\",", "");
        json = json.Replace(",\"id\":\"\"", "");
        return json;
    }

    public async Awaitable<IWebRequestReponse> UpdateObject2D(Object2D object2D)
    {
        string route = "/Object2D/" + object2D.id;
        string data = JsonUtility.ToJson(object2D);

        return await webClient.SendPutRequest(route, data);
    }
    
    public async Awaitable<IWebRequestReponse> DeleteObject2D(string objectId)
    {
        string route = "/Object2D/" + objectId; 
        return await webClient.SendDeleteRequest(route);
    }

    private IWebRequestReponse ParseObject2DResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("📥 Response data raw:\n" + data.Data);
                string fixedJson = FixJsonCasing(data.Data);
                Object2D object2D = JsonUtility.FromJson<Object2D>(fixedJson);
                return new WebRequestData<Object2D>(object2D);
            default:
                return webRequestResponse;
        }
    }
    private string FixJsonCasing(string json)
    {
        return json
            .Replace("\"Id\"", "\"id\"")
            .Replace("\"Environment2DID\"", "\"environmentId\"")
            .Replace("\"PrefabId\"", "\"prefabId\"")
            .Replace("\"PositionX\"", "\"positionX\"")
            .Replace("\"PositionY\"", "\"positionY\"")
            .Replace("\"ScaleX\"", "\"scaleX\"")
            .Replace("\"ScaleY\"", "\"scaleY\"")
            .Replace("\"RotationZ\"", "\"rotationZ\"")
            .Replace("\"SortingLayer\"", "\"sortingLayer\"")
            .Replace("\"UserID\"", "\"userId\""); // alleen als je ooit wil meegeven
    }



    private IWebRequestReponse ParseObject2DListResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data);
                List<Object2D> environments = JsonHelper.ParseJsonArray<Object2D>(data.Data);
                WebRequestData<List<Object2D>> parsedData = new WebRequestData<List<Object2D>>(environments);
                return parsedData;
            default:
                return webRequestResponse;
        }
    }
}