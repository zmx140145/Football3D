using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalPlayerState : State<FieldPlayer>
{
   // Start is called before the first frame update
   private string Name="GlobalPlayerState";
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override bool OnMessage(FieldPlayer player, Telegram telegram)
    {
       switch(telegram.Msg)
       {
           case MessageType.PlayerAttacking:
           {
               Debug.Log("messageinAttack");
            player.GetFSM().ChangeState(PlayerAttacking.Instance);
           return true;
           }
           case MessageType.PlayerDefending:
            {
            player.GetFSM().ChangeState(PlayerDefending.Instance);   
           return true;
           }
           case MessageType.PlayerGoHome:
            {
                if(player.GetFSM().CurrentState()!=PlayerGoHome.Instance)
                {
player.GetFSM().ChangeState(PlayerGoHome.Instance);   
                return true;
                }
                else
                {
                    return false;
                }
           
           }
           case MessageType.PlayerRececeiveBall:
           {
               //设置目标
            player.m_pSteering.arrive=true;
               player.m_pSteering.SetArriveTarget((Vector3)telegram.ExtraInfo,Deceleration.fast);
               player.GetFSM().ChangeState(PlayerReceiveBall.Instance);
               return true;
               
           }
            case MessageType.PlayerSupportAttacker:
           {
              //如果已经在接应位置，那么返回
              if(player.GetFSM().CurrentState()==PlayerSupportAttacker.Instance)
              {
              return true;
              }

              else
              {
              //设置目标为最佳接应的位置
            //   player.m_pSteering.SetArriveTarget(player.myTeam.GetSupportSpot(),Deceleration.fast);
              //改变状态
              player.GetFSM().ChangeState(PlayerSupportAttacker.Instance);
              return true;
              }
              
           }
             case MessageType.PlayerWait:
           {
              player.GetFSM().ChangeState(PlayerWait.Instance);
              return true;
        
           }
             case MessageType.PlayerPassToMe:
           {
              //得到请求传球队员的位置
            
              PlayerBase receiver=(PlayerBase)(telegram.ExtraInfo);
              
              //如果球不在球员课触及的范围，或者请求队员不具备射门的条件，该队员不能传球给请求队员
              if(!player.BallWithinKickingRange())
              {
                  return true;
              }
              //传球
              player.m_pBall.Kick(receiver.M_Pos()-player.m_pBall.M_Pos(),player.playerData.MaxPassingForce);
              
                  //通知这个接球队员， 将要开始传球
            MessageDispatcher.Instance.DispatchMessage(0,player.ID,receiver.ID,MessageType.PlayerRececeiveBall,receiver.M_Pos());
              //改变状态
              player.FindSupport();
              player.GetFSM().ChangeState(PlayerWait.Instance);
              
              return true;
           }
            case MessageType.PursuitEnemy:
            {
                player.DefendPrsuitTarget=(PlayerBase)telegram.ExtraInfo;
                if(player.GetFSM().m_pCurrentState==PlayerDefendSupport.Instance)
                {
                     player.DefendTarget=(PlayerBase)telegram.ExtraInfo;
                }
                player.GetFSM().ChangeState(PlayerPursuitClosestEnemy.Instance);
                return true;
            }
       }
        return false;
    }
    public override void Enter(FieldPlayer player)
    {
    

    }
     public override void Execute(FieldPlayer player)
    {
    //首先要判断球队有没有在控球
    if(!player.myTeam.BallInControll())
    {
        //如果球的owner是空的 
        //队员不在断球 不在抢球 不在支援
     if(player.m_pBall.m_pOwner==null&&player.GetFSM().CurrentState()!=PlayerCutBall.Instance&&player.GetFSM().CurrentState()!=PlayerChaseAttacker.Instance&&player.GetFSM().CurrentState()!=PlayerSupportAttacker.Instance)
        {
        
        //并且 球在接球范围内 那么就去夺球
        if((player.M_Pos()-player.m_pBall.M_Pos()).magnitude<player.playerData.ReceiveBallRange)
        {
        player.GetFSM().ChangeState(PlayerSnatchBall.Instance);
        return;
        }
        }
    }
    else
    {
      
        
       
    }
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
     public override void Exit(FieldPlayer player)
    {
        
    }
    public override string GetName()
    {
        return Name;
    }
    private static GlobalPlayerState instance;
     public static GlobalPlayerState Instance
     {
get{
    if(instance==null)
    {
        instance=new GlobalPlayerState();
    }
    return instance;
}
     }
}
