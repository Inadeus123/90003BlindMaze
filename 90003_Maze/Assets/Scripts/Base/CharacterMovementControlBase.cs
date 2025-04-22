using System;
using GGG.Tool;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public abstract class CharacterMovementControlBase : MonoBehaviour
{
    [SerializeField] protected CharacterController _control;
    [SerializeField] protected Animator _animator;
    
    protected Vector3 _moveDirection;
    //地面检测
    protected bool _characterIsOnGround;
    [SerializeField,Header("地面检测")] protected float _groundDetectionPositionOffset;
    [SerializeField] protected float _detectionRang;
    [SerializeField] protected LayerMask _whatIsGround;
    
    //重力
    protected readonly float CharacterGravity = -9.81f;
    protected float _characterVerticalVelocity;//用来更新角色Y轴的速度，可以应用于重力和跳跃高度，或者受击反馈
    protected float _fallOutDeltaTime;//用来判断角色是否在高低差过小的情况下播放跌落动画
    protected float _fallOutTime = 0.15f; //常量，高低差阈值，防止在高低差过小的情况下播放跌落动画
    protected readonly float _characterVerticalMaxVelocity= 54f; //最大角色下落速度
    protected Vector3 _characterVerticalDirection; //角色的Y轴方向。因为是通过CharacterController来控制角色的，所以把_characterVerticalVelocity赋值给这个变量来控制角色的Y轴方向
    protected bool _isEnableGravity;
    
    protected virtual void Awake()
    {
        _control = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    protected virtual void OnEnable()
    {
        GameEventManager.MainInstance.AddEventListening<bool>("EnableCharacterGravity", EnableCharacterGravity);
        GameEventManager.MainInstance.AddEventListening<bool>("EnableCharacterCollider", EnableCharacterCollider);
    }
    
    protected virtual void OnDisable()
    {
        GameEventManager.MainInstance.RemoveEvent<bool>("EnableCharacterGravity", EnableCharacterGravity);
        GameEventManager.MainInstance.RemoveEvent<bool>("EnableCharacterCollider", EnableCharacterCollider);
    }  

    protected virtual void Start()
    {
        _fallOutDeltaTime = _fallOutTime;
    }

    protected virtual void Update()
    {
        SetCharacterGravity();
        UpdateCharacterGravity();
    }
    
   
    protected virtual void OnAnimatorMove()
    {
        _animator.ApplyBuiltinRootMotion();
        UpdateCharacterMoveDirection(_animator.deltaPosition);
    }
    
      
    private void UpdateCharacterMoveDirection(Vector3 direction)
    {
        _moveDirection = SlopResetDirection(direction);
        _control.Move(_moveDirection * Time.deltaTime);
    }

    
    private void UpdateCharacterGravity()
    {
        if (!_isEnableGravity)
        {
            return;
        }
        _characterVerticalDirection.Set(0,_characterVerticalVelocity,0f);
        _control.Move(_characterVerticalDirection* Time.deltaTime);
    }

    /// <summary>
    /// 地面检测
    /// </summary>
    /// <returns></returns>
    private bool GroundDection()
    {
        var detectionPosition = new Vector3(transform.position.x, 
            transform.position.y - _groundDetectionPositionOffset, transform.position.z); //检测的中心点
        
        //检测范围，detectionPosition为中心点，_detectionRang为半径,_whatIsGround为检测的层级
        return Physics.CheckSphere(detectionPosition, _detectionRang, _whatIsGround, QueryTriggerInteraction.Ignore);
    }
    
    //模拟重力
    private void SetCharacterGravity()
    {
        _characterIsOnGround = GroundDection();
        //Debug.Log("角色在地面上吗？"+_characterIsOnGround);
        if (_characterIsOnGround)
        {
           //如果角色在地面上，那么我们需要重置FallOutTime
           //重置角色的垂直速度
           _fallOutDeltaTime = _fallOutTime;
           if (_characterVerticalVelocity < 0f)
           {
               _characterVerticalVelocity = -2f;
           }
           
        }
        else
        {
            //不在地面上
            if (_fallOutDeltaTime > 0)
            {
                _fallOutDeltaTime -= Time.deltaTime; //等待一个_fallOutTime的时间,如果角色在这个时间内落地则代表高低差很小，不需要专门的跌落动画
            }
            else
            {
                //角色还没有落地，需要播放下落动画
            }

            if (_characterVerticalVelocity <_characterVerticalMaxVelocity && _isEnableGravity)
            {
                _characterVerticalVelocity += CharacterGravity * Time.deltaTime; //应用重力
            }
               
               
        }
        
    }


    /// <summary>
    /// 坡道检测
    /// </summary>
    private Vector3 SlopResetDirection(Vector3 moveDirection)
    {
        if (Physics.Raycast(transform.position + (Vector3.up * .3f), -Vector3.up, out var _hit,
                _control.height * 2f, _whatIsGround, QueryTriggerInteraction.Ignore))
        {
            var angle = Vector3.Dot(Vector3.up, _hit.normal);
            if (angle != 0f && _characterVerticalVelocity<0f)
            {
                return Vector3.ProjectOnPlane(moveDirection, _hit.normal);
            }
        }
        return moveDirection;
    }
  
    //OnDrawGizmos绘制
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = _characterIsOnGround ? Color.green : Color.red;

        Vector3 position = Vector3.zero;

        position.Set(transform.position.x, transform.position.y - _groundDetectionPositionOffset,
            transform.position.z);

        Gizmos.DrawWireSphere(position, _detectionRang);
    }
  
    //事件注册
    private void EnableCharacterGravity(bool enable)
    {
        _isEnableGravity = enable;
        _characterVerticalVelocity = enable ? -2f : 0f;
        //Debug.Log("IsEnableGravity" + _isEnableGravity);
    }
    
    private void EnableCharacterCollider(bool enable)
    {
        //Debug.Log("ChangeCollider" + enable);
        /*if (enable == false)
        {
            _control.height = 1.0f;
        }
        else
        {
            _control.height = 1.8f;
        }*/
    }
}
