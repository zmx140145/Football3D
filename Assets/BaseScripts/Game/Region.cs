using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Region : MonoBehaviour
{
    //附近的region 不包含自己
   public  List<Region> NearbyRegionsList; 
  public Vector3 areaPos;
  public BoxCollider boxCollider;
  public List<GameObject> entitysList;
  public List<PlayerBase> pbsList;
  //Y轴
  public float high;
  //X轴
  public float width;
  //Z轴
  public float length;
  public int Num;
  private float updateTime=0.5f;
 void Start()
 {
     if(GetComponent<BoxCollider>())
     {
         boxCollider=GetComponent<BoxCollider>();
     }
     else
     {
          boxCollider=gameObject.AddComponent<BoxCollider>();
     }
     boxCollider.size=new Vector3(width,high,length);
     boxCollider.isTrigger=true;
     areaPos=transform.position;
     pbsList=new List<PlayerBase>();
 }
 void update()
 {
     if(updateTime>0)
      {
         updateTime-=Time.deltaTime;
      }
      else
      {
         JudgeInRegion();
      }
 }
 private void JudgeInRegion()
 {

 }
 //更新附近的Region 到list 这个要在球场生成完所有region的时候 在soccerPitch里执行
 public void InitNearbyRegionList()
 {
      int widthNum=SoccerPitch.Instance.WidthNum;
     int[] areaNums;
     //这个分三种情况  如果是两边的格子 那就要排除多余的附近格子   
     //可以根据求余的值来判断
     if((Num+1)%widthNum==1)
     {
areaNums=new int[]{Num+1,Num-widthNum,Num-widthNum+1,Num+widthNum,Num+widthNum+1};
     }
     else
     {
      if((Num+1)%widthNum==0)
      {
 areaNums=new int[]{Num-1,Num-widthNum,Num-widthNum-1,Num+widthNum,Num+widthNum-1};
      }
      else
      {
areaNums=new int[]{Num+1,Num-1,Num-widthNum,Num-widthNum+1,Num-widthNum-1,Num+widthNum,Num+widthNum-1,Num+widthNum+1};
      }
     }
     
     Region tempRegion=null;
     foreach(var num in areaNums)
     {

         if(SoccerPitch.Instance.m_RegionsList.TryGetValue(num,out tempRegion))
         {
              NearbyRegionsList.Add(tempRegion);
         }
     
     }

 }
private void OnTriggerEnter(Collider enterCollider)
{
    if(enterCollider.CompareTag("Player"))
    {
        pbsList.Add( enterCollider.GetComponent<PlayerBase>());
    }
}
  private void OnTriggerStay(Collider enterCollider) {
      if(enterCollider.CompareTag("Player"))
      {
       enterCollider.gameObject.GetComponent<PlayerBase>().EnterAreaNum=Num;
      }
  }
  private void OnTriggerExit(Collider enterCollider) {
      if(enterCollider.CompareTag("Player"))
      {
       enterCollider.gameObject.GetComponent<PlayerBase>().EnterAreaNum=-1;
       pbsList.Remove( enterCollider.GetComponent<PlayerBase>());
      }
  }
  
}
