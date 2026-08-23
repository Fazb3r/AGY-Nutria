using UnityEngine;

public class SoundWave : MonoBehaviour
{
    [Header("Wave Settings")]
    public int signalIndex = 0;      // 0 = Attract, 1 = Scare, etc.
    public float maxRadius = 3f;     // How big the wave gets
    public float expandSpeed = 5f;   // How fast it grows

    void Update()
    {
        // Grow the wave over time
        transform.localScale += Vector3.one * (expandSpeed * Time.deltaTime);

        // Destroy the wave once it reaches its maximum radius
        if (transform.localScale.x >= maxRadius)
        {
            Destroy(gameObject);
        }
    }
}