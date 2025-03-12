using UnityEngine;

public class AdjustCamera : MonoBehaviour
{
    public Transform worldContainer;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        AdjustView();
    }

    public void AdjustView()
    {
        if (worldContainer == null) return;

        float worldWidth = worldContainer.localScale.x;
        float worldHeight = worldContainer.localScale.y;

        // ✅ Adjust Camera Position
        cam.transform.position = new Vector3(worldWidth / 2, worldHeight / 2, -10);

        // ✅ Adjust Camera Zoom to Fit
        float aspectRatio = Screen.width / (float)Screen.height;
        float sizeX = worldWidth / (2 * aspectRatio);
        float sizeY = worldHeight / 2;
        cam.orthographicSize = Mathf.Max(sizeX, sizeY);

        Debug.Log($"🎥 Camera adjusted to fit world: {worldWidth}x{worldHeight}");
    }
}