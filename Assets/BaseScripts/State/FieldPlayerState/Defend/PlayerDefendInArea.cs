using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDefendInArea : State<FieldPlayer>
{
      public string Name="PlayerDefendInArea";
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
    player.m_pSteering.seek=false;

    }
     public override void Execute(FieldPlayer player)
    {
         //如果还没到达指定的区域就前往区域
         //只有到了指定区域才会进入等待状态
         //优先考虑是否防守
         if(player.isClosestTeamMemberToBall())
         {
         player.GetFSM().ChangeState(PlayerChaseAttacker.Instance);
           return;
        }
     if(player.EnterAreaNum!=player.DefendAreaNum)
        {
        Region region=player.DefendRegion;   
         if(region!=null)
         {
           
         player.m_pSteering.SetArriveTarget(region.areaPos,Deceleration.fast);
          
         
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
    private static PlayerDefendInArea instance;
     public static PlayerDefendInArea Instance
     {
get{
    if(instance==null)
    {
        instance=new PlayerDefendInArea();
    }
    return instance;
}
     }
}
