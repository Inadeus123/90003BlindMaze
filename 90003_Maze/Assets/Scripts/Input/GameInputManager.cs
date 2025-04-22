using System;
using GGG.Tool.Singleton;
using UnityEngine;

public class GameInputManager : Singleton<GameInputManager>
{
   private InputActions _gameInputAction;
   
   public Vector2 Movement => _gameInputAction.GameInput.Movement.ReadValue<Vector2>();
   public Vector2 CameraLook => _gameInputAction.GameInput.CameraLook.ReadValue<Vector2>();
  

   private void Awake()
   {
      base.Awake();
      _gameInputAction ??= new InputActions(); //判断InputActions是否已经实例化，如果没有则实例化
   }
   

   private void OnEnable()
   {
      _gameInputAction.Enable();
   }
   
   private void OnDisable()
   {
      _gameInputAction.Disable();
   }
   
}
