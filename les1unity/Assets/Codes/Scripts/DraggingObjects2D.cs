using System;
using UnityEngine;

public class DraggingObject2D : MonoBehaviour
{
    public Object2D objectData;
    public Object2DApiClient apiClient;
    public string environmentId;
    public bool isDragging = false;
    public ObjectManager objectManager;

    private void Start()
    {
        // Zorg dat het object weet in welke wereld het zit
        environmentId = PlayerPrefs.GetString("SelectedWorldId");
        objectData.Environment2DID = environmentId;
    }

    private void Update()
    {
        if (isDragging)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
            // ↺ Horizontaal roteren met Q en E
            if (Input.GetKeyDown(KeyCode.Q))
            {
                transform.Rotate(0f, 0f, 15f); // linksom
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                transform.Rotate(0f, 0f, -15f); // rechtsom
            }
            // 🔃 Verticale flip met W en S (via scale)
            if (Input.GetKeyDown(KeyCode.W))
            {
                transform.localScale = new Vector3(transform.localScale.x, Mathf.Abs(transform.localScale.y), transform.localScale.z);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                transform.localScale = new Vector3(transform.localScale.x, -Mathf.Abs(transform.localScale.y), transform.localScale.z);
            }
        }

        if (Input.GetMouseButtonDown(1)) // rechtermuisklik om te verwijderen
        {
            if (IsMouseOverThisObject() && !isDragging && !string.IsNullOrEmpty(objectData.Id))
            {
                DeleteObject();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            objectManager.DeleteAllObjects();
        }

    }


    private void OnMouseUpAsButton()
    {
        isDragging = !isDragging;

        objectManager.ShowMenu();

        objectData.PrefabId = gameObject.name.Replace("(Clone)", "");
        objectData.PositionX = transform.position.x;
        objectData.PositionY = transform.position.y;
        objectData.ScaleX = transform.localScale.x;
        objectData.ScaleY = transform.localScale.y;
        objectData.RotationZ = transform.rotation.eulerAngles.z;
        objectData.SortingLayer = GetComponentInChildren<SpriteRenderer>()?.sortingOrder ?? 0;

        if (!isDragging)
        {
            if (string.IsNullOrEmpty(objectData.Id))
                CreateObject();
            else
                UpdateObject();
        }
    }

    public void Initialize(ObjectManager manager, Object2DApiClient client)
    {
        objectManager = manager;
        apiClient = client;
    }

    public void SetObjectData(Object2D data)
    {
        objectData = data;
        environmentId = data.Environment2DID;
    }

    private async void CreateObject()
    {
        var response = await apiClient.CreateObject2D(objectData);
        if (response is WebRequestData<Object2D> obj)
        {
            objectData.Id = obj.Data.Id;
            Debug.Log($"✅ Object aangemaakt met id: {objectData.Id}");
        }
        else
        {
            Debug.LogError("❌ Kon object niet aanmaken.");
        }
    }

    private async void UpdateObject()
    {
        var response = await apiClient.UpdateObject2D(objectData);
        if (response is WebRequestData<string>)
        {
            Debug.Log("🔄 Object geüpdatet!");
        }
        else
        {
            Debug.LogError("❌ Kon object niet updaten.");
        }
    }

    private async void DeleteObject()
    {
        var response = await apiClient.DeleteObject2D(objectData.Id);
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

    private bool IsMouseOverThisObject()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                return true;
            }
        }

        return false;
    }

    
    public async void DeleteAllObjects()
    {
        foreach (var obj in FindObjectsOfType<DraggingObject2D>())
        {
            if (!string.IsNullOrEmpty(obj.objectData.Id))
            {
                await obj.apiClient.DeleteObject2D(obj.objectData.Id);
            }
            Destroy(obj.gameObject);
        }

        Debug.Log("🔥 Alle objecten verwijderd!");
    }

}