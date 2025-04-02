using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;

public class GetObjects : MonoBehaviour
{
    public Object2DApiClient object2DApiClient;
    public Environment2DApiClient environment2DApiClient; // Added to fetch world size
    public Transform worldContainer;
    public GameObject[] objectPrefabs; // Assign in Inspector

    void Start()
    {
        string worldId = PlayerPrefs.GetString("SelectedWorldId", "");
        if (!string.IsNullOrEmpty(worldId))
        {
            SetWorldSize(worldId);
            ReadObject2Ds(worldId);
        }
    }

    // ✅ Fetch World Size and Adjust WorldContainer
    public async void SetWorldSize(string worldId)
    {
        IWebRequestReponse response = await environment2DApiClient.ReadSingleEnvironment(worldId);

        if (response is WebRequestData<Environment2D> dataResponse)
        {
            Environment2D worldData = dataResponse.Data;

            // ✅ Set world size correctly
            worldContainer.localScale = new Vector3(worldData.MaxWidth, worldData.MaxHeight, 1);

            Debug.Log($"🌍 World Size Set: {worldData.MaxWidth}x{worldData.MaxHeight}");
        }
        else if (response is WebRequestError errorResponse)
        {
            Debug.LogError($"❌ Failed to fetch world size: {errorResponse.ErrorMessage}");
        }
    }
    public async void ReadObject2Ds(string environmentId)
    {
        Debug.Log($"📡 Fetching objects for environment ID: {environmentId}");

        IWebRequestReponse webRequestResponse = await object2DApiClient.ReadObject2Ds(environmentId);

        switch (webRequestResponse)
        {
            case WebRequestData<List<Object2D>> dataResponse:
                Debug.Log($"✅ {dataResponse.Data.Count} objects found in environment {environmentId}");
                InstantiateObjects(dataResponse.Data);
                break;
            case WebRequestError errorResponse:
                Debug.LogError($"❌ Error fetching objects: {errorResponse.ErrorMessage}");
                break;
            default:
                throw new System.NotImplementedException();
        }
    }

    void InstantiateObjects(List<Object2D> objects)
    {
        foreach (var obj in objects)
        {
            GameObject prefab = System.Array.Find(objectPrefabs, p => p.name == obj.prefabId);
            if (prefab != null)
            {
                Instantiate(prefab, new Vector3(obj.positionX, obj.positionY, 0), Quaternion.identity, worldContainer);
            }
            else
            {
                Debug.LogError($"❌ Prefab {obj.prefabId} not found!");
            }
        }
    }
}
