using UnityEngine;

public class FloorChangeTrigger : MonoBehaviour
{
    public Elevator elevator;                   // Reference to the Elevator script
    public int targetFloor = 2;                 // Target floor number to change to
    public KeyCode floorChangeKey = KeyCode.E;  // Key used to trigger the floor change
    public bool useOriginalFloor = true;        // Toggle option to use the original floor as the target

    private bool playerInRange = false;         // Indicates if the player is within the trigger
    private int originalFloor;                  // Original floor number
    private int currentTargetFloor;             // Current target floor number

    private void Start()
    {
        originalFloor = elevator.GetOriginalFloor();  // Store the elevator's original floor
        currentTargetFloor = targetFloor;             // Initialize the target floor

        // Disable the MeshRenderer component on start
        GetComponent<MeshRenderer>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Press '" + floorChangeKey.ToString() + "' to change floor.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log("Player left the trigger.");
        }
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(floorChangeKey))
        {
            int currentFloor = elevator.GetCurrentFloor();

            if (currentFloor == currentTargetFloor)
            {
                // If the elevator is already at the target floor, set it to the opposite floor
                currentTargetFloor = (currentFloor == originalFloor) ? targetFloor : originalFloor;
            }
            else
            {
                // If elevator is not at the target, move it to the current target floor
                currentTargetFloor = targetFloor;
            }

            // Change the elevator's target floor to the determined floor
            elevator.ChangeTargetFloor(currentTargetFloor);

            Debug.Log("Floor changed to " + currentTargetFloor.ToString());
        }
    }
}
