using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObject:Singleton<WorldObject>
{
   public Vector3 WanderTogetherTargetLocal=Vector3.zero;
   private Vector3 m_dWanderTarget=Vector3.zero;
    float m_dWanderJitter=1f;
    float wanderUpdateTime=1.5f;
    float WanderRadius=3f;
    float WanderDistance=0f;
    float time=0f;
   public WorldObject_SO worldObject_SO;
   private void Update() {
      if(time<0)
      {
       time=wanderUpdateTime;
       RandomVector3();
      }
      else
      {
         time-=Time.deltaTime;
      }
   }
   public void RandomVector3()
   {
      //取随机数
   m_dWanderTarget=new Vector3(Random.Range(-1,1)*m_dWanderJitter,0,Random.Range(-1,1)*m_dWanderJitter);
   m_dWanderTarget.Normalize();
    m_dWanderTarget*=WanderRadius;
   //TODO:problem
   Vector3 targetLocal=m_dWanderTarget+new Vector3(0,0,WanderDistance);
  WanderTogetherTargetLocal=targetLocal;
   
   }
   public float AngleTransformToFloat(float angle)
   {
    return Mathf.PI/180f*angle;
   }
}
