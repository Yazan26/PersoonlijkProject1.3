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
        json = RemoveIdFieldFromJson(json); // als je dat nog gebruikt
        Debug.Log("📤 POST JSON:\n" + json);

        IWebRequestReponse response = await webClient.SendPostRequest(route, json);

        Debug.Log($"📬 RESPONSE TYPE: {response?.GetType().Name ?? "null"}");

        if (response == null)
        {
            Debug.LogError("❌ Response is null!");
        }
        else if (response is WebRequestData<string> str)
        {
            Debug.Log("📥 WebRequestData<string> ontvangen:\n" + str.Data);
        }
        else if (response is WebRequestData<object> obj)
        {
            Debug.Log("📦 WebRequestData<object> ontvangen:\n" + obj.Data);
        }
        else
        {
            Debug.LogWarning("⚠️ Onbekend response type ontvangen:\n" + response.ToString());
        }
        return ParseObject2DResponse(response);
    }



    private string RemoveIdFieldFromJson(string json)
    {
        json = json.Replace("\"Id\":\"\",", "");
        json = json.Replace(",\"Id\":\"\"", "");
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
            .Replace("\"Environment2DID\"", "\"environment2DID\"")
            .Replace("\"PrefabId\"", "\"prefabId\"")
            .Replace("\"PositionX\"", "\"positionX\"")
            .Replace("\"PositionY\"", "\"positionY\"")
            .Replace("\"ScaleX\"", "\"scaleX\"")
            .Replace("\"ScaleY\"", "\"scaleY\"")
            .Replace("\"RotationZ\"", "\"rotationZ\"")
            .Replace("\"SortingLayer\"", "\"sortingLayer\"")
            .Replace("\"UserID\"", "\"userID\"");
    }





    private IWebRequestReponse ParseObject2DListResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("📥 Response data raw:\n" + data.Data);

                // Fix casing voor hele lijst
                string fixedJson = FixJsonCasing(data.Data);

                // Deserialize naar Object2D array (let op!)
                List<Object2D> objects = JsonHelper.ParseJsonArray<Object2D>(fixedJson);
                return new WebRequestData<List<Object2D>>(objects);

            default:
                Debug.LogError("❌ Onverwacht response type ontvangen bij GET Object2Ds.");
                return webRequestResponse;
        }
    }

}