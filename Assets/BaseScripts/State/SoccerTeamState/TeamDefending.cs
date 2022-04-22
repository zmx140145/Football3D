using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamDefending : State<SoccerTeam>
{
    // Start is called before the first frame update
    private string Name="TeamDefending";

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
        team.m_pControllingPlayer=null;
    //发送信息让队员落到防守阵型
    team.AllFieldPlayersToDefendingArea();

    }
     public override void Execute(SoccerTeam team)
    {
     //如果在控球中，那么改变他的状态
     if(team.BallInControll())
     {
         team.GetFSM().ChangeState(TeamAttacking.Instance);
     }
        if(team.OpponentsTeam.GetFSM().CurrentState()==TeamDefending.instance)
        {
            if((team.m_pBall.M_Pos()-team.m_pPlayerClosestToBall.M_Pos()).sqrMagnitude<(team.m_pBall.M_Pos()-team.OpponentsTeam.m_pPlayerClosestToBall.M_Pos()).sqrMagnitude)
            {
                team.GetFSM().ChangeState(TeamAttacking.Instance);
            }
        }
    }
     public override void Exit(SoccerTeam  team)
    {
        
    }
    private static TeamDefending instance;
     public static TeamDefending Instance
     {
get{
    if(instance==null)
    {
        instance=new TeamDefending();
    }
    return instance;
}
     }
}
