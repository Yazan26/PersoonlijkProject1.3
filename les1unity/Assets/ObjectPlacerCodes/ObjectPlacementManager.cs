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

    void Start()
    {
        selectedWorldId = PlayerPrefs.GetString("SelectedWorldId", "");
        userId = PlayerPrefs.GetString("UserId", "");

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

            GameObject newObject = Instantiate(selectedObjectPrefab, mousePosition, Quaternion.identity, worldContainer);
            SaveObjectToDatabase(newObject);
        }
    }

    async void SaveObjectToDatabase(GameObject obj)
    {
        Object2D newObject = new Object2D
        {
            id = System.Guid.NewGuid().ToString(), // Generate a new GUID
            prefabId = obj.name,
            positionX = obj.transform.position.x,
            positionY = obj.transform.position.y,
            scaleX = obj.transform.localScale.x,
            scaleY = obj.transform.localScale.y,
            rotationZ = obj.transform.rotation.eulerAngles.z,
            sortingLayer = 0, // Default sorting layer
            environmentId =  PlayerPrefs.GetString("SelectedWorldId", "")
        };

        IWebRequestReponse response = await object2DApiClient.CreateObject2D(newObject);

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
        string worldId = PlayerPrefs.GetString("SelectedWorldId", "");
    
        if (string.IsNullOrEmpty(worldId))
        {
            Debug.LogError("❌ SelectedWorldId is missing! Make sure it is set in PlayerPrefs.");
            return;
        }

        Debug.Log($"📡 Loading objects for world: {worldId}"); // ✅ Debugging

        IWebRequestReponse response = await object2DApiClient.ReadObject2Ds(worldId);

        if (response is WebRequestData<List<Object2D>> dataResponse)
        {
            Debug.Log($"✅ Loaded {dataResponse.Data.Count} objects!"); // ✅ Debugging
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
