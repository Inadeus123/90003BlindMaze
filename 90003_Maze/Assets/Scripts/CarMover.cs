using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMover : MonoBehaviour
{
   public Vector3 moveDir = Vector3.forward;
   private float speed = 50f;

   private void Update()
   {
     transform.position += moveDir * speed * Time.deltaTime;
   }
   
}
