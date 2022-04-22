using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MessageDispatcher :Singleton<MessageDispatcher>
{
   private Queue<Telegram> PriorityQ;
   //调用这方式让实体接收信息
   private void DisCharge(BaseGameEntity pReceiver,Telegram msg)
   {
pReceiver.HandleMessage(msg);
   }
   public void DispatchMessage(double delay,int Sender,int receiver,MessageType msg,System.Object ExtraInfo)
   {
BaseGameEntity pReceiver=EntityManager.Instance.GetEntityFromID(receiver);
//创建信息
Telegram telegram=CreateTelegram(delay,Sender,receiver,msg,ExtraInfo);
if(delay<=0f)
{
    //发送telegram到接收器
    DisCharge(pReceiver,telegram);
    
}
else
{
    double CurrentTime=Time.time;
    telegram.DispatchTime=CurrentTime+delay;
    PriorityQ.Enqueue(telegram);
}

   }
   private Telegram CreateTelegram(double delay,int Sender,int receiver,MessageType msg,System.Object ExtraInfo)
   {
       Telegram telegram;
       telegram.DispatchTime=delay;
       telegram.Sender=Sender;
       telegram.Receiver=receiver;
       telegram.Msg=msg;
       telegram.ExtraInfo=ExtraInfo;
       return telegram;
   }
   //发送任何延迟信息，主游戏中update更新
   public void DispatchDelayMessage()
   {
double CurrentTime=Time.time;
while((PriorityQ.Peek().DispatchTime<CurrentTime)&&PriorityQ.Peek().DispatchTime>0)
{
//从队列的前面读取telegram
Telegram telegram=PriorityQ.Dequeue();
BaseGameEntity pReceiver=EntityManager.Instance.GetEntityFromID(telegram.Receiver);
DisCharge(pReceiver,telegram);

}
}
}
