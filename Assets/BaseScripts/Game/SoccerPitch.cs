using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoccerPitch :Singleton<SoccerPitch>
{
  //采样器
  public int SpotWidthNum;
  public int SpotLengthNum;
  
    //足球
  public SoccerBall m_pBall;
  //足球队
  public SoccerTeam m_pRedTeam;
  public SoccerTeam m_pBlueTeam;
  public Goal m_pRedGoal;
  public Goal m_pBlueGoal;
  //保持墙的容器
  public List<Wall> m_ListWalls;
 //定义球场的尺寸
 //场地的长度
 public float PitchLength;
 //场地的宽度
 public float PitchWidth;
//宽度分区数
 public int WidthNum;
 //长度分区数
 public int LengthNum;
 public int TotalNum;
 private float areaLength;
 private float areaWidth;
 public Region m_pPlayingArea;
 public GameObject RegionPrefab;
 public Dictionary<int,Region> m_RegionsList;
 //比赛是否开始
public bool m_bGameOn;
//守门员拿到球
public bool isGoalKeeperHasBall;
protected override void Awake()
{
base.Awake();
m_RegionsList=new Dictionary<int, Region>();
}
 void Start() {
   /*
   m_pRedGoal=  m_pRedTeam.OurGoal;
   m_pBlueGoal=  m_pBlueTeam.OurGoal;
   */
   //设置场地大小
   //先计算有多少个区域数
   TotalNum=WidthNum*LengthNum;
   //每个区域有多长多宽
  areaLength=PitchLength/(float)LengthNum;
  areaWidth=PitchWidth/(float)WidthNum;
 m_pPlayingArea=RegionPrefab.GetComponent<Region>();
 //设定预制体长宽高
 m_pPlayingArea.high=1f;
 m_pPlayingArea.length=areaLength;
 m_pPlayingArea.width=areaWidth;
 
 //生成预制体
 for(int i=0;i<TotalNum;i++)
 {
   GameObject obj= Instantiate(RegionPrefab,CalculatePos(i),Quaternion.identity);
  obj.GetComponent<Region>().Num=i;
  m_RegionsList.Add(i,obj.GetComponent<Region>());
 }
 //加完之后 要给每个region 的list指定附近的region
for(int i=0;i<TotalNum;i++)
{
 Region region=null;
 m_RegionsList.TryGetValue(i,out region);
 region.InitNearbyRegionList();
}



}
public SoccerPitch(int czClient,int cxClient)
{

}
//这个update和Render需要在其他地方调用
public void Update() {
    
}
public bool Render()
{
    return true;
}
  
//计算行列的公式
 //用自己的area序号0--TotalNum-1
  //求除以长度或者宽度序号的余数得多少行或者列
  void CalculatePos(out float Length,out float Width,int Num)
  {
    float LPosNum,WPosNum;
    if(Num<TotalNum&&Num>=0)
    {
     WPosNum=(float)Num%WidthNum+0.5f;
     LPosNum=(float)((int)(Num/WidthNum)+0.5f);
     Length=LPosNum*areaLength;
     Width=WPosNum*areaWidth;
    }
    else
    {
      Length=0;
      Width=0;
    }
  }
  Vector3 CalculatePos(int Num)
  {
    float Length,Width;
    float High=0.5f;
    CalculatePos(out Length,out Width,Num);
    return new Vector3(Width,High,Length);

  }
  public Vector3 PitchCenterPoint()
  {
    return new Vector3(PitchWidth/2,0,PitchLength/2);
  }

  //判断是否比赛已经开始
  public bool isGameOn()
  {
    return m_bGameOn;
  }
  //TODO:判断守门员是否持有球 调用两个队伍的是否有守门员拿球 都没有就false
  public bool GoalKeeperHasBall()
  {
    return false;
  }
  public bool IsInPitch(Vector3 pos)
  {
  if(pos.x<PitchWidth&&pos.x>=0&&pos.z<PitchLength&&pos.z>0)
  {
    return true;
  }
  else
  {
    return false;
  }
  }
}
