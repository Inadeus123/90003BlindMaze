using UnityEngine;
using System.Collections;

public class SmoothMover : MonoBehaviour
{
    // Movement parameters
    public float distanceX = 0f; // Distance to move along local X-axis
    public float distanceY = 0f; // Distance to move along local Y-axis
    public float distanceZ = 10f; // Distance to move along local Z-axis

    public float maxVelocity = 5f; // Maximum velocity in units per second
    public float acceleration = 2f; // Acceleration rate
    public float deceleration = 2f; // Deceleration rate

    public bool pingPong = false; // Toggle for ping-pong movement
    public float pingPongDelay = 1f; // Delay between ping-pong movements in seconds

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float currentSpeed = 0f;
    private bool moving = true;
    private bool decelerating = false;
    private bool movingToTarget = true;
    private bool waitingForPingPong = false;

    void Start()
    {
        startPosition = transform.position;

        // Calculate the target position in local space and convert to world space
        Vector3 localOffset = new Vector3(distanceX, distanceY, distanceZ);
        targetPosition = startPosition + transform.TransformDirection(localOffset);
    }

    void Update()
    {
        if (moving && !waitingForPingPong)
        {
            MoveTowardsTarget();
        }
    }

    private void MoveTowardsTarget()
    {
        // Set the current target based on the direction
        Vector3 destination = movingToTarget ? targetPosition : startPosition;

        // Calculate the remaining distance to the current destination
        float remainingDistance = Vector3.Distance(transform.position, destination);

        // Check if it's time to decelerate
        if (remainingDistance <= Mathf.Pow(currentSpeed, 2) / (2 * deceleration))
        {
            decelerating = true;
        }

        // Adjust speed based on acceleration or deceleration
        if (decelerating)
        {
            currentSpeed -= deceleration * Time.deltaTime;
            if (currentSpeed <= 0)
            {
                currentSpeed = 0;
                moving = false; // Stop movement when speed reaches 0

                // Toggle direction for ping-pong effect if enabled
                if (pingPong)
                {
                    StartCoroutine(PingPongDelayCoroutine());
                }
            }
        }
        else
        {
            currentSpeed += acceleration * Time.deltaTime;
            currentSpeed = Mathf.Min(currentSpeed, maxVelocity); // Clamp to max velocity
        }

        // Move towards the destination at the current speed
        transform.position = Vector3.MoveTowards(transform.position, destination, currentSpeed * Time.deltaTime);

        // If reached the destination and ping-pong is off, stop moving
        if (transform.position == destination && !pingPong)
        {
            moving = false;
        }
    }

    // Coroutine to handle ping-pong delay
    private IEnumerator PingPongDelayCoroutine()
    {
        waitingForPingPong = true;
        yield return new WaitForSeconds(pingPongDelay);

        // Reset movement variables for the return trip
        movingToTarget = !movingToTarget; // Switch direction
        decelerating = false;
        moving = true;
        waitingForPingPong = false;
    }
}
