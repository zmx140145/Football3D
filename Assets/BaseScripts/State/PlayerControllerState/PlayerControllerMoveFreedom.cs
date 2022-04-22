using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerMoveFreedom : State<PlayerController>
{
      public string Name="PlayerControllerMoveFreedom";
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
       
    player.m_pSteering.arrive=false;
    player.m_pSteering.seek=true;
    player.m_pSteering.pursuit=false;
    player.m_pSteering.flee=false;
    player.m_pSteering.evade=false;

    }
     public override void Execute(PlayerController player)
    {  
        Vector3 Target=Vector3.zero;
       Target=player.M_Pos()+player.Dirction*player.playerData.MaxSpeed;
       if(player.ChaseBallKey)
       {
      player.m_pSteering.SetArriveTarget(player.m_pBall.M_Pos()+player.m_pBall.M_vVelocity(),Deceleration.fast);
       }
       else
       {
          player.m_pSteering.SetArriveTarget(Target,Deceleration.fast);
       }
      
       if(player.BallWithinReceiverRange())
       {
           player.GetFSM().ChangeState(PlayerControllerChaseBall.Instance);
           return;
       }
    }
     public override void Exit(PlayerController player)
    {
        
    }
    public override string GetName()
    {
        return Name;
    }
    private static PlayerControllerMoveFreedom instance;
     public static PlayerControllerMoveFreedom Instance
     {
get{
    if(instance==null)
    {
        instance=new PlayerControllerMoveFreedom();
    }
    return instance;
}
     }
}