using UnityEngine;

public class WorldSizeManager : MonoBehaviour
{
    public Transform worldContainer; // Assign the world parent object in Inspector

    void Start()
    {
        string worldId = PlayerPrefs.GetString("SelectedWorldId", "");
        float worldWidth = PlayerPrefs.GetFloat("WorldWidth", 100f); // Default size
        float worldHeight = PlayerPrefs.GetFloat("WorldHeight", 50f);

        worldContainer.localScale = new Vector3(worldWidth, worldHeight, 1);
        Debug.Log($"🌍 World size set to {worldWidth}x{worldHeight}");
    }
}