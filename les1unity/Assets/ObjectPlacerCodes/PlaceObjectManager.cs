using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class PlaceObjectManager : MonoBehaviour
{
    public GameObject[] objectPrefabs; // Assign in Inspector
    public Transform worldContainer; // Parent for placed objects
    public Object2DApiClient object2DApiClient;

    private string selectedWorldId;
    private GameObject selectedObjectPrefab;

    void Start()
    {
        selectedWorldId = PlayerPrefs.GetString("SelectedWorldId", "");
        if (string.IsNullOrEmpty(selectedWorldId))
        {
            Debug.LogError("❌ No world selected!");
            return;
        }
    }

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
            prefabId = obj.name,
            positionX = obj.transform.position.x,
            positionY = obj.transform.position.y,
            environmentId = selectedWorldId
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
}
