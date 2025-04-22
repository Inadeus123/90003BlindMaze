using System;
using GGG.Tool;
using UnityEngine;

public class CameraCollider : MonoBehaviour
{
    //最小值，最大值，偏移量
    //Layer障碍物的层级
    [SerializeField,Header("最大最小偏移量")] private Vector2 _maxDistanceOffset;
    [SerializeField,Header("检测层级"),Space(10)] 
    private LayerMask _whatIsWall;
    
    [SerializeField,Header("射线长度"),Space(10)]
    private float _detectionDistance;

    [SerializeField, Header("碰撞移动的平滑时间"), Space(10)]
    private float _colliderSmoothTime;
    
    //开始的时候保存起始点和起始的偏移量
    private Vector3 _originPosition;
    private float _originOffsetDistance;
    private Transform _mainCameraTransform;

    private void Awake()
    {
        _mainCameraTransform = Camera.main.transform;
    }

    private void Start()
    {
        _originPosition = transform.localPosition.normalized;
        _originOffsetDistance = _maxDistanceOffset.y;
    }

    private void LateUpdate()
    {
        UpdateCameraCollider();
    }

    private void UpdateCameraCollider()
    {
        var detectionDirection = transform.TransformPoint(_originPosition * _detectionDistance);
        if (Physics.Linecast(transform.position, detectionDirection, out var hit, _whatIsWall,
                QueryTriggerInteraction.Ignore))
        {
            //如果打到东西了
            _originOffsetDistance = Mathf.Clamp(hit.distance*0.9f, _maxDistanceOffset.x, _maxDistanceOffset.y);
            //Debug.Log("打到东西了"+hit.distance);
        }else
        {
            //如果没有打到东西了
            _originOffsetDistance = _maxDistanceOffset.y;
        }
        
        _mainCameraTransform.localPosition = Vector3.Lerp(_mainCameraTransform.localPosition,
            _originPosition * (_originOffsetDistance - 0.2f), DevelopmentToos.UnTetheredLerp(_colliderSmoothTime));

    }
}
