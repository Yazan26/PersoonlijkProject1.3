using UnityEngine;

public class DraggingObject2D : MonoBehaviour
{
    public Object2D objectData;
    public Object2DApiClient apiClient;
    public ObjectManager objectManager;
    public bool isDragging = false;

    private bool HasValidId => !string.IsNullOrEmpty(objectData?.id);

    private void Start()
    {
        objectData.environment2DID = PlayerPrefs.GetString("SelectedWorldId");
    }

    private void Update()
    {
        if (isDragging)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);

            if (Input.GetKeyDown(KeyCode.Q)) transform.Rotate(0f, 0f, 15f);
            if (Input.GetKeyDown(KeyCode.E)) transform.Rotate(0f, 0f, -15f);
            if (Input.GetKeyDown(KeyCode.W)) transform.localScale = new Vector3(transform.localScale.x, Mathf.Abs(transform.localScale.y), transform.localScale.z);
            if (Input.GetKeyDown(KeyCode.S)) transform.localScale = new Vector3(transform.localScale.x, -Mathf.Abs(transform.localScale.y), transform.localScale.z);
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (IsMouseOverThisObject() && !isDragging)
                DeleteObject();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            if (IsMouseOverThisObject() && !isDragging)
            {
                isDragging = true;
                Debug.Log($"📦 Object opgepakt (ID: {objectData.id})");
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

        objectData.prefabId = gameObject.name.Replace("(Clone)", "");
        objectData.positionX = transform.position.x;
        objectData.positionY = transform.position.y;
        objectData.scaleX = transform.localScale.x;
        objectData.scaleY = transform.localScale.y;
        objectData.rotationZ = transform.rotation.eulerAngles.z;
        objectData.sortingLayer = GetComponentInChildren<SpriteRenderer>()?.sortingOrder ?? 0;

        if (!isDragging)
        {
            if (!HasValidId)
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
    }

    private async void CreateObject()
    {
        Debug.Log("🆕 CreateObject aangeroepen");

        var response = await apiClient.CreateObject2D(objectData);

        if (response is WebRequestData<Object2D> obj && !string.IsNullOrEmpty(obj.Data.id))
        {
            objectData.id = obj.Data.id;
            Debug.Log($"✅ Object aangemaakt met ID: {objectData.id}");
        }
        else
        {
            Debug.LogError("❌ Kon object niet aanmaken (geen ID ontvangen).");
        }
    }

    private async void UpdateObject()
    {
        Debug.Log($"✏️ UpdateObject aangeroepen met ID: {objectData.id}");

        if (!HasValidId)
        {
            Debug.LogError("❌ Update gefaald, object heeft geen geldige ID!");
            return;
        }

        var response = await apiClient.UpdateObject2D(objectData);

        if (response is WebRequestData<string> or WebRequestData<object>)
            Debug.Log("🔄 Object geüpdatet!");
        else
            Debug.LogError("❌ Kon object niet updaten.");
    }

    private async void DeleteObject()
    {
        Debug.Log($"🗑️ DeleteObject aangeroepen met ID: {objectData.id}");

        if (!HasValidId)
        {
            Debug.LogWarning("❌ Delete genegeerd, object had geen geldige ID.");
            Destroy(gameObject); // Toch lokaal verwijderen indien niet opgeslagen.
            return;
        }

        var response = await apiClient.DeleteObject2D(objectData.id);
        if (response is WebRequestData<string> or WebRequestData<object>)
        {
            Debug.Log("🗑️ Object succesvol verwijderd!");
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("❌ Object verwijderen mislukt.");
        }
    }

    private bool IsMouseOverThisObject()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);
        foreach (var hit in hits)
            if (hit.collider != null && hit.collider.gameObject == gameObject)
                return true;
        return false;
    }
}
