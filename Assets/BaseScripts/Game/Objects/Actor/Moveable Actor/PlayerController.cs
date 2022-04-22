using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : PlayerBase
{

    
    public bool PassKey;
    public bool ForceKey;
    public bool ForceKeyHaveDownWhenPass;
    public float PassKeyDownTime;
    public bool IsRecordTheTime=false;
    public bool ChaseBallKey;
    public float KickPower;
    public float currentPassPower;
    [Header("函数系数")]
    public float k;
    public float b;
    public float c;
    public float tm;
    public float mp;
    public float tn;
    public float np;
    public float t;
    
    StateMachine<PlayerController> m_pStateMachine;
     
    //构造函数
    // public FieldPlayer(SoccerTeam myTeam):base(myTeam)
    // {
    
    // }
    public override void  Start()
    {
base.Start();
 m_pStateMachine= new StateMachine<PlayerController>(this);
 m_pStateMachine.SetCurrentState(PlayerControllerChaseBall.Instance);
  m_pStateMachine.SetGlobalState(GlobalPlayerControllerState.Instance);
  m_pStateMachine.SetPreviousState(null);
   m_pSteering.seek=true;
  mp=playerData.MaxPassingForce;
  np=playerData.NicePassingForce;
  tn=playerData.NicePassingTime;
  tm=playerData.MaxPassingTime;
  MakePassForceAndTimeFunc();
    }
public override void Update() {
    base.Update();
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
    m_pStateMachine.StateMachineUpdate();
    if(calculate)
    {
    SteeringForce=m_pSteering.Calculate()*LeftShiftPower*playerData.calculatePower;
    }
    else
    {
        SteeringForce=Vector3.zero;
    }
   
    InputUpdate();
    JudgeCalculate();
     JudgeShiftKeyValue();
    // Debug.Log(m_pStateMachine.CurrentState().GetName());
    CalculatePassForce();
     StateName=GetFSM().m_pCurrentState.GetName();
  
}
//传球力度与时间函数的求解
public void MakePassForceAndTimeFunc()
{ 
    k=Mathf.Sqrt((Mathf.Pow(mp,2)/tm));
        // k=mp/tm;
    // k=(np-(mp-k*Mathf.Pow(tm,2f))/tm*tn)/Mathf.Pow(tn,2f);
    // b=(mp-k*Mathf.Pow(tm,2f))/tm;
// k=(mp-1/2f*np*Mathf.Pow(tm,2f)-np*tm+np*tm*tn)/(1/6f*Mathf.Pow(tm,3f)-1/2f*Mathf.Pow(tm,2f)*tn-1/2f*Mathf.Pow(tn,2f)*tm+Mathf.Pow(tn,2f)*tm);
// b=np-k*tn;
// c=np-1/2f*k*Mathf.Pow(tn,2f)-(np-k*tn)*tn;
}
public float GetValueFromMakePassFunc(float t)
{
    return k*Mathf.Sqrt(t);
// return (1/6f*k*Mathf.Pow(t,3f)+1/2*b*Mathf.Pow(t,2f)+c*t);
// return k*Mathf.Pow(t,2f)+b*t;
}
/*关于操控的方法*/
public void JudgeCalculate()
{
     if(Dirction!=Vector3.zero)
        {
            calculate=true;
        }
        else
        {
            calculate=false;
        }
}
//shift


      //wasd的输入
      void InputUpdate()
    {
        Dirction=Vector3.zero;
        if(Input.GetKey(KeyCode.A))
        {
            Dirction+=Vector3.left;
        }
        if(Input.GetKey(KeyCode.W))
        {
            Dirction+=Vector3.forward;
        }
        if(Input.GetKey(KeyCode.D))
        {
            Dirction+=Vector3.right;
        }if(Input.GetKey(KeyCode.S))
        {
            Dirction+=Vector3.back;
        }
        if(Input.GetKey(KeyCode.LeftShift))
        {
          LeftShiftKey=true;
        }
        else
        {
            LeftShiftKey=false;
        }
        if(Input.GetKeyDown(KeyCode.B))
        {
            if(PassKey==true)
            {
             PassKey=false;
            }
            else
            {
            PassKey=true;
            }
            
        }
        
          if(Input.GetKeyDown(KeyCode.N))
        {
           ForceKey=true;
        }
       if(Input.GetKeyUp(KeyCode.N))
       {
           ForceKey=false;
       }
        if(Input.GetKey(KeyCode.C))
         {
        ChaseBallKey=true;
        }
        else
        {
            ChaseBallKey=false;
        }
    }
    public void CalculatePassForce()
    {
       
        if(PassKey)
        {
           
            if(!IsRecordTheTime)
            {
            PassKeyDownTime=Time.time;
            IsRecordTheTime=true;
            }
               t=Time.time-PassKeyDownTime;
              currentPassPower=Mathf.Min(GetValueFromMakePassFunc(t),playerData.MaxPassingForce);
              if(!ForceKeyHaveDownWhenPass)
              {
                  KickPower=currentPassPower;
              }
              if(ForceKey)
              {
                
            ForceKeyHaveDownWhenPass=true;
          
                  
              }
        }
        else
        {
              KickPower=0f;
             PassKeyDownTime=0f;
            IsRecordTheTime=false;
             ForceKeyHaveDownWhenPass=false;
        }
    }
    public override bool HandleMessage(Telegram msg)
{
    Debug.Log("handleMessage");
    return m_pStateMachine.HandleMessage(msg);
}

 public new StateMachine<PlayerController> GetFSM(){return m_pStateMachine;}

}
