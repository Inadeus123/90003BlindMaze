
using System;
using GGG.Tool;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerMovementControl : CharacterMovementControlBase
{
    private float _rotationAngle;
    private float _angleVelocity = 0f; 
    [SerializeField] private float _rotationSmoothTime;
    private Transform _mainCamera;

    protected override void Awake()
    {
        base.Awake();
        _mainCamera = Camera.main.transform;
        
    }
    
    private void LateUpdate()
    { 
        UpdateAnimation();
        CharacterRotationControl();
    }

    private void CharacterRotationControl()
    {
        if (_animator.GetBool(AnimationID.HasInputID))
        {
            _rotationAngle = Mathf.Atan2(GameInputManager.MainInstance.Movement.x, GameInputManager.MainInstance.Movement.y) * Mathf.Rad2Deg + _mainCamera.eulerAngles.y;
            
        }
        
        if (_animator.GetBool(AnimationID.HasInputID) && _animator.AnimationAtTag("Motion"))
        {
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, _rotationAngle, ref _angleVelocity, _rotationSmoothTime);
        }
    }

    /// <summary>
    /// 更新角色动画
    /// </summary>
    private void UpdateAnimation()
    {
        #region DubugInfoDisplay
        DebugInfoDisplay.MainInstance.PlayerRotationAngle = _rotationAngle;
        DebugInfoDisplay.MainInstance.HasInput = _animator.GetBool(AnimationID.HasInputID);
        DebugInfoDisplay.MainInstance.IsPlayerRunning = _animator.GetBool(AnimationID.IsRunningID);
        DebugInfoDisplay.MainInstance.PlayerMovementInput = _animator.GetFloat(AnimationID.MovementID);
        #endregion
        
        if (!_characterIsOnGround)
        {
           
            return;
        }
        _animator.SetBool(AnimationID.HasInputID, GameInputManager.MainInstance.Movement != Vector2.zero);
        
       // Debug.Log("HasInput:" + GameInputManager.MainInstance.Movement);
        if (_animator.GetBool(AnimationID.HasInputID))
        {
            if (GameInputManager.MainInstance.Run)
            {
                _animator.SetBool(AnimationID.IsRunningID, true);
            }
            
           _animator.SetFloat(AnimationID.MovementID,_animator.GetBool(AnimationID.IsRunningID) ? 2f : GameInputManager.MainInstance.Movement.sqrMagnitude, 0.25f, Time.deltaTime);
        }
        else
        {
            _animator.SetFloat(AnimationID.MovementID, 0f, 0.25f, Time.deltaTime);
            if (_animator.GetFloat(AnimationID.MovementID) < 0.2f)
            {
                _animator.SetBool(AnimationID.IsRunningID, false);
            }
        }

    }
}
