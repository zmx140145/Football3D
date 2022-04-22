using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Singleton<Player>
{
  public Player_SO player_SO;
  public int isThreatened( PlayerBase player,float outside,float inside)
  {
   List<Region> regionsList=new List<Region>();
   Region m_region=null;
   int widthNum=SoccerPitch.Instance.WidthNum;
  
   List<PlayerBase> list=new List<PlayerBase>();
   //首先的到附近范围内的所以region
   SoccerPitch.Instance.m_RegionsList.TryGetValue(player.EnterAreaNum,out m_region);
   IEnumerator<Region> regionIE =m_region.NearbyRegionsList.GetEnumerator();
 
  while(regionIE.MoveNext())
  {
  regionsList.Add(regionIE.Current);
  }
   regionsList.Add(m_region);
   //要找到储存着player的list 从rigion里面拿 要拿上附近的8个rigion的player
   foreach(Region region in regionsList)
   {
   foreach(var obj in region.entitysList)
   {
     if( obj.GetComponent<PlayerBase>()?.TeamNum!=player.TeamNum)
     {
     list.Add(obj.GetComponent<PlayerBase>());
     }
   }
   }
   //然后用tagNeiberhour 标记外圈范围player 再判断是不是对手 是的话 outsideOppNum++
   int outsideOppNum=0;
   player.TagNeighbors<PlayerBase,PlayerBase>(player,list,outside);
   IEnumerator<PlayerBase> ListIE=list.GetEnumerator();
   while(ListIE.MoveNext())
   {
   if(ListIE.Current.m_Tag)
   {
      outsideOppNum++;
   }
   }
   //然后同理是 insideOppNum++
   int insideOppNum=0;
   player.TagNeighbors<PlayerBase,PlayerBase>(player,list,inside);
    ListIE=list.GetEnumerator();
   while(ListIE.MoveNext())
   {
   if(ListIE.Current.m_Tag)
   {
      insideOppNum++;
   }
   
   }
//注意外圈是包含内圈的
   //根据 两个num的关系 返回相应的数
    //0级是外圈都没有敌人
   //1级是外圈有敌人少于3个然后内圈没有敌人
   //2级是外圈多于等于三个敌人然后内圈没有敌人或者外圈敌人少于3个而且内圈有一个敌人
   //3级是内圈有多个敌人
   if(outsideOppNum==0)
   {
      return 0;
   }
   if(outsideOppNum<3&&insideOppNum==0)
   {
      return 1;
   }
   if((outsideOppNum>=3&&insideOppNum==0)||(outsideOppNum<3&&insideOppNum==1))
   {
      return 2;
   }
   return 3;
    


  }
}
