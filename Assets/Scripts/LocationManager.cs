using System.Collections.Generic;
using UnityEngine;

public class LocationManager : Singleton<LocationManager>
{
    // �����ͬ�ؿ��������ڲ�ͬ�ؿ�����ж��ʱ�ı�
    [ReadOnly] public List<GameObject> LocationList; // ����Ϊ���Կ��ӻ���
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
