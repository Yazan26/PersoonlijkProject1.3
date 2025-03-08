using UnityEngine;

public class Animatedcode : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;

    public Sprite[] sprites;

    private int currentSpriteIndex;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating(nameof(AnimateSprite), 0.15f, 0.15f);
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void AnimateSprite()
    {
        currentSpriteIndex++;

        if (currentSpriteIndex >= sprites.Length)
        {
            currentSpriteIndex = 0;
        }

        spriteRenderer.sprite = sprites[currentSpriteIndex];
    }

}
