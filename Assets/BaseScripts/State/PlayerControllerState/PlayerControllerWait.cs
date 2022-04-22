using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerWait : State<PlayerController>
{
      public string Name="PlayerControllerWait";
    private Region region;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override bool OnMessage(PlayerController entity_Type, Telegram telegram)
    {
        
      
        return false;
    }
    public override void Enter(PlayerController player)
    {
        //关闭所有的行为 
        //让球员停下来
    player.m_pSteering.arrive=false;
    player.m_pSteering.seek=false;
    player.m_pSteering.pursuit=false;
    player.m_pSteering.flee=false;
    player.m_pSteering.evade=false;

    }
     public override void Execute(PlayerController player)
    {  
        //如果是控球就执行进攻的模块
        if(player.myTeam.BallInControll())
        {
        //如果该球员不在arrive的位置，那么就会到位置
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
        //如果球员的球队正在控制着球
        //如果球员不是控球球员
        //并且该球员位置更靠近前场
        //那么请求传球
        if(player.BallOutOfViewRange())
        {
           player.GetFSM().ChangeState(PlayerControllerMoveFreedom.Instance);
            return;
        }
        else
        {
        if(player.myTeam.BallInControll()&&(!player.isControllingPlayer())&&player.isAheadOfAttacker())
        {
            player.myTeam.RequestPass(player);
            return;
        }
        }
        if(SoccerPitch.Instance.isGameOn())
        {
            //如果该队员是球队中离球最近的，球队也没有分配接球队员，守门员没有拿到球，那么去追球
            if(player.isClosestTeamMemberToBall()&&player.myTeam.m_pReceivingPlayer==null&&!SoccerPitch.Instance.GoalKeeperHasBall())
            {
                player.GetFSM().ChangeState(PlayerControllerChaseBall.Instance);
                return;
            }
        }
        }
        else
        {

        }
    }
     public override void Exit(PlayerController player)
    {
        
    }
    public override string GetName()
    {
        return Name;
    }
    private static PlayerControllerWait instance;
     public static PlayerControllerWait Instance
     {
get{
    if(instance==null)
    {
        instance=new PlayerControllerWait();
    }
    return instance;
}
     }
}