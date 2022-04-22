using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class actor : BaseGameEntity
{
  
  StateMachine<actor> m_pStateMachine;
  
  public virtual void Awake() {
    m_pStateMachine= new StateMachine<actor>(this);
    
    
  
    }

    public override void Start()
    {
      base.Start();
   

    }
public override bool HandleMessage(Telegram msg)
{
    return m_pStateMachine.HandleMessage(msg);
}

 public  StateMachine<actor> GetFSM(){return m_pStateMachine;}
}
