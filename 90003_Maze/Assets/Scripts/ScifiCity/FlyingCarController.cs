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
		// 只用 horInput
		Vector3 movement = transform.forward * forwardSpeed
		                   + transform.right  * horInput * horizontalSpeed;

		Vector3 newPos = rb.position + movement * Time.fixedDeltaTime;
		newPos.x = Mathf.Clamp(newPos.x, -8f, 8f);   // 保证不飞出车道
		rb.MovePosition(newPos);

		// 仅做左右滚动视觉
		float bank = -horInput * 20f;
		rb.MoveRotation(
			Quaternion.Slerp(rb.rotation,
				Quaternion.Euler(0f, 0f, bank),
				4f * Time.fixedDeltaTime));
	}
}

