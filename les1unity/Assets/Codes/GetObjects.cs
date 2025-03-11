using System.Collections.Generic;
using UnityEngine;

public class GetObjects : MonoBehaviour
{
    public Object2DApiClient object2DApiClient; // API client for fetching objects

    // ✅ Fetch Objects when viewing an environment
    public async void ReadObject2Ds(string environmentId)
    {
        Debug.Log($"📡 Fetching objects for environment ID: {environmentId}");

        IWebRequestReponse webRequestResponse = await object2DApiClient.ReadObject2Ds(environmentId);

        switch (webRequestResponse)
        {
            case WebRequestData<List<Object2D>> dataResponse:
                Debug.Log($"✅ {dataResponse.Data.Count} objects found in environment {environmentId}");
                DisplayObjects(dataResponse.Data);
                break;
            case WebRequestError errorResponse:
                Debug.LogError($"❌ Error fetching objects: {errorResponse.ErrorMessage}");
                break;
            default:
                throw new System.NotImplementedException();
        }
    }

    // ✅ Display objects (Temporary - Implement UI logic later)
    private void DisplayObjects(List<Object2D> objects)
    {
        foreach (var obj in objects)
        {
            Debug.Log($"📦 Object: {obj.prefabId}, Position: ({obj.positionX}, {obj.positionY})");
        }
    }
}