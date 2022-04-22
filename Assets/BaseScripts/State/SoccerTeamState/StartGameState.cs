 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameState : State<SoccerTeam>
{
    // Start is called before the first frame update
    private string Name="StartGameState";
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
   
  
    }
     public override void Execute(SoccerTeam team)
    {
     
       team.GetFSM().ChangeState(PrepareForKickOff.Instance);
    }
     public override void Exit(SoccerTeam team)
    {
        
    }
    private static StartGameState instance;
     public static StartGameState Instance
     {
get{
    if(instance==null)
    {
        instance=new StartGameState();
    }
    return instance;
}
     }
}
