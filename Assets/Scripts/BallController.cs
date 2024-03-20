using UnityEngine;

public class BallController : MonoBehaviour
{
    public float respawnDelay = 1f;
    public float randomBounceChance = 0.1f;
    public float randomBounceStrength = 10f;

    private Rigidbody rb;
    private Vector3 initialPosition;
    private GameObject rink;
    private Vector3 rinkExtents;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialPosition = transform.position;
        rink = GameObject.FindGameObjectWithTag("Rink");
        rinkExtents = rink.GetComponent<Renderer>().bounds.extents;
    }

    private void FixedUpdate()
    {
        ClampPositionWithinRink();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Random.value < randomBounceChance)
        {
            ApplyRandomBounce();
        }
    }

    public void Respawn()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        gameObject.SetActive(false);
        Invoke("RespawnBall", respawnDelay);
    }

    private void RespawnBall()
    {
        transform.position = initialPosition;
        gameObject.SetActive(true);
    }

    private void ApplyRandomBounce()
    {
        Vector3 randomDirection = Random.insideUnitSphere;
        rb.AddForce(randomDirection * randomBounceStrength, ForceMode.Impulse);
    }

    private void ClampPositionWithinRink()
    {
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -rinkExtents.x, rinkExtents.x);
        clampedPosition.z = Mathf.Clamp(clampedPosition.z, -rinkExtents.z, rinkExtents.z);
        transform.position = clampedPosition;
    }
}