using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGameEntity : MonoBehaviour
{
    //Tag标记
    private bool m_tag;
    //每一个实体的唯一id
    private int m_ID;
    //这是下一个有效的ID，每创建一个BaseGameEntity 
    private static int m_iNextVaildID=0;
     //碰撞边缘的距离
    protected float B_radius;
   //构造函数    
    public BaseGameEntity()
    {
        SetID();
    }
    ~BaseGameEntity(){}
    public  abstract  bool HandleMessage(Telegram msg);
  
    public virtual void Start(){
        EntityManager.Instance.RegisterEntity(this);
        AddStaticList();
    }
    public void Tag()
    {
        m_tag=true;
    }
    public void UnTag()
    {
        m_tag=false;
    }
    public bool m_Tag
    {
        get {return m_tag;}
    }
   public int ID{
        get{return m_ID;}
    }
     void SetID()
    {
    m_ID=m_iNextVaildID;
    m_iNextVaildID++;
    }
    public float Radius
    {
        get{return B_radius;}
    }
 public void TagNeighbors<conT,T>(conT entity,List<T> ContainerOfEntities ,float radius) where T:BaseGameEntity where conT:BaseGameEntity
 {
     //迭代所有的实体，检查范围
     IEnumerator<T> curEntity=ContainerOfEntities.GetEnumerator();
    while(curEntity.MoveNext())
     {
         
     curEntity.Current.UnTag();
     Vector3 to=curEntity.Current.transform.position-entity.transform.position;
     //考虑包围半径
     float range=radius+curEntity.Current.Radius;
     //如果实体在范围内，标记它，作进一步考虑
     if((curEntity.Current!=entity)&&(to.sqrMagnitude<range*range))
     {
         curEntity.Current.Tag();
     }
     } //下一个实体
   
 }
 public void TagWithInViewRange<conT,T>(conT self,List<T> ContainerOfEntities ,float length,float angle) where T:BaseGameEntity where conT:Vehicle
 {
     angle=Mathf.Cos(Mathf.PI*angle/180f);
     if(ContainerOfEntities.Count>0)
     {
     //迭代所有的实体，检查范围
     IEnumerator<T> curEntity=ContainerOfEntities.GetEnumerator();
       while(curEntity.MoveNext())
     {
     curEntity.Current.UnTag();
     Vector3 to=curEntity.Current.transform.position-self.transform.position;
     //考虑包围半径
     float range=length+curEntity.Current.Radius;
    
     //如果实体在范围内，标记它，作进一步考虑
     if((curEntity.Current!=self)&&(to.sqrMagnitude<range*range)&&Vector3.Dot(to,self.M_vHeading())>angle)
     {
         curEntity.Current.Tag();
     }
     } //下一个实体
  
     }
 }


 //根据标签加入静态全局list
 void AddStaticList()
 {
     if(this.CompareTag("Obstacle"))
     {
         GameList.Instance.ObstaclesList.Add(this);
     }
      if(this.CompareTag("Vehicle"))
     {
         GameList.Instance.VehiclesList.Add(this.GetComponent<Vehicle>());
     }
 }
}
