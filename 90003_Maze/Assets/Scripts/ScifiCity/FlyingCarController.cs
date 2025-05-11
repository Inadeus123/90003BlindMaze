using System;
using UnityEngine;
using UnityEngine.InputSystem;  // 新输入系统命名空间

public class FlyingCarController : MonoBehaviour
{
    [Header("飞车移动参数")]
    public float forwardSpeed = 20f;   // 飞车前进速度（单位：米/秒）
    public float yawSpeed = 60f;       // 左右偏航转向速度（度/秒）
    public float pitchSpeed = 60f;     // 上下俯仰旋转速度（度/秒）
    public float maxPitchAngle = 45f;  // 俯仰角最大值限制（度）
    float yawInput;   // 左右输入, 左为-1, 右为1
    float pitchInput; // 上下输入, 上为1, 下为-1
    private float actualPitchDelta;
    private float pitchDelta;
    private float yawDelta; // 根据左右输入，计算偏航旋转角度增量
    // 计算俯仰的目标角（累加，但限制范围）
    float targetPitch;
    

    [Header("输入动作引用")]
    [SerializeField] public InputActionReference moveAction;  // 引用Input System配置的“Move”动作

    private float currentPitch = 0f;  // 当前俯仰角度累计值

    void OnEnable()
    {
        // 启用输入动作
        if (moveAction != null)
            moveAction.action.Enable();
    }

    void OnDisable()
    {
        // 禁用输入动作（清理）
        if (moveAction != null)
            moveAction.action.Disable();
    }

    void Update()
    {
        // 读取输入的二维向量值 (X: 左右, Y: 上下)
        Vector2 inputVector = Vector2.zero;
        if (moveAction != null)
        {
            inputVector = moveAction.action.ReadValue<Vector2>();
        }
        // 输入解释：假设Up箭头产生 Y=1 表示想抬头（负俯仰角变化），Down箭头 Y=-1 表示机头向下
        float yawInput = inputVector.x;   // 左右输入, 左为-1, 右为1
        float pitchInput = inputVector.y; // 上下输入, 上为1, 下为-1

        // 1. 偏航旋转：绕世界Y轴旋转，实现左右转向
        // 根据左右输入，计算偏航旋转角度增量
        yawDelta = yawInput * yawSpeed * Time.deltaTime;
        

        // 2. 俯仰旋转：绕本地X轴旋转，实现机头上下
        // 计算新的俯仰角增量（注意上按键应使机头抬起，即俯仰角减小，因此取负号）
         pitchDelta = -pitchInput * pitchSpeed * Time.deltaTime;
        // 计算俯仰的目标角（累加，但限制范围）
        targetPitch = currentPitch + pitchDelta;
        // 限制俯仰角在 -maxPitchAngle 和 +maxPitchAngle 之间，防止翻转过头
        targetPitch = Mathf.Clamp(targetPitch, -maxPitchAngle, maxPitchAngle);
        // 计算实际应用的增量（如果超出限制则会小于原delta）
        actualPitchDelta = targetPitch - currentPitch;
        

       
    }

    private void FixedUpdate()
    {
        // 绕世界坐标的Y轴旋转物体
        transform.Rotate(0f, yawDelta, 0f, Space.World);
        // 绕自身X轴旋转俯仰角增量
        transform.Rotate(actualPitchDelta, 0f, 0f, Space.Self);
        // 更新当前累计俯仰角
        currentPitch = targetPitch;
        // 3. 自动前进：每帧按照当前朝向向前移动
        Vector3 forwardMove = transform.forward * forwardSpeed * Time.deltaTime;
        transform.Translate(forwardMove, Space.World);
    }
}
