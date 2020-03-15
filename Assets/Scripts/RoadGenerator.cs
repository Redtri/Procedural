using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

[ExecuteInEditMode]
public class RoadGenerator : MonoBehaviour
{
    public Transform center;
    public int nbStraightRoads;
    public int nbPeriphRoads;
    public Vector2 distRange;
    public Vector2 nbCornersRange;
    public Vector2 dirNoise;
    public Vector2 cornerDirNoise;

    public GameObject cornerPrefab;

    public List<Road> roads = new List<Road>();
    public List<Road> periphRoads = new List<Road>();

    [System.Serializable]
    public class Road
    {
        public int nbCorners;
        public Transform container;
        public Vector3 origin;
        public LineRenderer lineRenderer;
        public LinkedList<GameObject> corners = new LinkedList<GameObject>();
        public Road(string name, Transform trsf, LineRenderer lineRender, Vector3 startPoint, Transform topParent = null)
        {
            container = trsf;
            container.gameObject.name = name;
            origin = startPoint;
            lineRenderer = lineRender;
            lineRenderer.positionCount = 1;
            container.parent = topParent;
            corners = new LinkedList<GameObject>();
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GenerateRoads();
        }

        if (Input.GetKeyDown((KeyCode.C)))
        {
            ClearRoads(roads);
            ClearRoads(periphRoads);
        }
    }

    public void GenerateRoads()
    {
        //Creating a road list for the peripheral roads
        periphRoads = new List<Road>(nbPeriphRoads);
        for(int i = 0; i < nbPeriphRoads; ++i) {
            GameObject newRoadGO = new GameObject();
            Road currentRoad = new Road("PeriphRoad_" + i, newRoadGO.transform, newRoadGO.AddComponent<LineRenderer>(), Vector3.zero, this.transform);
            periphRoads.Add(currentRoad);
        }

        for (int i = 0; i < nbStraightRoads; ++i) {
            //Calculating angle in which the road points to and the direction associated
            float currentAngle = (360f / nbStraightRoads) * i;
            Vector3 direction = new Vector3(Mathf.Cos(Mathf.Deg2Rad * (currentAngle + Random.Range(dirNoise.x, dirNoise.y))),
                                                        0f, 
                                                        Mathf.Sin(Mathf.Deg2Rad * (currentAngle + Random.Range(dirNoise.x, dirNoise.y)))
                                                        );

            //Creating new road
            Road currentRoad = CreateRoad(center.position, direction);

            //Generating corners inside the road
            GenerateCorners(i, currentRoad, direction);

            roads.Add(currentRoad);
        }
    }

    public void ClearRoads(List<Road> tRoads)
    {
        foreach(Road road in tRoads)
        {
            ClearRoad(road);
            Destroy(road.container.gameObject);
        }

        tRoads.Clear();
    }

    private void ClearRoad(Road road)
    {
        foreach (GameObject corner in road.corners)
        {
            if(corner)
                Destroy(corner);
        }
        road.corners.Clear();
        road.lineRenderer.positionCount = 1;
    }

    private Road CreateRoad(Vector3 origin, Vector3 direction)
    {
        //Spawning a Road GameObject that will contain nbCorners corners
        GameObject newRoadGo = new GameObject();
        Road newRoad = new Road("StraightRoad_" + roads.Count, newRoadGo.transform, newRoadGo.AddComponent<LineRenderer>(), origin, this.transform);

        //Setting a random amount of corners
        newRoad.nbCorners = Random.Range((int)nbCornersRange.x, (int)nbCornersRange.y);
        
        return newRoad;
    }

    private void GenerateCorners(int roadIndex, Road currentRoad, Vector3 direction)
    {
        //At this point, the previous corner is the origin of the map generator
        Vector3 previousLocation = currentRoad.origin;

        //Iterating to create as many corners as defined by nbCorners
        for (int cornerIndex = 0; cornerIndex < currentRoad.nbCorners; ++cornerIndex)
        {

            CreateCorner(currentRoad, previousLocation, direction, cornerIndex, roadIndex);
            previousLocation = currentRoad.corners.Last.Value.transform.position;
        }
    }

    private void CreateCorner(Road currentRoad, Vector3 previousLocation, Vector3 direction, int cornerIndex, int roadIndex)
    {
        //STRAIGHT ROAD
        float randX = Random.Range(cornerDirNoise.x, cornerDirNoise.y);
        float randY = Random.Range(cornerDirNoise.x, cornerDirNoise.y);
        GameObject newCornerGo = Instantiate(cornerPrefab, previousLocation /*+ new Vector3(randX, 0f, randY)*/ + direction * Random.Range((int)distRange.x, (int)distRange.y),
                                            Quaternion.identity, currentRoad.container.transform);
        LineRenderer mainRoadlineRenderer = currentRoad.lineRenderer;

        currentRoad.corners.AddLast(newCornerGo);
        
        //LINE RENDERER
        mainRoadlineRenderer.positionCount += 1;
        mainRoadlineRenderer.SetPosition(cornerIndex+1, newCornerGo.transform.position);

        //PERIPHERAL ROAD
        //The current corner represents an index for each peripheral road
        if (cornerIndex < nbPeriphRoads)
        {
            LineRenderer secRoadLineRenderer = periphRoads[cornerIndex].lineRenderer;
            if (roadIndex > 0)
            {
                secRoadLineRenderer.positionCount += 1;
                secRoadLineRenderer.SetPosition(roadIndex-1, periphRoads[cornerIndex].corners.Last.Value.transform.position);
                if (roadIndex == nbStraightRoads - 1) {
                    secRoadLineRenderer.SetPosition(roadIndex, newCornerGo.transform.position);
                    secRoadLineRenderer.positionCount += 1;
                    secRoadLineRenderer.SetPosition(secRoadLineRenderer.positionCount-1, periphRoads[cornerIndex].corners.First.Value.transform.position);
                }
            }
            periphRoads[cornerIndex].corners.AddLast(newCornerGo);
        }
    }

    private void BindCorners(LineRenderer render, GameObject cornerA, GameObject cornerB)
    {
        
    }
    
    private void BindCorner()
    {
        
    }

    private void OnDrawGizmos()
    {
        for(int i = 0; i < nbStraightRoads; ++i) {
            float currentAngle = (360f / nbStraightRoads) * i;
            Vector2 direction = new Vector2(Mathf.Cos(Mathf.Deg2Rad * currentAngle), Mathf.Sin(Mathf.Deg2Rad * currentAngle));

            Debug.DrawLine(center.position, center.position + new Vector3(direction.x, 0f, direction.y) * 100f);
        }
    }
}
