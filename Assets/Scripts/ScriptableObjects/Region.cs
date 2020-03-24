using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Region", menuName = "ScriptableObjects/RegionManager", order = 1)]

public class Region : ScriptableObject
{
    public Vector2 sizeRange;
    public float minBuildSpace;
    public float minRadius;
    public float maxRadius;
    public float spawnPercent;
    public Color buildingColor;

    public Vector3 GetSizeModifier()
    {
        return new Vector3(0f, Random.Range(sizeRange.x, sizeRange.y), 0f);
    }
}
