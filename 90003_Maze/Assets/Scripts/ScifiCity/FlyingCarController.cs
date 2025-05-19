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
	public bool inputLocked { get; set; } = false; 
 
	void Awake()
	{
    	rb = GetComponent<Rigidbody>();
	}
 
	public void SetInput(float hor, float ver)
	{
		if (inputLocked) return;
    	horInput = hor;
    	verInput = ver;
    	//Debug.Log($"[FlyingCarController] SetInput => hor: {horInput}, ver: {verInput}");
	}
 
	void FixedUpdate()
	{
		//Debug.Log(rb.velocity);
		// 只用 horInput
		Vector3 movement = transform.forward * forwardSpeed
		                   + transform.right  * horInput * horizontalSpeed;

		Vector3 newPos = rb.position + movement * Time.fixedDeltaTime;
		newPos.x = Mathf.Clamp(newPos.x, -8f, 8f);   // 保证不飞出车道
		rb.MovePosition(newPos);

		// 仅做左右滚动视觉
		float bank = -horInput * 20f;
		float yaw = rb.rotation.eulerAngles.y;
		rb.MoveRotation(
			Quaternion.Slerp(rb.rotation,
				Quaternion.Euler(0f, yaw, bank),
				4f * Time.fixedDeltaTime));
	}
}

