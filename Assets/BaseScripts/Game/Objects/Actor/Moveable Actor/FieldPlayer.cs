using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum FieldPlayerType{Forward,Midfield,Guard}
public class FieldPlayer : PlayerBase
{
   public FieldPlayerType type;
    public bool NeedDefend;
    public bool NeedAttack;
    
    //构造函数
    // public FieldPlayer(SoccerTeam myTeam):base(myTeam)
    // {
    
    // }
    public override void  Start()
    {
base.Start();
 m_pStateMachine= new StateMachine<FieldPlayer>(this);
 m_pStateMachine.SetCurrentState(PlayerWait.Instance);
  m_pStateMachine.SetGlobalState(GlobalPlayerState.Instance);
  m_pStateMachine.SetPreviousState(null);
   m_pSteering.seek=true;
   

   
  switch(type)
  {
      case FieldPlayerType.Forward:
      NeedAttack=true;
       break;
       case FieldPlayerType.Midfield:
       
       break;
       case FieldPlayerType.Guard:
       NeedDefend=true;
       break;
  }
   
    }
public override void Update() {
    base.Update();
    Dirction=myTeam.OurGoal.m_vFacing;
    // if(Input.GetKeyUp(KeyCode.A))
    // {
    //     MessageDispatcher.Instance.DispatchMessage(0,this.ID,this.ID,MessageType.PlayerDefending,null);
    // }
    // if(Input.GetKeyUp(KeyCode.S))
    // {
    //     MessageDispatcher.Instance.DispatchMessage(0,this.ID,this.ID,MessageType.PlayerAttacking,null);
    // }
    // if(Input.GetKeyUp(KeyCode.D))
    // {
    //     MessageDispatcher.Instance.DispatchMessage(0,this.ID,this.ID,MessageType.PlayerGoHome,null);
    // }
    if(allowCodeFlow)
    {
    if(DefendTarget!=null)
    {
    LeftShiftKey=DefendTarget.LeftShiftKey;
    }
    else
    {
    if(Opp_DefendedByPlayer!=null)
    {
        LeftShiftKey=Opp_DefendedByPlayer.LeftShiftKey;
    }
    else
    {
        LeftShiftKey=false;
    }
    }
    }
     JudgeShiftKeyValue();
    m_pStateMachine.StateMachineUpdate();
    
    SteeringForce=m_pSteering.Calculate()*LeftShiftPower*playerData.calculatePower;
   
    
    // Debug.Log(m_pStateMachine.CurrentState().GetName());
   StateName=GetFSM().m_pCurrentState.GetName();
}


      StateMachine<FieldPlayer> m_pStateMachine;
    public override bool HandleMessage(Telegram msg)
{
    Debug.Log("handleMessage");
    return m_pStateMachine.HandleMessage(msg);
}

 public new StateMachine<FieldPlayer> GetFSM(){return m_pStateMachine;}

}
