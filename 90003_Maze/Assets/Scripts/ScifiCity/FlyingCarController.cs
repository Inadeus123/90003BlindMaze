using UnityEngine;
 
[RequireComponent(typeof(Rigidbody))]
public class FlyingCarController : MonoBehaviour
{
	public float forwardSpeed = 10f;
	public float horizontalSpeed = 8f;
	public float verticalSpeed = 6f;
 
	private float horInput;
	private float verInput;
 
	private Rigidbody rb;
 
	void Awake()
	{
    	rb = GetComponent<Rigidbody>();
	}
 
	public void SetInput(float hor, float ver)
	{
    	horInput = hor;
    	verInput = ver;
    	Debug.Log($"[FlyingCarController] SetInput => hor: {horInput}, ver: {verInput}");
	}
 
	void FixedUpdate()
	{
    	Vector3 movement = transform.forward * forwardSpeed +
                       	transform.right * horInput * horizontalSpeed +
                       	transform.up * verInput * verticalSpeed;
 
    	rb.MovePosition(rb.position + movement * Time.fixedDeltaTime);
 
    	float bank = -horInput * 20f;
    	float pitch = -verInput * 15f;
    	Quaternion tilt = Quaternion.Euler(pitch, 0f, bank);
    	rb.MoveRotation(Quaternion.Slerp(rb.rotation, tilt, 4f * Time.fixedDeltaTime));
	}
}

