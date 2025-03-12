using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Object2DApiClient : MonoBehaviour
{
    public WebClient webClient;

    // ✅ Fetch objects for a specific world belonging to the authenticated user
    public async Task<IWebRequestReponse> ReadObject2Ds(string worldId)
    {
        string route = $"/Object2D/user/world/{worldId}"; // ✅ Corrected route

        IWebRequestReponse webRequestResponse = await webClient.SendGetRequest(route);
        return ParseObject2DListResponse(webRequestResponse);
    }

    // ✅ Create a new Object2D
    public async Task<IWebRequestReponse> CreateObject2D(Object2D object2D, string userId)
    {
        object2D.userID = userId; // ✅ Zet de juiste UserID voordat je het verstuurt!
        string route = "/Object2D"; // ✅ Matches [HttpPost] in API
        string data = JsonUtility.ToJson(object2D);

        IWebRequestReponse webRequestResponse = await webClient.SendPostRequest(route, data);
        return ParseObject2DResponse(webRequestResponse);
    }

    // ✅ Delete an Object2D by ID
    public async Task<IWebRequestReponse> DeleteObject2D(Guid objectId)
    {
        string route = $"/Object2D/{objectId}"; // ✅ Matches [HttpDelete] in API

        return await webClient.SendDeleteRequest(route);
    }

    // ✅ Parse individual Object2D response
    private IWebRequestReponse ParseObject2DResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data);
                Object2D object2D = JsonUtility.FromJson<Object2D>(data.Data);
                return new WebRequestData<Object2D>(object2D);
            default:
                return webRequestResponse;
        }
    }

    // ✅ Parse list of Object2D responses
    private IWebRequestReponse ParseObject2DListResponse(IWebRequestReponse webRequestResponse)
    {
        switch (webRequestResponse)
        {
            case WebRequestData<string> data:
                Debug.Log("Response data raw: " + data.Data);
                List<Object2D> objects = JsonHelper.ParseJsonArray<Object2D>(data.Data);
                return new WebRequestData<List<Object2D>>(objects);
            default:
                return webRequestResponse;
        }
    }
}
