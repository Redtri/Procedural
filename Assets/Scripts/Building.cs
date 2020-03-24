using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;
using Random = UnityEngine.Random;

public class Building : MonoBehaviour
{
    public Region[] regions;

    public GameObject cubeParent;
    public GameObject cubePrefab;
    private List<GameObject> cubes;
    public int maxBuildingPerDistrict;

    bool isColliding(GameObject cubeObject)
    {
        Collider[] hitColliders = Physics.OverlapBox(cubeObject.transform.position, cubeObject.transform.localScale / 2, Quaternion.identity);

        //Debug.Log(hitColliders.Length);
        return hitColliders.Length > 1;
    }

    public GameObject createCube(Vector3 pos, Transform parent, Color color, Vector3 sizeModifier)
    {
        GameObject cube = Instantiate(cubePrefab, pos, Quaternion.identity, parent);
        cube.transform.localScale += sizeModifier;
        cube.transform.position += sizeModifier/2;
        cube.GetComponent<MeshRenderer>().material.SetColor("_Color", color + new Color(Random.Range(0, 0.1f), Random.Range(0, 0.1f), Random.Range(0, 0.1f)));
        
        if (isColliding(cube)) {
            Destroy(cube);
            return null;
        } else  {
            cubes.Add(cube);
            return cube;
        }
    }


    public List<GameObject> genAlea(Transform buildingParent, Vector3 a, Vector3 b, Vector3 c)
    {
        float spawnProb = 0;
        List<GameObject> districtBuildings = new List<GameObject>();
        
        //Calculating center coordinates of the current district
        float xDistrictCenter = (a.x + b.x + c.x) / 3f;
        float zDistrictCenter = (a.z + b.z + c.z) / 3f;
        Debug.DrawLine(new Vector3(xDistrictCenter, 0f, zDistrictCenter), Vector3.up*50f);
        
        Region region = getRegionFromDist(Vector3.Distance(cubeParent.transform.position, new Vector3(xDistrictCenter, 0f, zDistrictCenter)));
        
        Debug.Log("Generating in " + region);
        for (int i = 0; i < maxBuildingPerDistrict; ++i)
        {
            spawnProb = Random.Range(0f, 1f);
            if(spawnProb >= region.spawnPercent)
            {
                float r1 = Random.Range(0, (float)region.minBuildSpace), r2 = Random.Range(0, (float)region.minBuildSpace), r3 = Random.Range(0, (float)region.minBuildSpace);

                float x = (region.minBuildSpace - Mathf.Sqrt(r1)) * a.x + (Mathf.Sqrt(r1) * (region.minBuildSpace - r2)) * b.x + (Mathf.Sqrt(r1) * r2) * c.x;
                float z = (region.minBuildSpace - Mathf.Sqrt(r1)) * a.z + (Mathf.Sqrt(r1) * (region.minBuildSpace - r2)) * b.z + (Mathf.Sqrt(r1) * r2) * c.z;
                
                GameObject tmp = createCube(new Vector3(x, 0f, z), buildingParent, region.buildingColor, region.GetSizeModifier());
                if(tmp)
                    districtBuildings.Add(tmp);
            }
        }

        return districtBuildings;
    }

    Region getRegionFromDist(float dist)
    {
        for (int i = 0; i < regions.Length; ++i)
        {
            if (dist >= regions[i].minRadius && dist <= regions[i].maxRadius)
                return regions[i];
        }
        //Returning the farthest region by default
        return regions[regions.Length-1];
    }

    void Start()
    {
        cubes = new List<GameObject>();
    }

}
