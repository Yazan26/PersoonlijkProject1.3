using UnityEngine;

public class CarController : MonoBehaviour
{
    public Sprite[] carSprites; // Assign 4 sprites in Inspector (Front, Back, Left, Right)
    private SpriteRenderer spriteRenderer;
    private int currentRotationIndex = 0; // 0 = Front, 1 = Back, 2 = Left, 3 = Right

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (carSprites.Length != 4)
        {
            Debug.LogError("❌ Assign exactly 4 car sprites (Front, Back, Left, Right)");
            return;
        }

        spriteRenderer.sprite = carSprites[currentRotationIndex]; // Start with Front sprite
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            RotateCar(-1); // Rotate left
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            RotateCar(1); // Rotate right
        }
    }

    void RotateCar(int direction)
    {
        currentRotationIndex = (currentRotationIndex + direction + 4) % 4;
        spriteRenderer.sprite = carSprites[currentRotationIndex];
        Debug.Log($"✅ Car rotated: {currentRotationIndex}");
    }
}