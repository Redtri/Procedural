using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Region", menuName = "ScriptableObjects/RegionManager", order = 1)]

public class Region : ScriptableObject
{
    public int minBuildSpace;
    public int maxBuildSpace;
    public float minRadius;
    public float maxRadius;
}
