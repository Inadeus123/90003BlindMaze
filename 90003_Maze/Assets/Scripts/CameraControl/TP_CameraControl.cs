using System;
using GGG.Tool;
using UnityEngine;

public class TP_CameraControl : MonoBehaviour
{
   //private GameInputManager _gameInputManager;
   //相机的移动速度
   [SerializeField, Header("相机参数配置")] private float _controlSpeed = 1f;
   [SerializeField] private Vector2 _cameraVerticalMaxAngle; //相机竖向最大角度
   [SerializeField] private float _smoothSpeed;
   [SerializeField] private Transform _lookTarget;
   [SerializeField] private float _positionOffset;
   [SerializeField] private float _positionSmoothTime;
   private Vector3 _smoothDampVelocity = Vector3.zero;

   private Vector2 _input; 
   private Vector3 _cameraRotation;

   private void Awake()
   {
      _lookTarget = GameObject.FindWithTag("CameraTarget").transform;
   }

   private void Update()
   {
      CameraInput();
   }

   private void LateUpdate()
   {
      UpdateCameraRotation();
      CameraPosition();
   }

   private void CameraInput()
   {
      _input.y += GameInputManager.MainInstance.CameraLook.x * _controlSpeed;
      _input.x -= GameInputManager.MainInstance.CameraLook.y * _controlSpeed;
      _input.x = Mathf.Clamp(_input.x, _cameraVerticalMaxAngle.x, _cameraVerticalMaxAngle.y);
   }
   
   /// <summary>
   /// 相机旋转，SmoothDamp是一个平滑插值函数，可以让相机的旋转更加平滑
   /// </summary>
   private void UpdateCameraRotation()
   {
     _cameraRotation = Vector3.SmoothDamp(_cameraRotation, new Vector3(_input.x, _input.y, 0), ref _smoothDampVelocity, _smoothSpeed);
     transform.eulerAngles = _cameraRotation;
   }
   
   /// <summary>
   /// 相机位置，位于角色的后方
   /// </summary>
   private void CameraPosition()
   {
      var newPosition = (_lookTarget.position +(-transform.forward * _positionOffset));
      transform.position = Vector3.Lerp(transform.position, newPosition, DevelopmentToos.UnTetheredLerp(_positionSmoothTime));
   }
}
