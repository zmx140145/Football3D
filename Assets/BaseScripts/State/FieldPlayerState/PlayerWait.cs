using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWait : State<FieldPlayer>
{
      public string Name="PlayerWait";
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
        //关闭所有的行为 
        //让球员停下来
    player.m_pSteering.arrive=false;
    player.m_pSteering.seek=false;
    player.m_pSteering.pursuit=false;
    player.m_pSteering.flee=false;
    player.m_pSteering.evade=false;

    }
     public override void Execute(FieldPlayer player)
    {  
         //如果该球员不在arrive的位置，那么就会到位置
       
        //如果球员的球队正在控制着球
         if(!player.AtTarget())
        {
            player.m_pSteering.arrive=true;
            return;
        }
        else
        {
            player.m_pSteering.arrive=false;
            player.SetVelocity(Vector3.zero);
            //让球员盯着球
            player.TrackBall();
        }

       if(SoccerPitch.Instance.isGameOn())
        {
        //如果是控球就执行进攻的模块
        /*进攻模块*/
        if(player.myTeam.BallInControll())
        {
       if(player.isClosestTeamMemberToBall())
       {
           player.GetFSM().ChangeState(PlayerChaseBall.Instance);
           return;
       }
       
        //如果球员不是控球球员
        //并且该球员位置更靠近前场
        //那么请求传球
        //并且可以射门 然后是球员有自信拿球
        //要优先处理可以射门的传球请求  
        //TODO:如果后期发现球队一直不传威胁球 那么把这个传球请求写成一个队列  隔一段时间处理一次传球信息 找最好的传球方案
        if((!player.isControllingPlayer()))
        {
             if(player.isAheadOfAttacker())
             {  
                 
        
            if(player.isThreatened()<player.playerData.ThreatenedPassFactor)
            {
                Vector3 ShootTarget=Vector3.zero;
           if(player.myTeam.CanShoot(player.M_Pos(),Player.Instance.player_SO.MaxShootingForce,ref ShootTarget))
           {
          player.myTeam.RequestPass(player);
           return;
            }
           
          
            }
        }
        //如果球员认为被盯住了
        //那么再次跑完去support
        if(player.isThreatened()>player.playerData.ThreatenedPassFactor)
        {
        
        player.GetFSM().ChangeState(PlayerSupportAttacker.Instance);
        return;
        }
        if(player.isThreatened()==0)
        {
          player.myTeam.RequestPass(player);
           return;
        }
        }
        
       
            //如果该队员是球队中离球最近的，球队也没有分配接球队员，守门员没有拿到球，那么去追球
            if(player.isClosestTeamMemberToBall()&&player.myTeam.m_pReceivingPlayer==null&&!SoccerPitch.Instance.GoalKeeperHasBall())
            {
                player.GetFSM().ChangeState(PlayerChaseBall.Instance);
                return;
            }
        }
        

        //这是防守的部分
        /*防守模块*/
        else
        {
            
if(player==player.myTeam.m_pPlayerClosestToBall)
{
player.GetFSM().ChangeState(PlayerChaseAttacker.Instance);
return;
}
if(player==player.myTeam.m_pDefendSupportingPlayer)
{
player.GetFSM().ChangeState(PlayerDefendSupport.Instance);
return;
}
            
    player.m_pSteering.SetWander(1f,3f,1f);
    player.m_pSteering.wander=true;
    //如果有敌人进入我的防区 那么就跟随他
    if(player.NeedDefend)
    {
    if(player.IsRegionAreaHaveOppTeamMember(player.EnterAreaNum))
    {
        if(player.DefendPrsuitTarget.Opp_PursuitedByPlayer==null)
        {
        player.GetFSM().ChangeState(PlayerPursuitClosestEnemy.Instance);
        player.DefendTarget=null;
        return;
        }
    }
    }

        }
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
    private static PlayerWait instance;
     public static PlayerWait Instance
     {
get{
    if(instance==null)
    {
        instance=new PlayerWait();
    }
    return instance;
}
     }
}