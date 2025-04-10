using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightAnimations : MonoBehaviour
{
    public Light2D light2D; // Reference to the Light2D component
    public AnimationCurve intensityCurve; // Curve for animating intensity
    public AnimationCurve radiusCurve; // Curve for animating radius
    public float animationDuration = 2f; // Duration of one animation cycle

    private float timer;

    void Start()
    {
        if (light2D == null)
        {
            light2D = GetComponent<Light2D>();
        }
    }

    void Update()
    {
        // Increment the timer and loop it
        timer += Time.deltaTime;
        if (timer > animationDuration)
        {
            timer -= animationDuration;
        }

        // Evaluate the curves based on the normalized time
        float normalizedTime = timer / animationDuration;
        light2D.intensity = intensityCurve.Evaluate(normalizedTime);
        light2D.pointLightOuterRadius = radiusCurve.Evaluate(normalizedTime);
    }
}   