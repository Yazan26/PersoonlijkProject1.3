using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ObjectManager : MonoBehaviour
{
    // Menu om objecten vanuit te plaatsen
    public GameObject UISideMenu;
    // Lijst met objecten die geplaatst kunnen worden die overeenkomen met de prefabs in de prefabs map
    public List<GameObject> prefabObjects;
    public Object2DApiClient objectApiClient;

    public Button Backbutton;
    // Lijst met objecten die geplaatst zijn in de wereld
    private List<GameObject> placedObjects;

    // Methode om een nieuw 2D object te plaatsen
    public void PlaceNewObject2D(int index)
    {
        UISideMenu.SetActive(false);

        GameObject instanceOfPrefab = Instantiate(prefabObjects[index], Vector3.zero, Quaternion.identity);

        DraggingObject2D objects2D = instanceOfPrefab.GetComponent<DraggingObject2D>();

        objects2D.objectManager = this;
        objects2D.apiClient = objectApiClient; // ✅ hier geef je de API client mee

        objects2D.isDragging = true;
    }

    public void Start()
    {
        Backbutton.onClick.AddListener(BackToWorldCreator);
    }

    public void BackToWorldCreator()
    {
        SceneManager.LoadScene("WorldSelector");
    }
    
    // Methode om het menu te tonen
    public void ShowMenu()
    {
        UISideMenu.SetActive(true);
    }
    
    private async System.Threading.Tasks.Task LoadExistingObjects()
    {
        string environmentId = PlayerPrefs.GetString("SelectedWorldId");

        var response = await objectApiClient.ReadObject2Ds(environmentId);

        if (response is WebRequestData<List<Object2D>> data)
        {
            Debug.Log($"📦 {data.Data.Count} objecten geladen");

            foreach (Object2D obj in data.Data)
            {
                SpawnObject(obj);
            }
        }
        else
        {
            Debug.LogError("❌ Kon objecten niet ophalen uit de database.");
        }
    }

    private void SpawnObject(Object2D objData)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/" + objData.prefabId);
        if (prefab == null)
        {
            Debug.LogError($"❌ Prefab '{objData.prefabId}' niet gevonden in Resources/Prefabs/");
            return;
        }

        GameObject instance = Instantiate(prefab, new Vector3(objData.positionX, objData.positionY, 0), Quaternion.Euler(0, 0, objData.rotationZ));
        instance.transform.localScale = new Vector3(objData.scaleX, objData.scaleY, 1);

        SpriteRenderer renderer = instance.GetComponentInChildren<SpriteRenderer>();
        if (renderer != null)
            renderer.sortingOrder = objData.sortingLayer;

   

        // ✅ Koppel ID terug aan DraggingObject2D zodat Delete werkt
        DraggingObject2D dragScript = instance.GetComponent<DraggingObject2D>();
        if (dragScript != null)
        {
            dragScript.SetSavedId(objData.id);
        }

        Debug.Log($"✅ Object geplaatst: {objData.prefabId} @ {objData.positionX}, {objData.positionY}");
    }


    // Methode om de huidige sc�ne te resetten
    public void Reset()
    {
        // Laad de huidige sc�ne opnieuw
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
