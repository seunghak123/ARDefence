using UnityEngine;
using System;

[Serializable]
public class CommonEvent 
{
    public E_ANIMATION_EVENT animationType;
    public string eventFunctionNameString;
    public string prefabName;
    public string stateName;
    public String arguString;
    public int arguInt;
    public float arguFloat;
    public float beginTime;

    [NonSerialized]
    public GameObject objTarget;
}
