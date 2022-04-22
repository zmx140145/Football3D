using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamAttacking : State<SoccerTeam>
{
    // Start is called before the first frame update
    private string Name="TeamAttacking";
     public override string GetName()
    {
        return Name;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override bool OnMessage(SoccerTeam entity_Type, Telegram telegram)
    {
        
      
        return false;
    }
    public override void Enter(SoccerTeam team)
    {
    //发送信息让队员落到防守阵型
    team.AllFieldPlayersToAttackingArea();

    }
     public override void Execute(SoccerTeam team)
    {
     //如果在控球中，那么改变他的状态
     if(!team.BallInControll()&& ((team.m_pBall.M_Pos()-team.m_pPlayerClosestToBall.M_Pos()).sqrMagnitude>=(team.m_pBall.M_Pos()-team.OpponentsTeam.m_pPlayerClosestToBall.M_Pos()).sqrMagnitude))
     {
         team.GetFSM().ChangeState(TeamDefending.Instance);
     }
    
    //给接应队员提供接应点
    // team.DetermineBestSupportingPostion();
    }
     public override void Exit(SoccerTeam  team)
    {
        
    }
    private static TeamAttacking instance;
     public static TeamAttacking Instance
     {
get{
    if(instance==null)
    {
        instance=new TeamAttacking();
    }
    return instance;
}
     }
}
