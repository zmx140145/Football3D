 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrepareForKickOff : State<SoccerTeam>
{
    // Start is called before the first frame update
    private string Name="PrepareForKickOff";
     public override string GetName()
    {
        return Name;
    }
    void Start()
    {
        
    }

    // Update is called once per fra                                                                                                            
    void Update()
    {
        
    }
     public override bool OnMessage(SoccerTeam entity_Type, Telegram telegram)
    {
      return  false;
    }
    public override void Enter(SoccerTeam team)
    {
    team.SetControllingPlayer(null);
    team.m_pSupportingPlayers.Clear();
    team.SetReceivingPlayer(null);
    team.SetPlayerClosestToBall(null);
  //给每一个队员发生Msg_GoHome的消息
          team.ReturnAllFieldPlayersToHome();
  
    }
     public override void Execute(SoccerTeam team)
    {
     //如果两个球队都到位了，开始游戏
     SoccerPitch.Instance.m_bGameOn=false;
     if(team.AllPlayersAtHome()&&team.OpponentsTeam.AllPlayersAtHome())
     {
         team.GetFSM().ChangeState(TeamDefending.Instance);
     }
       
    }
     public override void Exit(SoccerTeam team)
    {
        SoccerPitch.Instance.m_bGameOn=true;
    }
    private static PrepareForKickOff instance;
     public static PrepareForKickOff Instance
     {
get{
    if(instance==null)
    {
        instance=new PrepareForKickOff();
    }
    return instance;
}
     }
}
