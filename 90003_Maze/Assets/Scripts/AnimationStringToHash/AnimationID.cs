using UnityEngine;

public class AnimationID
{
   public static readonly int MovementID = Animator.StringToHash("Movement");
   public static readonly int HorizontalID = Animator.StringToHash("Horizontal");
   public static readonly int VerticalID = Animator.StringToHash("Vertical");
   public static readonly int LockID = Animator.StringToHash("Lock");
   public static readonly int HasInputID = Animator.StringToHash("HasInput");
   public static readonly int IsRunningID = Animator.StringToHash("IsRunning");
}
