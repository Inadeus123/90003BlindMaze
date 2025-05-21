using UnityEngine;

public class SimpleCarController : MonoBehaviour
{
    public float moveSpeed = 10f;       // 前进速度
    public float turnSpeed = 100f;      // 转向速度
    public float strafeSpeed = 5f;      // 左右平移速度（飞行车风格）

    void Update()
    {
        // 获取输入
        float move = Input.GetAxis("Vertical");   // W/S or Up/Down
        float turn = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float strafe = 0f;

        // 使用 Q/E 键或 Shift+方向键 左右平移（可选）
        if (Input.GetKey(KeyCode.Q)) strafe = -1f;
        if (Input.GetKey(KeyCode.E)) strafe = 1f;

        // 移动前进/后退
        transform.Translate(Vector3.forward * move * moveSpeed * Time.deltaTime);

        // 左右平移
        transform.Translate(Vector3.right * strafe * strafeSpeed * Time.deltaTime);

        // 转向
        transform.Rotate(Vector3.up, turn * turnSpeed * Time.deltaTime);
    }
}
