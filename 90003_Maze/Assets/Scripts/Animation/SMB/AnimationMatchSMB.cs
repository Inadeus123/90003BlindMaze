using System;
using UnityEngine;

public class AnimationMatchSMB : StateMachineBehaviour
{
    [SerializeField,Header("匹配信息")]private float _startTime;
    [SerializeField]private float _endTime;
    [SerializeField]private AvatarTarget _avatarTarget;
    
    [SerializeField,Header("激活重力")]private bool _isEnableGravity;
    [SerializeField] private float _enableTime;
    
    private Vector3 _matchPosition;
    private Quaternion _matchRotation;

    private void OnEnable()
    {
        GameEventManager.MainInstance.AddEventListening<Vector3,Quaternion>("SetAnimationMatchInfo", GetMatchInfo);
    }

    private void OnDisable()
    {
        GameEventManager.MainInstance.RemoveEvent<Vector3,Quaternion>("SetAnimationMatchInfo", GetMatchInfo);
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameEventManager.MainInstance.CallEvent("EnableCharacterCollider", false);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!animator.isMatchingTarget)
        {
            //MatchTarget用于将动画的目标位置和旋转与角色的实际位置和旋转进行匹配
            if (!animator.IsInTransition(0) && !animator.isMatchingTarget)
            {
                animator.MatchTarget(_matchPosition, _matchRotation, _avatarTarget, new MatchTargetWeightMask(Vector3.one,0f), _startTime, _endTime);
            }
            
        }

        if (_isEnableGravity)
        {
            float currentAnimatorTime;
            if (!animator.IsInTransition(0))
            {
                //Debug.Log("Current State Time: " + animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
                currentAnimatorTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            }
            else
            {
                //Debug.Log(">> In Transition, next state: " + animator.GetNextAnimatorStateInfo(0).shortNameHash);
                currentAnimatorTime = animator.GetNextAnimatorStateInfo(0).normalizedTime;
            }
            
            if (currentAnimatorTime >= _enableTime)
            {
                //TODO 激活重力
                GameEventManager.MainInstance.CallEvent("EnableCharacterGravity", true);
                GameEventManager.MainInstance.CallEvent("EnableCharacterCollider", true);
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}

    private void GetMatchInfo(Vector3 position, Quaternion rotation)
    {
        _matchPosition = position;
        _matchRotation = rotation;
    }
}
