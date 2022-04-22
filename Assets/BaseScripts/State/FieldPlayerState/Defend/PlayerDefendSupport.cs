using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDefendSupport : State<FieldPlayer>
{
   
      public string Name="PlayerDefendSupport";
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
 
 if(player.myTeam.IsOurTeamMember(player.m_pBall.m_pOwner))
    {
        player.GetFSM().ChangeState(PlayerWait.Instance);
        return;
    }
    else
    {
        
player.DefendTarget=player.m_pBall.m_pOwner;
//如果还是没赋值
if(!player.DefendTarget)
{
    //球没有人控制
player.DefendTarget=player.myTeam.OpponentsTeam.m_pControllingPlayer;
if(!player.DefendTarget)
{
    //对面没有人控球
    player.GetFSM().ChangeState(PlayerSnatchBall.Instance);
    return;
}

        }
       
        
         //TODO:判断有没有DT
         //如果我要支援协防的目标所记录的被谁协防 那个谁不是我或者不为空 那么就退出
          if( player.DefendTarget.Opp_DefendedSupportByPlayer==null|| player.DefendTarget.Opp_DefendedSupportByPlayer==player)
          {

                player.DefendTarget.Opp_DefendedSupportByPlayer=player;
                 player.DefendSupportTarget=player.DefendTarget.myTeam.NearestMemberForPlayerInOurTeam(player.DefendTarget);
          }
          else
          {
               //已经判定正在支援的人没有退出状态 
            //那么它起码是除我之外最近的球员 所以我就判断是不是需要去支援 看看主要防守的球员有没有退出状态
            if(player.DefendTarget.Opp_DefendedByPlayer==null||player.DefendTarget.Opp_DefendedByPlayer==player)
            {
                player.GetFSM().ChangeState(PlayerChaseAttacker.Instance);
                return;
            }
          else
             {
              player.GetFSM().ChangeState(PlayerDefending.Instance);
             return;
             }
               
          }
        
   
   
    
    }
    player.m_pSteering.seek=false;
    player.m_pSteering.arrive=true;
     
    }
   

     public override void Execute(FieldPlayer player)
    {
       //如果自己已经不是最近或者第二近的球员  那么就切换到defending状态 让denfending去判断需要追球还是支援
      if(player.DefendTarget==null)
      {
          player.DefendTarget=player.m_pBall.m_pOwner;
//如果还是没赋值
if(!player.DefendTarget)
{
    //球的owner是空的
player.DefendTarget=player.myTeam.OpponentsTeam.m_pControllingPlayer;
if(!player.DefendTarget)
{
    //对面没有在控制球
    player.GetFSM().ChangeState(PlayerSnatchBall.Instance);
    return;
}
      }
      }
        if(player.myTeam.m_pPlayerClosestToBall!=player&&player.myTeam.m_pDefendSupportingPlayer!=player)
        {
           player.GetFSM().ChangeState(PlayerDefending.Instance);
            player.DefendTarget.Opp_DefendedSupportByPlayer=null;
        player.DefendSupportTarget=null;
        player.DefendTarget=null;
        
           return;
        }
        //如果supportTarget与player在同一区域范围内 那就追随
        if(player.DefendSupportTarget.EnterAreaNum==player.EnterAreaNum)
        {
            //如果对方是没有人盯防的那么就去盯防   但是如果是有人盯防的那么就要等那个人发通知叫我盯防
            if(player.DefendSupportTarget.Opp_PursuitedByPlayer==null)
            {
         player.DefendPrsuitTarget=player.DefendSupportTarget;
         
         player.GetFSM().ChangeState(PlayerPursuitClosestEnemy.Instance);
          player.DefendTarget.Opp_DefendedSupportByPlayer=null;
        player.DefendSupportTarget=null;
        player.DefendTarget=null;
        //这里没有设置DefendTarget为空
        //是为了让pursuit里面可以知道这是根据当时位置要跟随的，而不是防守区域
            }
         return;
        }
        //并不需要每秒更新
        if(player.allowCodeFlow)
        {
         player.DefendSupportTarget=player.DefendTarget.myTeam.NearestMemberForPlayerInOurTeam(player.DefendTarget);
        }
        //那么就开始防守位置计算
        Vector3 direction=(player.DefendSupportTarget.M_Pos()-player.DefendTarget.M_Pos()).normalized;
        Vector3 ToGoal=(player.myTeam.OurGoal.m_pos-player.DefendTarget.M_Pos()).normalized;
        Vector3 target=player.DefendTarget.M_Pos()+(2f*direction+ToGoal)/3f*player.playerData.DefendKeepDistance;
        Vector3 PlayerToBall=player.m_pBall.M_Pos()-player.M_Pos();
        player.m_pSteering.SetArriveTarget(target,Deceleration.fast);
        float dot=Vector3.Dot(player.M_vHeading(),PlayerToBall);
       
         if(PlayerToBall.magnitude<player.playerData.CutBallRange)
        {
           player.GetFSM().ChangeState(PlayerCutBall.Instance);
            player.DefendTarget.Opp_DefendedSupportByPlayer=null;
        player.DefendSupportTarget=null;
        
        return;
        }
        
    }
     public override void Exit(FieldPlayer player)
    {
      player.m_pSteering.InitBool();
       
    }
   
    public override string GetName()
    {
        return Name;
    }
    private static PlayerDefendSupport instance;
     public static PlayerDefendSupport Instance
     {
get{
    if(instance==null)
    {
        instance=new PlayerDefendSupport();
    }
    return instance;
}
     }
}
