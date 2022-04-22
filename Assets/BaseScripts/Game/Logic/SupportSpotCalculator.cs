using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportSpotCalculator:Singleton<SupportSpotCalculator>
{
//存放着场内所以采样点
  public Dictionary<int,SupportSpot> SpotDic;
  
    //先获得足球场的大小
  public float PitchLength;
  public float PitchWidth;
  public int WidthNum;
 //长度分区数
 public int LengthNum;
 public int TotalNum;
 private float areaLength;
 private float areaWidth;
 private float time=0.5f;
 private bool allowCodeFlow;
 
//  private SupportSpot m_pBestSupportingSpot;
 WorldObject_SO worldObject_SO;
 void Start()
 {
  //  m_pBestSupportingSpots=new Dictionary<PlayerBase, SupportSpot>();
   SpotDic=new Dictionary<int, SupportSpot>();
   worldObject_SO=WorldObject.Instance.worldObject_SO;
   PitchLength=SoccerPitch.Instance.PitchLength;
   PitchWidth=SoccerPitch.Instance.PitchWidth;
 this.WidthNum=SoccerPitch.Instance.SpotWidthNum;
 this.LengthNum=SoccerPitch.Instance.SpotLengthNum;
 areaLength=PitchLength/LengthNum;
 areaWidth=PitchWidth/WidthNum;
 TotalNum=WidthNum*LengthNum;
 float X,Z;
 for(int i=0;i<TotalNum;i++)
 {
 CalculatePos(out Z,out X,i);
 SupportSpot sp=new SupportSpot(new Vector3(X,0,Z),0);
 SpotDic.Add(i,sp);
 }
 }
private void Update() {
  if(time<0)
  {
    allowCodeFlow=true;
    time=0.5f;
  }
  else
  {
    allowCodeFlow=false;
  }
  time-=Time.deltaTime;
}
 void CalculatePos(out float Length,out float Width,int Num)
  {
    float LPosNum,WPosNum;
    if(Num<TotalNum&&Num>=0)
    {
     WPosNum=(float)Num%WidthNum+0.5f;
     LPosNum=(int)(Num/WidthNum)+0.5f;
     Length=LPosNum*areaLength;
     Width=WPosNum*areaWidth;
    }
    else
    {
      Length=0;
      Width=0;
    }
  }
  public Vector3 DetermineBestSupportingPostion(SoccerTeam myTeam,PlayerBase supporter)
  {
    Debug.Log("spot calculate");
    // m_pBestSupportingSpot存放在每一个playerbase自己身上
   SupportSpot m_pBestSupportingSpot;
  m_pBestSupportingSpot=supporter.m_pBestSupportSpot;
    //每几帧更新该位置
    if(!allowCodeFlow&&m_pBestSupportingSpot.m_vPos!=Vector3.zero)
    {
     return m_pBestSupportingSpot.m_vPos;
    }
    //重置接应点
    m_pBestSupportingSpot.m_vPos=Vector3.zero;
    m_pBestSupportingSpot.m_dScore=0f;
    var curSpot=SpotDic.GetEnumerator();
   while(curSpot.MoveNext())
   {
     //首先删除之前的分数（分数被设为1，从而看到所有点的位置）
   curSpot.Current.Value.m_dScore=1f;
//测试1：传球到这个位置是否安全
//感觉这个后面要改为个人的传球能力
if(myTeam.isPassSafeFromOpponents(myTeam.m_pControllingPlayer.M_Pos(),curSpot.Current.Value.m_vPos,null,Player.Instance.player_SO.MaxPassingForce))
{
curSpot.Current.Value.m_dScore+=worldObject_SO.Spot_PassSafeStrength;
}
//测试2：是否可以在这个位置射门
Vector3 shootTarget=Vector3.zero;
if(myTeam.CanShoot(curSpot.Current.Value.m_vPos,Player.Instance.player_SO.MaxPassingForce,ref shootTarget))
{
curSpot.Current.Value.m_dScore+=worldObject_SO.Spot_CanScoreStrength;
}
//测试3：计算这个点离控球队员多远，远一点分数高
//任何远于optimalDistance像素距离的无法接到球
if(myTeam .m_pControllingPlayer)
{
  float dist=Vector3.Distance(myTeam.m_pControllingPlayer.M_Pos(),curSpot.Current.Value.m_vPos);
  float temp=worldObject_SO.OptimalDistance-dist;
  if(temp>0)
  {
    //标准化距离，把他加到分数里
    curSpot.Current.Value.m_dScore+=worldObject_SO.Spot_PassDistanceStrength*((worldObject_SO.NicePassDistance-Mathf.Abs(worldObject_SO.NicePassDistance-dist))/worldObject_SO.NicePassDistance);
  }
}
//测试4：
//检测这些点离这个supporter的距离越近越好
if(supporter)
{
float dist=Vector3.Distance(supporter.M_Pos(),curSpot.Current.Value.m_vPos);
float temp=worldObject_SO.MaxSupportDistance-dist;
float temp1=dist=worldObject_SO.MinSupportDistance;
  if(temp>0&&temp1>0)
  {
    //标准化距离，把他加到分数里
    curSpot.Current.Value.m_dScore+=worldObject_SO.Spot_SupportDistanceStrength*((worldObject_SO.NiceSupportDistance-Mathf.Abs(worldObject_SO.NiceSupportDistance-dist))/worldObject_SO.NiceSupportDistance);
  }
}
//测试5：
//离进攻距离的位置
if(supporter)
{
  float dist=Vector3.Distance(supporter.M_Pos(),curSpot.Current.Value.m_vPos);
float temp=worldObject_SO.MaxAreaDistance-dist;

  if(temp>0)
  {
    //标准化距离，把他加到分数里
    curSpot.Current.Value.m_dScore+=worldObject_SO.Spot_NearAttackAreaStrength*((worldObject_SO.NiceAreaDistance-Mathf.Abs(worldObject_SO.NiceAreaDistance-dist))/worldObject_SO.NiceAreaDistance);
  }
}
//检查到目前为止这个点是不是最优点
if(curSpot.Current.Value.m_dScore>m_pBestSupportingSpot.m_dScore)
{
  m_pBestSupportingSpot.m_dScore=curSpot.Current.Value.m_dScore;
  m_pBestSupportingSpot.m_vPos=curSpot.Current.Value.m_vPos;
}
   }
   //把算出来的结果重新放回supporter身上
  supporter.m_pBestSupportSpot=m_pBestSupportingSpot;
   return m_pBestSupportingSpot.m_vPos;
  }


  
   
}
public class SupportSpot
{
   public Vector3 m_vPos;
   public float m_dScore;
  public  SupportSpot(Vector3 pos,float val)
   {
  this.m_vPos=pos;
  this.m_dScore=val;
   }
}
