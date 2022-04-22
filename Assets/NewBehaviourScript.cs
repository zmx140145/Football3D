using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
   public Vector3 input;
   public Vector3  output=Vector3.zero;
   public Vector3 handing;
   private void Update() {
      calculate();
   }
  public void calculate()
  {
     float sign=Mathf.Sign(Vector3.SignedAngle(input,handing,Vector3.up));
    
     float angle=Mathf.PI/2*sign;
     Debug.Log(Mathf.Cos(angle));
     output.z=input.z*Mathf.Cos(angle)-input.x*Mathf.Sin(angle);
     output.x=input.x*Mathf.Cos(angle)+input.z*Mathf.Sin(angle);
     Debug.Log(Vector3.Angle(input,output));
  }
}
