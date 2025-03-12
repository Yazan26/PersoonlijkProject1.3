using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ObjectPlacementManager : MonoBehaviour
{
    public GameObject[] objectPrefabs; // Assign prefabs in Inspector
    public Transform worldContainer; // Parent for placed objects
    public Object2DApiClient object2DApiClient;

    private GameObject selectedObjectPrefab;
    private string selectedWorldId;
    private string userId;
    private string environmentId;

    void Start()
    {
        selectedWorldId = PlayerPrefs.GetString("SelectedWorldId", "");
        userId = PlayerPrefs.GetString("UserId", "");
        environmentId = selectedWorldId; // Gebruik `selectedWorldId` als `environmentId`

        if (string.IsNullOrEmpty(selectedWorldId) || string.IsNullOrEmpty(userId))
        {
            Debug.LogError("❌ No world or user selected!");
            return;
        }

        LoadObjects(); // ✅ Load existing objects when entering the scene
    }

    // ✅ UI calls this method when an object is selected
    public void SelectObject(int index)
    {
        if (index >= 0 && index < objectPrefabs.Length)
        {
            selectedObjectPrefab = objectPrefabs[index];
            Debug.Log($"✅ Selected Object: {selectedObjectPrefab.name}");
        }
    }

    void Update()
    {
        if (selectedObjectPrefab != null && Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return; // Avoid placing on UI

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; // Ensure 2D placement

            GameObject newObj = Instantiate(selectedObjectPrefab, mousePosition, Quaternion.identity, worldContainer);
            SaveObjectToDatabase(newObj);
        }
    }

    async void SaveObjectToDatabase(GameObject obj)
    {
        // ✅ Check dat UserID en EnvironmentID correct zijn
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(environmentId))
        {
            Debug.LogError("❌ Cannot save object: UserID or EnvironmentID is missing!");
            return;
        }

        Object2D newObject = new Object2D
        {
            id = Guid.NewGuid().ToString(), // ✅ Converteer naar string
            userID = userId, // ✅ UserID als string
            environmentId = environmentId, // ✅ Zorg dat dit de juiste worldId is!
            prefabId = obj.name, // ✅ Gebruik de naam van de prefab
            positionX = obj.transform.position.x,
            positionY = obj.transform.position.y,
            scaleX = obj.transform.localScale.x,
            scaleY = obj.transform.localScale.y,
            rotationZ = obj.transform.rotation.eulerAngles.z,
            sortingLayer = 0
        };

        Debug.Log($"📡 Sending Object2D to API: {JsonUtility.ToJson(newObject)}");

        IWebRequestReponse response = await object2DApiClient.CreateObject2D(newObject, userId);

        if (response is WebRequestData<Object2D>)
        {
            Debug.Log("✅ Object saved to database!");
        }
        else if (response is WebRequestError errorResponse)
        {
            Debug.LogError($"❌ Failed to save object: {errorResponse.ErrorMessage}");
        }
    }

    // ✅ Load saved objects when the scene starts
    public async void LoadObjects()
    {
        if (string.IsNullOrEmpty(selectedWorldId))
        {
            Debug.LogError("❌ SelectedWorldId is missing! Make sure it is set in PlayerPrefs.");
            return;
        }

        Debug.Log($"📡 Loading objects for world: {selectedWorldId}");

        IWebRequestReponse response = await object2DApiClient.ReadObject2Ds(selectedWorldId);

        if (response is WebRequestData<List<Object2D>> dataResponse)
        {
            Debug.Log($"✅ Loaded {dataResponse.Data.Count} objects!");
            InstantiateObjects(dataResponse.Data);
        }
        else if (response is WebRequestError errorResponse)
        {
            Debug.LogError($"❌ Error loading objects: {errorResponse.ErrorMessage}");
        }
    }

    void InstantiateObjects(List<Object2D> objects)
    {
        foreach (var obj in objects)
        {
            GameObject prefab = System.Array.Find(objectPrefabs, p => p.name == obj.prefabId);
            if (prefab != null)
            {
                GameObject newObj = Instantiate(prefab, new Vector3(obj.positionX, obj.positionY, 0), Quaternion.identity, worldContainer);
                newObj.transform.localScale = new Vector3(obj.scaleX, obj.scaleY, 1);
                newObj.transform.rotation = Quaternion.Euler(0, 0, obj.rotationZ);
            }
            else
            {
                Debug.LogError($"❌ Prefab {obj.prefabId} not found!");
            }
        }
    }
}
