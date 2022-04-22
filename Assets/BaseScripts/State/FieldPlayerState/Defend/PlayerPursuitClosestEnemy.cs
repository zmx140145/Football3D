using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPursuitClosestEnemy : State<FieldPlayer>
{
      public string Name="PlayerPursuitClosestEnemy";
    private Region region;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override bool OnMessage(FieldPlayer entity_Type, Telegram telegram)
    {
        
      
        return false;
    }
    public override void Enter(FieldPlayer player)
    {
    
if(player.DefendPrsuitTarget==null)
{
    player.DefendPrsuitTarget=player.myTeam.OpponentsTeam.NearestMemberForPlayerInOurTeam(player);
}
//如果我要准备追随的人的记录的追随的人不为空
//那么就说明已经有人在追它了
//那么就不需要追它 就回到防守状态
//感觉这个要在改变状态就要判断 不然就会老是进入这转态 跳来跳去 ps:已经修改了
if(player.DefendPrsuitTarget.Opp_PursuitedByPlayer!=null&&player.DefendPrsuitTarget.Opp_PursuitedByPlayer!=player)
{
player.GetFSM().ChangeState(PlayerDefending.Instance);
return;
}
else
{
    //那么就告诉它追它的人是我
    player.DefendPrsuitTarget.Opp_PursuitedByPlayer=player;
}
player.m_pSteering.seek=false;
player.m_pSteering.arrive=true;
player.playerLastPos=player.M_Pos();
    }
     public override void Execute(FieldPlayer player)
    {
        Vector3 FuturePos=player.DefendPrsuitTarget.M_Pos()+player.DefendPrsuitTarget.M_vVelocity();
        
        Vector3 target=FuturePos+(player.myTeam.OurGoal.m_pos-FuturePos).normalized*player.playerData.DefendPursuitKeepDistance;
        if(!player.DefendTarget)
        {
        if((target-player.DefendRegion.areaPos).magnitude>player.playerData.DefendPursuitRange)
        {
            Vector3 OppToBall=(player.m_pBall.M_Pos()-player.DefendPrsuitTarget.M_Pos()).normalized;
            target=(player.DefendPrsuitTarget.M_Pos()+OppToBall*player.playerData.DefendPursuitKeepDistance);
        }
        player.m_pSteering.SetArriveTarget(target,Deceleration.fast);
        //超出了范围就不追了
        if((player.M_Pos()-player.DefendRegion.areaPos).magnitude>player.playerData.DefendPursuitRange)
        {
            player.GetFSM().ChangeState(PlayerDefending.Instance);
            return;
        }
        }
        else
        {
             player.m_pSteering.SetArriveTarget(target,Deceleration.fast);
            if((player.M_Pos()-player.DefendTarget.M_Pos()).magnitude>player.playerData.DefendPursuitRange)
            {
                 player.GetFSM().ChangeState(PlayerDefending.Instance);
                 return;
            }
        }
        if(player.allowCodeFlow)
        {
            PlayerBase pb=player.myTeam.NearestMemberForPlayerInOurTeam(player.DefendPrsuitTarget);
        //如果他去到了队员的身边并且这个队员不是正在抢球的队友
        if(pb!=player&&pb!=player.myTeam.m_pPlayerClosestToBall)
        {
            //TODO:可能改成进入谁的防区通知谁去防守
            //那么就回啊自己防守的位置 并发送消息通知队友去盯防
            player.GetFSM().ChangeState(PlayerDefending.Instance);
            
            MessageDispatcher.Instance.DispatchMessage(0,player.ID,pb.ID,MessageType.PursuitEnemy,player.DefendPrsuitTarget);
            return;
        }
        }
    }
     public override void Exit(FieldPlayer player)
    {
         player.m_pSteering.InitBool();
        player.DefendPrsuitTarget.Opp_PursuitedByPlayer=null;
        player.DefendPrsuitTarget=null;
        player.DefendTarget=null;
    }
    public override string GetName()
    {
        return Name;
    }
    private static PlayerPursuitClosestEnemy instance;
     public static PlayerPursuitClosestEnemy Instance
     {
get{
    if(instance==null)
    {
        instance=new PlayerPursuitClosestEnemy();
    }
    return instance;
}
     }
}
