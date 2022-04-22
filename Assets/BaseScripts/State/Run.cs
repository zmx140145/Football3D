using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Run : State<actor>
{
     private string Name="Run";

     public override string GetName()
    {
        return Name;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override bool OnMessage(actor entity_Type, Telegram telegram)
    {
        if(telegram.Msg==MessageType.run)
        {
   
        return true;
        }
        return false;
    }
    public override void Enter(actor actor)
    {

    }
     public override void Execute(actor actor)
    {
     
        
    }
     public override void Exit(actor actor)
    {
        
    }
    private static Run instance;
     public static Run Instance
     {
get{
    if(instance==null)
    {
        instance=new Run();
    }
    return instance;
}
     }
}
