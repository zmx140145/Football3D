using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacking : State<FieldPlayer>
{
    public string Name="PlayerAttacking";
    
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
        //设置player不在防守 在进攻
     player.isDefending=false;
        player.isAttacking=true;
     
SoccerPitch.Instance.m_RegionsList.TryGetValue(player.AttackAreaNum,out player.AttackRegion);
    }
     public override void Execute(FieldPlayer player)
    { 
       if(player==player.myTeam.m_pControllingPlayer)
       {
           player.GetFSM().ChangeState(PlayerDribbleBall.Instance);
           return;
       }
       if(player.isClosestTeamMemberToBall())
       {
           player.GetFSM().ChangeState(PlayerChaseBall.Instance);
           return;
       }
        if(player.EnterAreaNum!=player.AttackAreaNum)
        {
       Region region=player.AttackRegion;
         if(region!=null)
         {
           
         player.m_pSteering.SetArriveTarget(region.areaPos,Deceleration.fast);
        
         player.m_pSteering.arrive=true;
         return;
         }
         else
         {

       player.GetFSM().ChangeState(PlayerWait.Instance);
       return;
         }
        }
        else
        {
         
          player.GetFSM().ChangeState(PlayerWait.Instance);
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
    private static PlayerAttacking instance;
     public static PlayerAttacking Instance
     {
get{
    if(instance==null)
    {
        instance=new PlayerAttacking();
    }
    return instance;
}
     }
}
