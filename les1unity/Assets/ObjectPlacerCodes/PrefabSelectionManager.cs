using UnityEngine;
using UnityEngine.EventSystems;

public class PrefabSelectionManager : MonoBehaviour
{
    public GameObject carPrefab; // Assign the Car Prefab in Inspector
    private GameObject selectedPrefab;
    private bool isDragging = false;

    void Update()
    {
        if (isDragging && Input.GetMouseButtonDown(0))
        {
            PlaceObject();
        }
    }

    public void SelectCarPrefab()
    {
        selectedPrefab = carPrefab;
        isDragging = true;
        Debug.Log("🚗 Car prefab selected for placement.");
    }

    void PlaceObject()
    {
        if (selectedPrefab == null) return;
        if (EventSystem.current.IsPointerOverGameObject()) return; // Prevent placing on UI

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0; // Ensure 2D placement

        Instantiate(selectedPrefab, mousePos, Quaternion.identity);
        isDragging = false; // Stop dragging
        selectedPrefab = null;

        Debug.Log("✅ Car placed in world!");
    }
}