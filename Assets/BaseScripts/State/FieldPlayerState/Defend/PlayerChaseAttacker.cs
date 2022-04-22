using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChaseAttacker : State<FieldPlayer>
{
      public string Name="PlayerChaseAttacker";
    
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
         
        //进入这个状态前就已经判断是最近的球员
   //有可能已经有人追球然后我反而是最近的球员 那么就会出现 本来第二近的防守的球员退出状态
    if(player.myTeam.IsOurTeamMember(player.m_pBall.m_pOwner))
    {
        
         player.GetFSM().ChangeState(PlayerWait.Instance);
        return;
    }
    else
    {
        //TODO:判断有没有defendtarget 有可能是从cutball里面回来的
         //设置我要防守的目标
          
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
        if(player.DefendTarget.Opp_DefendedByPlayer==null||player.DefendTarget.Opp_DefendedByPlayer==player)
        {
       
    
        //给对方知道我在防守它
    player.DefendTarget.Opp_DefendedByPlayer=player;
        }
        else
        {
            //已经判定正在追敌人的人没有退出状态 
            //那么它起码是除我之外最近的球员 所以我就判断是不是需要去支援 看看支援的球员有没有退出状态
            if(player.DefendTarget.Opp_DefendedSupportByPlayer==null||player.DefendTarget.Opp_DefendedSupportByPlayer==player)
            {
                player.GetFSM().ChangeState(PlayerDefendSupport.Instance);
                player.DefendTarget=null;
                return;
            }
          else
          {
               player.GetFSM().ChangeState(PlayerDefending.Instance);
                player.DefendTarget=null;
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
      if(!player.DefendTarget)
      {
player.GetFSM().ChangeState(PlayerDefending.Instance);
      }
        if(player.myTeam.m_pPlayerClosestToBall!=player&&player.myTeam.m_pDefendSupportingPlayer!=player)
        {
            Debug.Log("out");
           player.GetFSM().ChangeState(PlayerDefending.Instance);
               player.DefendTarget.Opp_DefendedByPlayer=null;
               player.DefendTarget=null;
           return;
        }
        Vector3 FutureBallPos=player.m_pBall.FuturePosition(player.playerData.PredictTime);
        Vector3 BallToOurGoal=player.myTeam.OurGoal.m_pos-FutureBallPos;
        Vector3 Target=FutureBallPos+BallToOurGoal.normalized*player.playerData.DefendKeepDistance;
        Vector3 PlayerToBall=(player.m_pBall.M_Pos()-player.M_Pos());
         float dot=Vector3.Dot(player.M_vHeading(),PlayerToBall);
       
        
        player.m_pSteering.SetArriveTarget(Target,Deceleration.fast);
        //如果球进入了可以截球的距离 那么就执行截球
      
         if(PlayerToBall.magnitude<player.playerData.CutBallRange||(Random.Range(0f,1f)<player.playerData.AttemptToCutBallPosibility&&player.allowCodeFlow))
        {
           player.GetFSM().ChangeState(PlayerCutBall.Instance);
           if(player.DefendTarget)
           {
               player.DefendTarget.Opp_DefendedByPlayer=null;
           }
               
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
    private static PlayerChaseAttacker instance;
     public static PlayerChaseAttacker Instance
     {
get{
    if(instance==null)
    {
        instance=new PlayerChaseAttacker();
    }
    return instance;
}
     }
}
