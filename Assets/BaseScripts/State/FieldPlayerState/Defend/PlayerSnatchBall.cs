using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSnatchBall : State<FieldPlayer>
{
   
      public string Name="PlayerSnatchBall";
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
    

    
    player.m_pSteering.arrive=true;
    
    }
     public override void Execute(FieldPlayer player)
    {
        Vector3 BallFuturePos=player.m_pBall.FuturePosition(player.playerData.PredictTime);
        
        player.m_pSteering.SetArriveTarget(BallFuturePos,Deceleration.fast);
        //如果我们球队控球了那么就让他到attacking去
        if(player.myTeam.BallInControll())
        {
            player.GetFSM().ChangeState(PlayerAttacking.Instance);
        }
        float distance=(player.m_pBall.M_Pos()-player.M_Pos()).magnitude;
    
       if(distance<player.playerData.CutBallRange)
       {
           if(player.isReadyForNextKick())
           {
           player.GetFSM().ChangeState(PlayerCutBall.Instance);
           return;
           }
       }
       if(player.isClosestTeamMemberToBall())
       {
       if(player.m_pBall.m_pOwner==player.myTeam.OpponentsTeam.m_pControllingPlayer)
       {
            player.GetFSM().ChangeState(PlayerChaseAttacker.Instance);
           player.DefendTarget=player.m_pBall.m_pOwner;
       }
       }
       else
       {
           player.GetFSM().ChangeState(PlayerDefendSupport.Instance);
           player.DefendTarget=null;
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
    private static PlayerSnatchBall instance;
     public static PlayerSnatchBall Instance
     {
get{
    if(instance==null)
    {
        instance=new PlayerSnatchBall();
    }
    return instance;
}
     }
}
