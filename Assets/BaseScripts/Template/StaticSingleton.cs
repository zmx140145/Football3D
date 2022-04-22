using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    //单例模式
public class StaticSingleton<T>  where T:StaticSingleton<T>
{
    private static T instance;
    public static T Instance
    {
        get{return instance;}
    }

      public static bool IsInitialized
    {
        get{return instance!=null;}
    }
   
}
