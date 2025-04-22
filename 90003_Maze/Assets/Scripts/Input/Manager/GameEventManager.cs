using System;
using System.Collections.Generic;
using GGG.Tool;
using GGG.Tool.Singleton;
using UnityEngine;

public class GameEventManager : Singleton<GameEventManager>
{
   private interface IEventHelp
   {
      
   }

   private class EventHelp : IEventHelp
   {
      private event Action _action;

      public EventHelp(Action action)
      {
         _action = action;
      }

      //增加事件的注册函数
      public void AddCall(Action action)
      {
         _action += action;
      }
      
      //调用事件
      public void Call()
      {
         _action?.Invoke();
      }
      
      public void RemoveCall(Action action)
      {
         _action -= action;
      }
   }
   
   private class EventHelp<T1,T2> : IEventHelp
   {
      private event Action<T1,T2> _action;

      public EventHelp(Action<T1,T2> action)
      {
         _action = action;
      }

      //增加事件的注册函数
      public void AddCall(Action<T1,T2> action)
      {
         _action += action;
      }
      
      //调用事件
      public void Call(T1 value1, T2 value2)
      {
         _action?.Invoke(value1, value2);
      }
      
      public void RemoveCall(Action<T1,T2> action)
      {
         _action -= action;
      }
   }
   
   private class EventHelp<T> : IEventHelp
   {
      private event Action<T> _action;

      public EventHelp(Action<T> action)
      {
         _action = action;
      }

      //增加事件的注册函数
      public void AddCall(Action<T> action)
      {
         _action += action;
      }
      
      //调用事件
      public void Call(T value)
      {
         _action?.Invoke(value);
      }
      
      public void RemoveCall(Action<T> action)
      {
         _action -= action;
      }
   }
   
   private Dictionary<string,IEventHelp> _eventCenter = new Dictionary<string, IEventHelp>();
   
   /// <summary>
   /// 添加事件监听
   /// </summary>
   /// <param name="eventName">事件名称</param>
   /// <param name="action">回调函数</param>
   public void AddEventListening(string eventName, Action action)
   {
      if (_eventCenter.TryGetValue(eventName, out var e))
      {
         (e as EventHelp)?.AddCall(action);
      }else
      {
         //如果事件中心没有这个事件，就创建一个新的事件
         _eventCenter.Add(eventName,new EventHelp(action));
      }
   }
   
   public void AddEventListening<T>(string eventName, Action<T> action)
   {
      if (_eventCenter.TryGetValue(eventName, out var e))
      {
         (e as EventHelp<T>)?.AddCall(action);
      }else
      {
         //如果事件中心没有这个事件，就创建一个新的事件
         _eventCenter.Add(eventName,new EventHelp<T>(action));
      }
   }
   
   public void AddEventListening<T1,T2>(string eventName, Action<T1,T2> action)
   {
      if (_eventCenter.TryGetValue(eventName, out var e))
      {
         (e as EventHelp<T1,T2>)?.AddCall(action);
      }else
      {
         //如果事件中心没有这个事件，就创建一个新的事件
         _eventCenter.Add(eventName,new EventHelp<T1,T2>(action));
      }
   }

   public void CallEvent(string eventName)
   {
      if (_eventCenter.TryGetValue(eventName, out var e))
      {
         (e as EventHelp)?.Call();
      }else
      {
         DevelopmentToos.LogWarningError($"没有这个事件：{eventName}");
      }
   }
   
   public void CallEvent<T>(string eventName, T value)
   {
      if (_eventCenter.TryGetValue(eventName, out var e))
      {
         (e as EventHelp<T>)?.Call(value);
      }else
      {
         DevelopmentToos.LogWarningError($"没有这个事件：{eventName}");
      }
   }
   
   public void CallEvent<T1, T2>(string eventName, T1 value1, T2 value2)
   {
      if (_eventCenter.TryGetValue(eventName, out var e))
      {
         (e as EventHelp<T1,T2>)?.Call(value1,value2);
      }else
      {
         DevelopmentToos.LogWarningError($"没有这个事件：{eventName}");
      }
   }

   public void RemoveEvent<T>(string eventName, Action<T> action)
   {
      if (_eventCenter.TryGetValue(eventName, out var e))
      {
         (e as EventHelp<T>)?.RemoveCall(action);
      }else
      {
         DevelopmentToos.LogWarningError($"没有这个事件：{eventName}");
      }
   }
 
   public void RemoveEvent<T1, T2>(string eventName, Action<T1, T2> action)
   {
      if (_eventCenter.TryGetValue(eventName, out var e))
      {
         (e as EventHelp<T1, T2>)?.RemoveCall(action);
      }else
      {
         DevelopmentToos.LogWarningError($"没有这个事件：{eventName}");
      }
   }
   
   public void RemoveEvent(string eventName, Action action)
   {
      if (_eventCenter.TryGetValue(eventName, out var e))
      {
         (e as EventHelp)?.RemoveCall(action);
      }else
      {
         DevelopmentToos.LogWarningError($"没有这个事件：{eventName}");
      }
   }
}
