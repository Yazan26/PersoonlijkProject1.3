using System;
using UnityEngine;

public class DraggingObject2D : MonoBehaviour
{
    public ObjectManager objectManager;
    public bool isDragging = false;
    public Object2DApiClient apiClient;
    private string savedId = "";
    private void Start()
    {
      
    }

    public void Update()
    {
        if (isDragging)
            this.transform.position = GetMousePosition();
        if (Input.GetMouseButtonDown(1)) // Rechtermuisknop
        {
            if (IsMouseOverThisObject() && !isDragging && savedId != "")
            {
                DeleteObject();
            }
        }

    }
    public void SetSavedId(string id)
    {
        savedId = id;
    }
    private void OnMouseUpAsButton()
    {
        isDragging = !isDragging;

        if (!isDragging)
        {
            objectManager.ShowMenu();
            SavePlacedObject();
        }
    }

    private async void SavePlacedObject()
    {
        string environmentId = PlayerPrefs.GetString("SelectedWorldId");

        Object2D object2D = new Object2D
        {
            environmentId = environmentId,
            prefabId = gameObject.name,
            positionX = transform.position.x,
            positionY = transform.position.y,
            scaleX = transform.localScale.x,
            scaleY = transform.localScale.y,
            rotationZ = transform.rotation.eulerAngles.z,
            sortingLayer = GetComponentInChildren<SpriteRenderer>()?.sortingOrder ?? 0
        };

        if (apiClient == null)
        {
            Debug.LogError("❌ apiClient is null! Is het wel toegewezen via ObjectManager?");
            return;
        }

        string jsonToPost = JsonUtility.ToJson(object2D);
        Debug.Log("📤 POST JSON:\n" + jsonToPost);

        var response = await apiClient.CreateObject2D(object2D);

        if (response is WebRequestData<Object2D> objData)
        {
            savedId = objData.Data.id;
            Debug.Log($"💾 Object opgeslagen met ID: {savedId}");

            string jsonResponse = JsonUtility.ToJson(objData.Data, true);
            Debug.Log("📥 RESPONSE JSON:\n" + jsonResponse);
        }
        else
        {
            Debug.LogError("❌ Object is niet opgeslagen, geen geldig response!");
        }
    }


    private async void DeleteObject()
    {
        var response = await apiClient.DeleteObject2D(savedId);
        if (response is WebRequestData<string> or WebRequestData<object>)
        {
            Debug.Log("🗑️ Object verwijderd!");
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("❌ Verwijderen mislukt!");
        }
    }
    

    private Vector3 GetMousePosition()
    {
        Vector3 positionInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        positionInWorld.z = 0;
        return positionInWorld;
    }
    private bool IsMouseOverThisObject()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mousePos);

        return hit != null && hit.gameObject == this.gameObject;
    }

}