using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell<entity>: MonoBehaviour
{
    //存在与这个单元的所有实体
    List<entity> Members;
    //单元的包围盒子
    
  public Vector3 lefttopfront;
  public Vector3 rifhtbottomback;
  virtual public void Start()
  {
  
  }

    
}