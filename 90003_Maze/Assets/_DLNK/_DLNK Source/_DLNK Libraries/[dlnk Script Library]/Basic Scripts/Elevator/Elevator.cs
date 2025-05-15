using UnityEngine;

public class Elevator : MonoBehaviour
{
    public float floorHeight = 3f;   // Height of each floor
    public float speed = 5f;         // Speed of the elevator
    public int originalFloor = 1;    // Original floor number (1 is the ground floor)

    private Vector3 initialPosition; // Initial position of the elevator as a reference point
    private Vector3 targetPosition;
    private int targetFloor = 1;      // Target floor number
    private bool isMoving = false;    // Flag to track if the elevator is currently moving
    private bool shouldReturn = false; // Flag to track if the elevator should return to the original position

    private int currentFloor;        // Current floor number

    public bool useOriginalFloor = true;    // Toggle option to use the original floor as the target

    private void Start()
    {
        initialPosition = transform.position;  // Store the initial position as a reference for originalFloor
        currentFloor = CalculateCurrentFloor();
        SetTargetPosition();
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            // Move the elevator towards the target position
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.fixedDeltaTime);

            // Check if the elevator has reached the target floor
            if (Mathf.Approximately(Vector3.Distance(transform.position, targetPosition), 0f))
            {
                // Arrived at the target floor, stop the elevator
                isMoving = false;
                currentFloor = CalculateCurrentFloor();

                // Reset `shouldReturn` to prevent unintended movement back
                shouldReturn = false;
            }
        }
    }

    private void SetTargetPosition()
    {
        // Calculate the target position based on the target floor and initial position offset
        targetPosition = initialPosition + Vector3.up * (targetFloor - originalFloor) * floorHeight;
    }

    private int CalculateCurrentFloor()
    {
        // Calculate the current floor based on the height relative to the initial position
        float heightOffset = transform.position.y - initialPosition.y;
        int calculatedFloor = originalFloor + Mathf.RoundToInt(heightOffset / floorHeight);
        return calculatedFloor;
    }

    public int GetCurrentFloor()
    {
        return currentFloor;
    }

    public int GetOriginalFloor()
    {
        return originalFloor;
    }

    public void ChangeTargetFloor(int newTargetFloor)
    {
        // Check if the elevator is already at the target floor
        if (newTargetFloor == targetFloor)
            return;

        // Only set `shouldReturn` when changing to a floor that is not the current target
        if (useOriginalFloor && newTargetFloor == originalFloor && currentFloor != originalFloor)
        {
            shouldReturn = true;
        }

        targetFloor = newTargetFloor;
        SetTargetPosition();
        isMoving = true;
    }
}
