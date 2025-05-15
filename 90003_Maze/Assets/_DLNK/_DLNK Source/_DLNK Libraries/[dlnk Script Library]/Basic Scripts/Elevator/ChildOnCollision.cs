using UnityEngine;
using System.Collections;

public class ChildOnCollision : MonoBehaviour
{
    public string targetTag = "Player";
    private Transform originalParent;
    private GameObject collidedObject;
    private CharacterController characterController;
    private Vector3 lastParentPosition;

    private bool originalParentSet = false;
    public bool useTrigger = true;
    public float exitDistanceThreshold = 1.0f;

    private void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (useTrigger && other.gameObject.CompareTag(targetTag))
        {
            HandleCollision(other.gameObject);
        }
    }

    void Update()
    {
        // If the collided object is assigned, sync its position
        if (collidedObject != null)
        {
            SyncChildPositionWithParent();

            // Check if the object has moved too far away
            if (IsObjectOutsideBounds(collidedObject))
            {
                StartCoroutine(ResetObjectParentAfterFrame());
            }
        }
    }

    private void HandleCollision(GameObject obj)
    {
        collidedObject = obj;

        // Only save the original parent the first time the object collides (don't overwrite)
        if (!originalParentSet)
        {
            originalParent = collidedObject.transform.parent;
            originalParentSet = true;
        }

        // Set the collided object as a child of this GameObject (the elevator)
        collidedObject.transform.SetParent(transform);

        // Get the CharacterController on the collided object
        characterController = collidedObject.GetComponent<CharacterController>();

        // Track the initial position of the parent for movement calculation
        lastParentPosition = transform.position;

        Debug.Log("Object with tag " + targetTag + " is now a child of " + gameObject.name);
    }

    private void SyncChildPositionWithParent()
    {
        if (characterController != null)
        {
            // Calculate movement of the parent object since the last frame
            Vector3 parentMovement = transform.position - lastParentPosition;

            // Apply the movement to the CharacterController of the child
            characterController.Move(parentMovement);

            // Update the last known position of the parent
            lastParentPosition = transform.position;
        }
    }

    private bool IsObjectOutsideBounds(GameObject obj)
    {
        Collider elevatorCollider = GetComponent<Collider>();
        Bounds elevatorBounds = elevatorCollider.bounds;
        Vector3 closestPoint = elevatorBounds.ClosestPoint(obj.transform.position);
        float distanceToClosestPoint = Vector3.Distance(obj.transform.position, closestPoint);

        return distanceToClosestPoint > exitDistanceThreshold;
    }

    private IEnumerator ResetObjectParentAfterFrame()
    {
        yield return new WaitForEndOfFrame();

        if (collidedObject != null)
        {
            if (originalParent != null)
            {
                collidedObject.transform.SetParent(originalParent);
                Debug.Log("Object returned to its original parent: " + originalParent.name);
            }
            else
            {
                collidedObject.transform.SetParent(null);
                Debug.Log("Object returned to the root of the hierarchy.");
            }

            // Clear references
            collidedObject = null;
            characterController = null;
            originalParentSet = false;
        }
    }
}
