using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class FootballPlayer : Vehicle
{
    public Vehicle ATarget;
    public float Speed;
    public float Force;
    //当前所在位置的序号 由Regionx检测到player的时候调用更改
    public int EnterAreaNum;
    //初始位置
      public int BaseAreaNum;
   public override void Awake()
    {
        base.Awake();
     
      
    }
    public override void Start()
    {
        base.Start();
        m_dMass=1f;
        m_dMaxTurnRate=90f;
        m_dMaxForce=Force;
       m_MaxSpeed=Speed;
      
    }
    public override void  Update() {
       
       base.Update();
       GetFSM().StateMachineUpdate();
       SteerUpdate(Time.fixedDeltaTime);
       Vector3 WallForce=m_pSteering.WallAvoidance();
       if(WallForce==Vector3.zero)
       {
             Vector3 ObForce=m_pSteering.ObstacleAvoidance();
           if(ObForce==Vector3.zero)
           {
               Vector3 OLForce=m_pSteering.EnforceNonPenetrationConstraint<Vehicle,Vehicle>(this,GameList.Instance.VehiclesList);
      if(OLForce==Vector3.zero)
      {
        SteeringForce=m_pSteering.Pursuit(ATarget);
     
      }
      else
      {
       
          Debug.Log(OLForce);
       SteeringForce=OLForce;
      }
           }
           else
           {
                SteeringForce=ObForce;
             
           }
       }
        else
        {
        SteeringForce=WallForce;
        
        }
      
    }
 public override void OnTriggerEnter(Collider collision) {
    
     base.OnTriggerEnter(collision);
        
  
 }
  public override void OnTriggerExit(Collider collision) {
    
     base.OnTriggerExit(collision);
        
  
 }
    }

