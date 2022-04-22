using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameList : Singleton<GameList>
{
public List<BaseGameEntity> ObstaclesList;
public List<Vehicle> VehiclesList;
   protected override void  Awake()
   {
   base.Awake();
   ObstaclesList=new List<BaseGameEntity>();
  VehiclesList=new List<Vehicle>();
   }
}
