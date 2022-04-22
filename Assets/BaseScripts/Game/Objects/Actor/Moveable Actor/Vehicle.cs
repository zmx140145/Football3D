using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(BoxCollider))]
public class Vehicle : MovingEntity
{
    
   public HashSet<BaseGameEntity> obstaclesHashSet;
   public List<BaseGameEntity> PathList; 
   public IEnumerator<BaseGameEntity> currentPath;
   public float m_dDViewLength;
  
  public float timelimit=0.02f;
   public BoxCollider boxCollider; 
   public SteeringBehaviors m_pSteering;
   public Vector3 Target;
   protected float Firtion;
   public Vehicle PursuitTarget;
   public Vector3 SteeringForce;
   public WorldObject_SO So;
   public float smoothedTime=0.01f;
   public override void Awake() {
      base.Awake();
   boxCollider=gameObject.GetComponent<BoxCollider>();
  
   }
   public  override void Start()
   {
base.Start();
So=WorldObject.Instance.worldObject_SO;

Firtion=WorldObject.Instance.worldObject_SO.Firction;



 m_pSteering=new SteeringBehaviors(this);
 obstaclesHashSet=new HashSet<BaseGameEntity>();
 PathList=new List<BaseGameEntity>();
 currentPath=PathList.GetEnumerator();
   SteeringForce=Vector3.zero;
   }
   public override void Update() {
      base.Update();
    
    
    
   }
   public void SteerUpdate(float time_elapsed)
   {
      
      Vector3 acceleration;
      //执行动作
      
      
    if(m_vVelocity.magnitude>0.00001f)
         {
            if(smoothedTime>0)
            {
           m_vHeading+=m_vVelocity.normalized;
           m_vHeading.Normalize();
           smoothedTime-=Time.deltaTime;
            }
            else
            {
            m_vHeading+=m_vVelocity.normalized;
           m_vHeading.Normalize();
           m_vSmoothedHeading=m_vHeading;
           smoothedTime=0.01f;
            }


            
         
                //增加摩擦力
          acceleration=(SteeringForce+m_vSmoothedHeading*Firtion)/m_dMass;
         
        
         }
         else
         {
         
            if(SteeringForce.sqrMagnitude>m_vSmoothedHeading.sqrMagnitude*Firtion*Firtion)
            acceleration=(SteeringForce+m_vSmoothedHeading*Firtion)/m_dMass;
            else
            {
            acceleration=Vector3.zero;
            }
           m_vVelocity=Vector3.zero; 
         }
      
  //更新速度
   m_vVelocity+=acceleration*time_elapsed;
  //确保交通工具的速度不超过最大速度
   m_vVelocity=Vector3.ClampMagnitude(m_vVelocity,m_MaxSpeed);
  //更新位置
  //TODO:problem
   transform.position+=m_vVelocity*time_elapsed;
   transform.forward=m_vSmoothedHeading;
   
   Vector3.OrthoNormalize(ref m_vSmoothedHeading,ref m_vUpward,ref m_vSide);
  
  //TODO:还要限定范围 
   }
//与SteeringBehavior的躲避障碍物有关
//碰撞体检测
public virtual void OnTriggerEnter(Collider collision) {
    if(collision.gameObject.CompareTag("Obstacle"))
   {
  
      if(collision.gameObject.GetComponent<BaseGameEntity>()!=null)
      {
       
             Debug.Log("good");
        obstaclesHashSet.Add(collision.gameObject.GetComponent<BaseGameEntity>());
      }
   }
}
public virtual void OnTriggerExit(Collider collision) {
    
    if(collision.gameObject.CompareTag("Obstacle"))
   {
  
      if(collision.gameObject.GetComponent<BaseGameEntity>()!=null)
      {
     Debug.Log("out");
        obstaclesHashSet.Remove(collision.gameObject.GetComponent<BaseGameEntity>());
      }
   }
}
}