using UnityEngine;
using System.Collections.Generic;
using System.Collections;
public class ObjectAnimations : MonoBehaviour
{ 
    public List<Sprite> sprites; // List of sprites for the animation
    public float animationSpeed = 0.1f; // Time between frames in seconds
    private SpriteRenderer spriteRenderer;
    private int currentFrame = 0;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (sprites.Count > 0)
        {
            StartCoroutine(AnimateSprites());
        }
    }

    private IEnumerator AnimateSprites()
    {
        while (true)
        {
            spriteRenderer.sprite = sprites[currentFrame];
            currentFrame = (currentFrame + 1) % sprites.Count; // Loop back to the first frame
            yield return new WaitForSeconds(animationSpeed);
        }
    }
}
