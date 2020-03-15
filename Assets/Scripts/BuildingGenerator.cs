using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BuildingGenerator : MonoBehaviour
{
    public GameObject buildingPrefab;
    public Transform topParent;
    public RoadGenerator roadGenerator;
    public Vector2Int nbBuildingRange;
    public float minRange;
    public List<District> districts;
    private Color currentColor = Color.red;
    
    [System.Serializable]
    public class District
    {
        public int nbBuildings;
        public List<GameObject> buildings;

        public District()
        {
            buildings = new List<GameObject>();
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
            GenerateDistricts();
    }

    public void GenerateDistricts()
    {
        for (int i = 0; i < roadGenerator.nbPeriphRoads * roadGenerator.nbStraightRoads; ++i)
        {
            GenerateDistrict(i);
        }
    }

    private void GenerateDistrict(int index)
    {
        Vector3 origin = roadGenerator.center.position;
        District district = new District();
        district.nbBuildings = Random.Range(nbBuildingRange.x, nbBuildingRange.y);

        if (index < roadGenerator.nbStraightRoads) {
            if (index == roadGenerator.nbStraightRoads - 1) {
                GenerateBuildings(district.nbBuildings,origin, roadGenerator.roads[index].corners.First.Value.transform.position, roadGenerator.roads[0].corners.First.Value.transform.position);
            } else {
                GenerateBuildings(district.nbBuildings,origin, roadGenerator.roads[index].corners.First.Value.transform.position, roadGenerator.roads[index+1].corners.First.Value.transform.position);
            }
        } else  {
            int roadIndex = index - (roadGenerator.nbStraightRoads * (index/roadGenerator.nbStraightRoads));
            int cornerIndex =  ((index) / roadGenerator.nbStraightRoads)-1;
            currentColor = Color.yellow;
            if (roadIndex == roadGenerator.nbStraightRoads - 1) {
                GenerateBuildings(district, RecursiveNext(cornerIndex, roadGenerator.roads[roadIndex].corners).transform.position,
                                            RecursiveNext(cornerIndex+1, roadGenerator.roads[roadIndex].corners).transform.position,
                                            RecursiveNext(cornerIndex, roadGenerator.roads[0].corners).transform.position,
                                            RecursiveNext(cornerIndex+1, roadGenerator.roads[0].corners).transform.position);
            } else {
                GenerateBuildings(district, RecursiveNext(cornerIndex, roadGenerator.roads[roadIndex].corners).transform.position,
                                            RecursiveNext(cornerIndex+1, roadGenerator.roads[roadIndex].corners).transform.position,
                                            RecursiveNext(cornerIndex, roadGenerator.roads[roadIndex+1].corners).transform.position,
                                            RecursiveNext(cornerIndex+1, roadGenerator.roads[roadIndex+1].corners).transform.position);
                
            }
        }
        districts.Add(district);
    }

    private GameObject RecursiveNext(int amount, LinkedList<GameObject> list)
    {
        LinkedListNode<GameObject> goIterator = list.First;
        for (int i = 0; i < amount; ++i)
        {
            goIterator = goIterator.Next;
        }
        return goIterator.Value;
    }
    
    public void GenerateBuildings(int nbBuildings, Vector3 a, Vector3 b, Vector3 c)
    {
        //Instantiating empty gameobject container for the buildings
        GameObject newDistrictGo = new GameObject();
        newDistrictGo.transform.parent = topParent;
        
        newDistrictGo.name = "District_" + districts.Count;
        for (int i = 0; i < nbBuildings; ++i)
        {
            float r1 = Random.Range(0, minRange), r2 = Random.Range(0, minRange), r3 = Random.Range(0, minRange);

            float x = (minRange - Mathf.Sqrt(r1)) * a.x + (Mathf.Sqrt(r1) * (minRange - r2)) * b.x + (Mathf.Sqrt(r1) * r2) * c.x;
            float z = (minRange - Mathf.Sqrt(r1)) * a.z + (Mathf.Sqrt(r1) * (minRange - r2)) * b.z + (Mathf.Sqrt(r1) * r2) * c.z;

            GameObject newBuildingGo = Instantiate( buildingPrefab, new Vector3(x, 0f, z), Quaternion.identity, newDistrictGo.transform);
            newBuildingGo.GetComponent<MeshRenderer>().material.SetColor("_Color", currentColor);
        }
    }
    
    public void GenerateBuildings(District district, Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        GenerateBuildings(district.nbBuildings / 2, a, b, c);
        GenerateBuildings(district.nbBuildings / 2, a, b, d);
    }
}
