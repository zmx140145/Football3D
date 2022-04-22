using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalPlayerControllerState : State<PlayerController>
{
   // Start is called before the first frame update
   private string Name="GlobalPlayerControllerState";
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override bool OnMessage(PlayerController player, Telegram telegram)
    {
       switch(telegram.Msg)
       {
           case MessageType.PlayerAttacking:
           {
           
           break;
           }
           case MessageType.PlayerDefending:
            {
            
           break;
           }
           case MessageType.PlayerGoHome:
            {
              
                return true;
           
           }
           case MessageType.PlayerRececeiveBall:
           {
               //设置目标
            player.m_pSteering.arrive=true;
               player.m_pSteering.SetArriveTarget((Vector3)telegram.ExtraInfo,Deceleration.fast);
               player.GetFSM().ChangeState(PlayerControllerReceiveBall.Instance);
               break;
           }
            
             case MessageType.PlayerWait:
           {
              player.GetFSM().ChangeState(PlayerControllerWait.Instance);
              return true;
        
           }
             
       }
        return false;
    }
    public override void Enter(PlayerController player)
    {
    

    }
     public override void Execute(PlayerController player)
    {
    //如果队员占有并接近球，那么降低他的最大速度
    //TODO:要把它和ai球员分开
    if(player.myTeam.m_pControllingPlayer==player)
    {
player.SetMaxSpeed(player.playerData.MaxSpeedWithBall*player.LeftShiftWithBallPower);
    }
    else
    {
        player.SetMaxSpeed(player.playerData.MaxSpeed*player.LeftShiftPower);
    }

    }
     public override void Exit(PlayerController player)
    {
        
    }
    public override string GetName()
    {
        return Name;
    }
    private static GlobalPlayerControllerState instance;
     public static GlobalPlayerControllerState Instance
     {
get{
    if(instance==null)
    {
        instance=new GlobalPlayerControllerState();
    }
    return instance;
}
     }
}
