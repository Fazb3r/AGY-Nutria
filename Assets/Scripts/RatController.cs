using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class RatController : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float patrolDistance = 2f;
    public float patrolSpeed = 2f;
    public float attractSpeed = 4f;

    private Vector3 _startPosition;
    private bool _movingUp = true;
    private bool _isAttracted = false;
    private Rigidbody2D _rb;
    private Coroutine _moveCoroutine;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _startPosition = transform.position;
    }

    void Update()
    {
        // Solo patrulla si no está siendo atraída por una señal
        if (!_isAttracted)
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        float topLimit = _startPosition.y + patrolDistance;
        float bottomLimit = _startPosition.y - patrolDistance;

        if (_movingUp)
        {
            transform.position += Vector3.up * patrolSpeed * Time.deltaTime;
            if (transform.position.y >= topLimit) _movingUp = false;
        }
        else
        {
            transform.position += Vector3.down * patrolSpeed * Time.deltaTime;
            if (transform.position.y <= bottomLimit) _movingUp = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Detección de Onda Sonora (Índice 2)
        SoundWave incomingWave = collision.GetComponent<SoundWave>();
        if (incomingWave != null && incomingWave.signalIndex == 2)
        {
            if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);
            _moveCoroutine = StartCoroutine(MoveToSound(incomingWave.transform.position));
        }

        // 2. Si toca a la nutria, reinicia el nivel
        if (collision.GetComponent<OtterController>() != null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private IEnumerator MoveToSound(Vector3 soundPos)
    {
        _isAttracted = true;

        while (Vector3.Distance(transform.position, soundPos) > 0.1f)
        {
            Vector3 newPos = Vector3.MoveTowards(transform.position, soundPos, attractSpeed * Time.deltaTime);
            _rb.MovePosition(newPos);
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(1.5f); // Se queda quieta investigando el sonido
        _startPosition = transform.position; // Actualiza el punto central de su patrullaje
        _isAttracted = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<OtterController>() != null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}