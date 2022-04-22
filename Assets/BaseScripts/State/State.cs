using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//要求是entity的状态
//状态模板
public abstract class State<Entity_type> where Entity_type:BaseGameEntity
{
    //进入状态
   public virtual void Enter(Entity_type t){}
   //执行状态
   public virtual void Execute(Entity_type t){}
   //退出状态
   public virtual void Exit(Entity_type t){}
   //如果智能体从消息发射器中接收了一条消息时就执行这个
   public abstract bool OnMessage(Entity_type entity_Type,Telegram telegram);
  
   
public abstract string GetName();
   
  

}
