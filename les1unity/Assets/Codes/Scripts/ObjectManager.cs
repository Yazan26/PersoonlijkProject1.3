using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ObjectManager : MonoBehaviour
{
    public GameObject UISideMenu;
    public List<GameObject> prefabObjects; // prefab must contain Object2DInstance
    public Object2DApiClient objectApiClient;

    public void ShowMenu() => UISideMenu.SetActive(true);

    private async void Start()
    {
        await LoadExistingObjects();
    }
    

    public void PlaceNewObject2D(int index)
    {
        UISideMenu.SetActive(false);

        GameObject prefab = prefabObjects[index];
        GameObject instance = Instantiate(prefab, Vector3.zero, Quaternion.identity);

        var logic = instance.GetComponent<DraggingObject2D>();
        logic.Initialize(this, objectApiClient);
        logic.isDragging = true;
    }

    private async Task LoadExistingObjects()
    {
        string environmentId = PlayerPrefs.GetString("SelectedWorldId");

        if (string.IsNullOrEmpty(environmentId))
        {
            Debug.LogError("⚠️ Geen SelectedWorldId gevonden.");
            return;
        }

        var response = await objectApiClient.ReadObject2Ds(environmentId);

        if (response is WebRequestData<List<Object2D>> data)
        {
            foreach (Object2D obj in data.Data)
                SpawnFromData(obj);
        }
        else
        {
            Debug.LogError("❌ Objecten konden niet worden opgehaald.");
        }
    }

    private void SpawnFromData(Object2D data)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/" + data.prefabId.Replace("(Clone)", ""));
        if (prefab == null)
        {
            Debug.LogError($"❌ Prefab '{data.prefabId}' niet gevonden.");
            return;
        }

        GameObject instance = Instantiate(prefab, new Vector3(data.positionX, data.positionY, 0), Quaternion.Euler(0, 0, data.rotationZ));
        instance.transform.localScale = new Vector3(data.scaleX, data.scaleY, 1);

        SpriteRenderer renderer = instance.GetComponentInChildren<SpriteRenderer>();
        if (renderer != null)
            renderer.sortingOrder = data.sortingLayer;

        DraggingObject2D logic = instance.GetComponent<DraggingObject2D>();
        logic.Initialize(this, objectApiClient);
        logic.SetObjectData(data);

        Debug.Log($"✅ Object geladen: {data.prefabId} ({data.id})");
    }
    public async void DeleteAllObjects()
    {
        foreach (var obj in FindObjectsOfType<DraggingObject2D>())
        {
            if (!string.IsNullOrEmpty(obj.objectData.id))
            {
                await obj.apiClient.DeleteObject2D(obj.objectData.id);
            }
            Destroy(obj.gameObject);
        }

        Debug.Log("🔥 Alle objecten verwijderd!");
    }

}
