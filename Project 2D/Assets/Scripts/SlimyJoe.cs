// 9/29/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using UnityEngine;

public class SlimyJoeWander : MonoBehaviour
{
    public float speed = 2f; // Speed of movement
    public float changeDirectionInterval = 2f; // Time interval to change direction

    private Rigidbody2D rb;
    private Vector2 movementDirection;
    private float timer;

    private void Start()
    {
        // Get the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();

        // Initialize the timer and set a random direction
        timer = changeDirectionInterval;
        SetRandomDirection();
    }

    private void Update()
    {
        // Update the timer
        timer -= Time.deltaTime;

        // Change direction when the timer reaches zero
        if (timer <= 0f)
        {
            SetRandomDirection();
            timer = changeDirectionInterval;
        }
    }

    private void FixedUpdate()
    {
        // Move Slimy Joe in the current direction
        rb.linearVelocity = movementDirection * speed;
    }

    private void SetRandomDirection()
    {
        // Generate a random direction
        float randomAngle = Random.Range(0f, 360f);
        movementDirection = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)).normalized;
    }
}