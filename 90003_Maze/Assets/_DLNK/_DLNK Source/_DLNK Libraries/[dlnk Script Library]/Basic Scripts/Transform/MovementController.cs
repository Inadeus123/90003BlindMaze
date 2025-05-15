using UnityEngine;

public class MovementController : MonoBehaviour
{
    public bool useVelocity = true;
    public bool useRotation = true;
    public bool useAcceleration = true;

    public Vector3 initialVelocity = new Vector3(5f, 0f, 0f); // Initial velocity of the object
    public Vector3 acceleration; // Acceleration of the object
    public Vector3 rotationSpeed; // Rotation speed of the object on each axis
    public Vector3 offsetRange; // Range of offset value for the transform

    private Vector3 initialLocalPosition;
    private Vector3 currentVelocity; // Current velocity to maintain accurate value

    void Start()
    {
        // Store the initial local position of the object
        initialLocalPosition = transform.localPosition;
        currentVelocity = initialVelocity; // Initialize current velocity

        // Generate random offset values within the specified range
        float randomOffsetX = Random.Range(-offsetRange.x, offsetRange.x);
        float randomOffsetY = Random.Range(-offsetRange.y, offsetRange.y);
        float randomOffsetZ = Random.Range(-offsetRange.z, offsetRange.z);

        // Apply the random offset to the initial local position
        transform.localPosition = initialLocalPosition + new Vector3(randomOffsetX, randomOffsetY, randomOffsetZ);
    }

    void Update()
    {
        if (useVelocity)
        {
            // Move the object with the given velocity and acceleration
            MoveWithVelocityAndAcceleration();
        }

        if (useRotation)
        {
            // Rotate the object with the given rotation speed
            RotateObject();
        }
    }

    void MoveWithVelocityAndAcceleration()
    {
        Vector3 displacement;

        if (useAcceleration)
        {
            // Calculate displacement based on current velocity and acceleration
            currentVelocity += acceleration * Time.deltaTime;
            displacement = currentVelocity * Time.deltaTime;
        }
        else
        {
            // Calculate displacement based on the initial velocity only
            displacement = initialVelocity * Time.deltaTime;
        }

        // Move the object in its local space
        transform.localPosition += transform.TransformDirection(displacement);
    }

    void RotateObject()
    {
        // Rotate the object around its respective local axis with the given rotation speed
        Vector3 rotation = rotationSpeed * Time.deltaTime;
        transform.Rotate(rotation, Space.Self);
    }
}
