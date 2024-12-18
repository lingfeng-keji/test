using System.Collections.Generic;
using UnityEngine;

public class LocationManager : Singleton<LocationManager>
{
    // 如果不同关卡，还需在不同关卡载入卸载时改变
    [ReadOnly] [SerializeField] private List<GameObject> locationList; // 单纯为调试可视化用

    private Dictionary<string, GameObject> nameToLocationDict;

    [ReadOnly] public Dictionary<string, GameObject> NameToLocationDict 
    {
        get => nameToLocationDict;
    }

    private void Start()
    {
        locationList = new List<GameObject>();
        nameToLocationDict = new Dictionary<string, GameObject>();
        foreach (Transform child in transform)
        {
            GameObject location = child.gameObject;
            NameToLocationDict.Add(location.name, location);
            locationList.Add(location);
        }
    }
}
