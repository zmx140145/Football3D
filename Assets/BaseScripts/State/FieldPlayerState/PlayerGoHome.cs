using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGoHome :State<FieldPlayer>
{
    public string Name="PlayerGoHome";
    
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
    SoccerPitch.Instance.m_RegionsList.TryGetValue(player.BaseAreaNum,out player.BaseRegion);
   
    player.m_pSteering.arrive=true;
    
    }
     public override void Execute(FieldPlayer player)
    {
    
        if(player.EnterAreaNum!=player.BaseAreaNum)
        {
    
         if(player.BaseRegion!=null)
         {
           
         player.m_pSteering.SetArriveTarget(player.BaseRegion.areaPos,Deceleration.fast);
           player.m_pSteering.wander=false;
         player.m_pSteering.arrive=true;
         }
        }
        else
        {
            
         player.m_pSteering.arrive=false;
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
    private static PlayerGoHome instance;
     public static PlayerGoHome Instance
     {
get{
    if(instance==null)
    {
        instance=new PlayerGoHome();
    }
    return instance;
}
     }
}
