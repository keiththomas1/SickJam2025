using UnityEngine;

[RequireComponent(typeof(Light))]
public class RandomDancingLight : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float maxRotationAngle = 20f;
    public float transitionDuration = 0.5f;

    [Header("Color Settings")]
    public float colorCycleSpeed = 0.1f;  // How fast to cycle through colors

    private float startYRotation;
    private float currentYRotation;
    private float targetYRotation;
    private float velocity;

    private float timer;
    private Light spotlight;
    private float hueOffset;

    void Start()
    {
        spotlight = GetComponent<Light>();
        startYRotation = transform.eulerAngles.y;
        currentYRotation = startYRotation;
        PickNewTarget();

        // Start hue at a random point for variety
        hueOffset = Random.Range(0f, 1f);
    }

    void Update()
    {
        timer += Time.deltaTime;

        // Smoothly rotate toward target
        currentYRotation = Mathf.SmoothDampAngle(currentYRotation, targetYRotation, ref velocity, transitionDuration);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, currentYRotation, transform.rotation.eulerAngles.z);

        // Switch to new target if close
        if (Mathf.Abs(Mathf.DeltaAngle(currentYRotation, targetYRotation)) < 1f && timer > transitionDuration * 0.5f)
        {
            PickNewTarget();
        }

        // Color cycling
        float hue = Mathf.Repeat(Time.time * colorCycleSpeed + hueOffset, 1f);
        Color color = Color.HSVToRGB(hue, 1f, 1f);
        spotlight.color = color;
    }

    void PickNewTarget()
    {
        float randomOffset = Random.Range(-maxRotationAngle, maxRotationAngle);
        targetYRotation = startYRotation + randomOffset;
        timer = 0f;
    }
}
