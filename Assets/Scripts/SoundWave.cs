using UnityEngine;

public class SoundWave : MonoBehaviour
{
    public int signalIndex;
    
    [Header("Wave Settings")]
    [Tooltip("Qué tan rápido se expande la onda")]
    public float expansionSpeed = 5f;
    
    [Tooltip("El tamaño/radio máximo que alcanzará antes de destruirse")]
    public float maxScale = 15f; 
    
    [Tooltip("Qué tan rápido se vuelve transparente")]
    public float fadeSpeed = 1.5f;

    private SpriteRenderer _sr;

    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 1. Expandir la escala en todos los ejes (X y Y)
        transform.localScale += Vector3.one * expansionSpeed * Time.deltaTime;

        // 2. Desvanecer la onda gradualmente (hacerla transparente)
        if (_sr != null)
        {
            Color c = _sr.color;
            c.a -= fadeSpeed * Time.deltaTime;
            _sr.color = c;
        }

        // 3. Destruir el objeto cuando alcance el radio máximo o sea invisible
        if (transform.localScale.x >= maxScale || (_sr != null && _sr.color.a <= 0f))
        {
            Destroy(gameObject);
        }
    }
}