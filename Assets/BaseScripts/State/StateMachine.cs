using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<Entity_type>  where Entity_type:BaseGameEntity
{
   private Entity_type m_pOwner;
   public State<Entity_type> m_pCurrentState;
   private State<Entity_type> m_pPreviousState;
   
   private State<Entity_type> m_pGlobalState;
   //构造函数
   public StateMachine(Entity_type owner)
   {
 
m_pOwner=owner;
m_pCurrentState=null;
m_pPreviousState=null;
m_pGlobalState=null;
   }
   //用这些方法可以初始化fsm
  
   public void SetCurrentState(State<Entity_type> s)
   {
      
       m_pCurrentState=s;
   }
    public void SetPreviousState(State<Entity_type> s)
   {
       m_pPreviousState=s;
   }
    public void SetGlobalState(State<Entity_type> s)
   {
       m_pGlobalState=s;
   }
  public void StateMachineUpdate()
  {
   
      //如果全局状态存在就执行
      if(m_pGlobalState!=null)

      {
          m_pGlobalState.Execute(m_pOwner);
      }
      //如果当前状态存在就执行
      if(m_pCurrentState!=null)m_pCurrentState.Execute(m_pOwner);

  }
    public void ChangeState(State<Entity_type> pNewState)
    {
        Debug.Log("in state change");
        //如果当前有状态退出当前
        if(m_pCurrentState!=null)
        m_pCurrentState.Exit(m_pOwner);
        //赋新值
        m_pPreviousState=m_pCurrentState;
        m_pCurrentState=pNewState;
        //进入新状态
        m_pCurrentState.Enter(m_pOwner);
    }
    public void RevertToPreviousState()
    {
        if(m_pPreviousState!=null)
        ChangeState(m_pPreviousState);
    }
    public State<Entity_type> CurrentState(){return m_pCurrentState;}
    public State<Entity_type> PreviousState(){return m_pPreviousState;}
    public State<Entity_type> GlobalState(){return m_pGlobalState;}
    public bool isInState(State<Entity_type> state)
    {
        if(state==m_pCurrentState)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    //消息处理
    public bool HandleMessage(Telegram msg)
    {
        //消息对于是当前状态的
        if(m_pCurrentState!=null&&m_pCurrentState.OnMessage(m_pOwner,msg))
        {
            return true;
        }
        //消息是对于全局状态的
        if(m_pGlobalState!=null&&m_pGlobalState.OnMessage(m_pOwner,msg))
        {
            return true;
        }
        return false;
        
    }
} 
