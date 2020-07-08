using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitySingleton<T>: MonoBehaviour
  where T : Component
{
  private static T instance;
  public static T Instance
  {
    get //gets called whenever we retrieve unity singleton
    {
      if (instance == null)
      {
        instance = FindObjectOfType<T>(); //searches scene and looks for type T
        if (instance == null)
        {
          GameObject obj = new GameObject();
          obj.hideFlags = HideFlags.HideAndDontSave;
          instance = obj.AddComponent<T>();
        }
      }
      return instance;
    }
  }
}
