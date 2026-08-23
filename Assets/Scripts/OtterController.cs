using UnityEngine;
using System.Collections;

public class OtterController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    
    private bool _isMoving = false;

    // The Otter still uses this to "hear" the expanding Sound Wave
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isMoving) return; 

        SoundWave incomingWave = collision.GetComponent<SoundWave>();
        if (incomingWave != null)
        {
            // If the signal is 0 (Attract), walk smoothly towards it
            if (incomingWave.signalIndex == 0)
            {
                StartCoroutine(WalkToTarget(incomingWave.transform.position));
            }
        }
    }

    private IEnumerator WalkToTarget(Vector3 target)
    {
        _isMoving = true;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        // Keep moving until we are really close to the center of the sound
        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            Vector3 newPos = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            
            // MovePosition forces the physics engine to handle the movement, ensuring hard collisions!
            rb.MovePosition(newPos);
            
            yield return new WaitForFixedUpdate();
        }

        _isMoving = false;
    }
    
    // This built-in Unity function detects hits with SOLID objects (like your invisible walls)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isMoving)
        {
            // We hit a wall! Cancel the WalkToTarget loop immediately.
            StopAllCoroutines(); 
            
            // Reset the movement state so the otter can listen to the next sound
            _isMoving = false; 
        }
    }
}

