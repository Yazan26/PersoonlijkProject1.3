using System.Collections.Generic;
using UnityEngine;

public class EnvironmentLoader : MonoBehaviour
{
    public Object2DApiClient objectClient;
    public GameObject[] prefabOptions; // Dice prefabs
    public Transform worldParent;
    public ObjectManager objectManager; // om objectManager logica te hergebruiken

    private async void Start()
    {
        string environmentId = PlayerPrefs.GetString("SelectedWorldId");
        
        var response = await objectClient.ReadObject2Ds(environmentId);
        if (response is WebRequestData<List<Object2D>> objectData)
        {
            foreach (var obj in objectData.Data)
                SpawnObject(obj);
        }
        else
        {
            Debug.LogError("❌ Kon objecten niet ophalen van backend");
        }
    }

    private void SpawnObject(Object2D obj)
    {
        GameObject prefab = GetPrefabById(obj.prefabId);
        if (prefab == null)
        {
            Debug.LogWarning($"⚠️ Geen prefab gevonden met naam: {obj.prefabId}");
            return;
        }

        Vector3 pos = new Vector3(obj.positionX, obj.positionY, 0f);
        Quaternion rot = Quaternion.Euler(0, 0, obj.rotationZ);
        Vector3 scale = new Vector3(obj.scaleX, obj.scaleY, 1f);

        GameObject instance = Instantiate(prefab, pos, rot, worldParent);
        instance.transform.localScale = scale;
        instance.name = obj.prefabId;

        // Koppel interactiviteit
        DraggingObject2D drag = instance.GetComponent<DraggingObject2D>();
        drag.objectManager = objectManager;
        drag.apiClient = objectClient;
        drag.SetSavedId(obj.id);
        drag.isDragging = false;

        Debug.Log($"✅ Object geladen: {obj.prefabId}");
    }

    private GameObject GetPrefabById(string id)
    {
        foreach (var p in prefabOptions)
            if (p.name == id) return p;
        return null;
    }
}
