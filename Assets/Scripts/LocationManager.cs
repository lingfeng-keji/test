using System.Collections.Generic;
using UnityEngine;

public class LocationManager : Singleton<LocationManager>
{
    // 如果不同关卡，还需在不同关卡载入卸载时改变
    [ReadOnly] public List<GameObject> LocationList; // 单纯为调试可视化用
    public Dictionary<string, GameObject> NameToLocationDict;

    private void Start()
    {
        LocationList = new List<GameObject>();
        NameToLocationDict = new Dictionary<string, GameObject>();
        foreach (Transform child in transform)
        {
            GameObject location = child.gameObject;
            NameToLocationDict.Add(location.name, location);
            LocationList.Add(location);
        }
    }
}
