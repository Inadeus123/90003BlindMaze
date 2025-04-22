using System;
using UnityEditor.Build;
using UnityEngine;

public class PlayerClimbControl : MonoBehaviour
{
    private Animator _animator;
    
    [SerializeField,Header("检测")] private float _detectionDistance = 1f;
    [SerializeField] private LayerMask _detectionLayer;
    private RaycastHit _hit;
    public Transform cube;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        CharacterClimbInput();
    }

    private bool CanClimb()
    {
        return Physics.Raycast(transform.position + (transform.up * 0.5f), transform.forward, out _hit,
                _detectionDistance, _detectionLayer,QueryTriggerInteraction.Ignore);

    }

    private void CharacterClimbInput()
    {
        if (!CanClimb()) return;

        if (GameInputManager.MainInstance.Climb)
        {
            var i = _hit.collider.transform.position.y - transform.position.y;
            var j = i+_hit.collider.bounds.extents.y;
            var position = Vector3.zero;
            var rotation = Quaternion.LookRotation(transform.forward, _hit.collider.transform.up);
            position.Set(_hit.point.x, j,_hit.point.z);
            cube.position = new Vector3(_hit.point.x,j,_hit.point.z);

            //用tag来匹配WAll
            switch (_hit.collider.tag)
            {
                case "MediumWall":
                    ToCallEvent(position, rotation);
                    _animator.CrossFade("ClimbMediumWall", 0.1f,0,0f);
                    break;
                case"HighWall":
                    ToCallEvent(position, rotation);
                    _animator.CrossFade("ClimbHighWall", 0.1f,0,0f);
                    break;
            }
        }
    }

    private void ToCallEvent(Vector3 position, Quaternion rotation)
    {
        GameEventManager.MainInstance.CallEvent("SetAnimationMatchInfo", position, rotation);
        GameEventManager.MainInstance.CallEvent("EnableCharacterGravity", false);
    }
}
