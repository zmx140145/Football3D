 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamGlobalState : State<SoccerTeam>
{
    // Start is called before the first frame update
    private string Name="TeamGlobalState";
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
        if(telegram.Msg==MessageType.TeamPrepareForKickOff)
        {
            entity_Type.GetFSM().ChangeState(PrepareForKickOff.Instance);
        return true;
        }
        return false;
    }
    public override void Enter(SoccerTeam team)
    {
   
    }
     public override void Execute(SoccerTeam team)
    {
   
        
    }
     public override void Exit(SoccerTeam team)
    {
        
    }
    private static TeamGlobalState instance;
     public static TeamGlobalState Instance
     {
get{
    if(instance==null)
    {
        instance=new TeamGlobalState();
    }
    return instance;
}
     }
}
