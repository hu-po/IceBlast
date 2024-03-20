using UnityEngine;

public class BallController : MonoBehaviour
{
    public float respawnDelay = 1f;
    public float randomBounceChance = 0.1f;
    public float randomBounceStrength = 1f;

    private Rigidbody rb;
    private Vector3 initialPosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialPosition = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("OutOfBounds"))
        {
            Respawn();
        }
        else
        {
            if (Random.value < randomBounceChance)
            {
                ApplyRandomBounce();
            }
        }
    }

    public void Respawn()
    {
        // Disable the ball's physics and hide it
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        gameObject.SetActive(false);

        // Wait for the respawn delay and then respawn the ball
        Invoke("RespawnBall", respawnDelay);
    }

    private void RespawnBall()
    {
        // Reset the ball's position and enable its physics
        transform.position = initialPosition;
        gameObject.SetActive(true);
    }

    private void ApplyRandomBounce()
    {
        // Apply a random bounce force to the ball
        Vector3 randomDirection = Random.insideUnitSphere;
        rb.AddForce(randomDirection * randomBounceStrength, ForceMode.Impulse);
    }
}