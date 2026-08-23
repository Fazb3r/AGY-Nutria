using UnityEngine;
using System.Collections;

public class OtterController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    
    private bool _isMoving = false;
    private Rigidbody2D _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isMoving) return; 

        SoundWave incomingWave = collision.GetComponent<SoundWave>();
        if (incomingWave != null)
        {
            if (incomingWave.signalIndex == 0)
            {
                StartCoroutine(WalkToTarget(incomingWave.transform.position, true));
            }
            else if (incomingWave.signalIndex == 1)
            {
                StartCoroutine(WalkToTarget(incomingWave.transform.position, false));
            }
        }
    }

    private IEnumerator WalkToTarget(Vector3 targetPos, bool shouldAttract)
    {
        _isMoving = true;

        Vector3 finalTarget = targetPos;
        if (!shouldAttract)
        {
            Vector3 directionAway = (transform.position - targetPos).normalized;
            finalTarget = transform.position + (directionAway * 3f);
        }

        // Usamos un temporizador de seguridad por si la ruta se bloquea por completo
        float safetyTimer = 0f;
        float maxTime = 3f; // Tiempo máximo que intentará caminar antes de liberarse solo

        while (Vector3.Distance(transform.position, finalTarget) > 0.1f && safetyTimer < maxTime)
        {
            safetyTimer += Time.deltaTime;
            Vector3 newPos = Vector3.MoveTowards(transform.position, finalTarget, moveSpeed * Time.deltaTime);
            _rb.MovePosition(newPos);
            
            yield return new WaitForFixedUpdate();
        }

        // Liberamos el movimiento al llegar o al cumplirse el tiempo de seguridad
        _isMoving = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si chocamos contra cualquier cosa sólida, detenemos el viaje de inmediato
        if (_isMoving)
        {
            StopAllCoroutines();
            _isMoving = false; 
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Si la nutria se queda rozando la pared, nos aseguramos de que no se quede bloqueada
        if (_isMoving)
        {
            StopAllCoroutines();
            _isMoving = false;
        }
    }
}