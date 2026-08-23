using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class RatController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float patrolSpeed = 2.5f;
    public float luredSpeed = 0.2f;
    public float lureDuration = 300f;
    public float inspectDuration = 2f;

    [Tooltip("1 to start moving UP, -1 to start moving DOWN")]
    public float startDirectionY = 1f;

    private float _currentDirectionY;
    private bool _isAttracted = false;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private Coroutine _moveCoroutine;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _currentDirectionY = Mathf.Sign(startDirectionY);
        UpdateSpriteFlip();
    }

    void FixedUpdate()
    {
        if (!_isAttracted)
        {
            Vector2 movement = new Vector2(0, _currentDirectionY * patrolSpeed * Time.fixedDeltaTime);
            _rb.MovePosition(_rb.position + movement);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<OtterController>() != null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        if (_isAttracted)
        {
            if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);
            StartCoroutine(FinishAttractionEarly());
            return;
        }

        _currentDirectionY *= -1f;
        UpdateSpriteFlip();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SoundWave incomingWave = collision.GetComponent<SoundWave>();
        if (incomingWave != null && incomingWave.signalIndex == 2)
        {
            if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);
            _moveCoroutine = StartCoroutine(MoveToSound(incomingWave.transform.position));
        }

        if (collision.GetComponent<OtterController>() != null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private IEnumerator MoveToSound(Vector3 soundPos)
    {
        _isAttracted = true;
        float timer = 0f;

        // Slow crawl toward the lure at 0.8f for up to lureDuration seconds
        while (Vector3.Distance(transform.position, soundPos) > 0.2f && timer < lureDuration)
        {
            timer += Time.deltaTime;
            Vector3 newPos = Vector3.MoveTowards(transform.position, soundPos, luredSpeed * Time.deltaTime);
            _rb.MovePosition(newPos);
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(inspectDuration);
        _isAttracted = false;
    }

    private IEnumerator FinishAttractionEarly()
    {
        yield return new WaitForSeconds(inspectDuration);
        _isAttracted = false;
    }

    private void UpdateSpriteFlip()
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.flipY = (_currentDirectionY < 0);
        }
    }
}