using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : Vehicle
{
    public float Speed;
    public float Force;
    public Vehicle vehicle;
 
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
    //    SteeringForce=m_pSteering.Flee(vehicle.transform.position);
  
     
      
    }
 public override void OnTriggerEnter(Collider collision) {
    
     base.OnTriggerEnter(collision);
        
  
 }
  public override void OnTriggerExit(Collider collision) {
    
     base.OnTriggerExit(collision);
        
  
 }
}
