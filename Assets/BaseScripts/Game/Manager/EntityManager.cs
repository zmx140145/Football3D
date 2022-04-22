using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager :Singleton<EntityManager>
{
    
   Dictionary<int,BaseGameEntity> EntityDic=new Dictionary<int, BaseGameEntity>();
   public void RegisterEntity(BaseGameEntity NewEntity)
   {
   EntityDic.Add(NewEntity.ID,NewEntity);
   }
   //通过id返回物体
   public BaseGameEntity GetEntityFromID(int id)
   {
       BaseGameEntity getEntity=null;
      if( EntityDic.TryGetValue(id,out getEntity))
      {
          return getEntity;
      }
       else
       {
           return null;
       }
   }
   //通过这方法来移除实体
   public bool RemoveEntity(BaseGameEntity pEntity)
   {
       if(EntityDic.ContainsKey(pEntity.ID))
       {
       EntityDic.Remove(pEntity.ID);
       return true;
       }
       else
       {
       return false;
       }
   }
}
