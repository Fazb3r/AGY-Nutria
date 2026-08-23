using UnityEngine;
using System.Collections;

public class OtterController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public bool flipSpriteOnX = true;

    private bool _isMoving = false;
    private Rigidbody2D _rb;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
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
        if (_animator != null) _animator.SetBool("isMoving", true);

        Vector3 finalTarget = targetPos;
        if (!shouldAttract)
        {
            Vector3 directionAway = (transform.position - targetPos).normalized;
            finalTarget = transform.position + (directionAway * 3f);
        }

        UpdateFacingDirection(finalTarget);

        float safetyTimer = 0f;
        float maxTime = 3.5f;

        while (Vector3.Distance(transform.position, finalTarget) > 0.1f && safetyTimer < maxTime)
        {
            safetyTimer += Time.deltaTime;
            Vector3 newPos = Vector3.MoveTowards(transform.position, finalTarget, moveSpeed * Time.deltaTime);
            _rb.MovePosition(newPos);
            yield return new WaitForFixedUpdate();
        }

        StopMovement();
    }

    private void StopMovement()
    {
        _isMoving = false;
        if (_animator != null) _animator.SetBool("isMoving", false);
    }

    private void UpdateFacingDirection(Vector3 destination)
    {
        Vector3 moveDir = destination - transform.position;

        if (flipSpriteOnX && _spriteRenderer != null)
        {
            if (moveDir.x < -0.05f)
            {
                _spriteRenderer.flipX = true;
            }
            else if (moveDir.x > 0.05f)
            {
                _spriteRenderer.flipX = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isMoving)
        {
            StopAllCoroutines();
            StopMovement();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (_isMoving)
        {
            StopAllCoroutines();
            StopMovement();
        }
    }
}