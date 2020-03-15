using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RoadGenerator : MonoBehaviour
{
    public Transform center;
    public int nbStraightRoads;
    public int nbPeriphRoads;
    public Vector2 distRange;
    public Vector2 nbCornersRange;

    public GameObject cornerPrefab;

    public List<Road> roads = new List<Road>();
    public List<Road> periphRoads = new List<Road>();

    [System.Serializable]
    public class Road
    {
        public int nbCorners;
        public Transform container;
        public LineRenderer lineRenderer;
        public List<GameObject> corners = new List<GameObject>();
        public Road(string name, Transform trsf, LineRenderer lineRender, Transform topParent = null)
        {
            container = trsf;
            container.gameObject.name = name;
            lineRenderer = lineRender;
            container.parent = topParent;
            corners = new List<GameObject>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            GenerateRoads();
        }
    }

    public void GenerateRoads()
    {
        //Creating a road list for the peripheral roads
        periphRoads = new List<Road>(nbPeriphRoads);
        for(int i = 0; i < nbPeriphRoads; ++i) {
            GameObject newRoadGO = new GameObject();
            Road currentRoad = new Road("PeriphRoad_" + i, newRoadGO.transform, newRoadGO.AddComponent<LineRenderer>(), this.transform);
            periphRoads.Add(currentRoad);
        }

        for (int i = 0; i < nbStraightRoads; ++i) {
            //Calculating angle in which the road points to
            float currentAngle = (360f / nbStraightRoads) * i;
            Vector2 direction = new Vector2(Mathf.Cos(Mathf.Deg2Rad * currentAngle), Mathf.Sin(Mathf.Deg2Rad * currentAngle));

            //Spawning a Road GameObject that will contain nbCorners corners
            GameObject newRoadGO = new GameObject();
            Road currentRoad = new Road("StraightRoad_" + i, newRoadGO.transform, newRoadGO.AddComponent<LineRenderer>(),this.transform);

            //Setting a random amount of corners
            currentRoad.nbCorners = Random.Range((int)nbCornersRange.x, (int)nbCornersRange.y);

            GenerateCorners(i, currentRoad, new Vector3(direction.x, 0f, direction.y));


            roads.Add(currentRoad);
        }
    }

    private void GenerateCorners(int roadIndex, Road currentRoad, Vector3 direction)
    {
        Debug.Log(direction);
        //At this point, the previous corner is the origin of the map generator
        Vector3 previousCorner = center.position;
        //Also, creating an array for the line renderer points as large as the number of corners
        Vector3[] lineRenderAnchors = new Vector3[currentRoad.nbCorners + 1];
        currentRoad.lineRenderer.positionCount = currentRoad.nbCorners + 1;

        //Iterating to create as many corners as defined by nbCorners
        for (int currentCorner = 0; currentCorner < currentRoad.nbCorners; ++currentCorner) {
            lineRenderAnchors[currentCorner] = previousCorner;
            Vector2 tmpDir = direction;// new Vector2(Mathf.Cos(Random.Range(0, 5f)), Mathf.Sin(Random.Range(0, 5f)));
            GameObject newCornerGO = Instantiate(cornerPrefab, currentRoad.container.transform);

            //Offseting the corner from the last one within range and adding it to the list
            newCornerGO.transform.position = previousCorner + new Vector3(tmpDir.x, 0f, tmpDir.y) * Random.Range((int)distRange.x, (int)distRange.y);
            currentRoad.corners.Add(newCornerGO);

            previousCorner = currentRoad.corners[currentCorner].transform.position;

            //The current corner represents an index for each peripheral road
            /*if (currentCorner < nbPeriphRoads && roadIndex > 0) {
                periphRoads[currentCorner].lineRenderer.positionCount += 1;
                periphRoads[currentCorner].lineRenderer.SetPosition(periphRoads[currentCorner].lineRenderer.positionCount - 1, roads[roadIndex - 1].corners[currentCorner].transform.position);
                /*if (roadIndex == nbStraightRoads - 1) {
                    //Linking the corners of the last straight road to the first road ones
                    periphRoads[currentCorner].lineRenderer.SetPosition(1, roads[roadIndex].corners[currentCorner].transform.position);
                }
            }*/
        }
        lineRenderAnchors[lineRenderAnchors.Length - 1] = previousCorner;

        currentRoad.lineRenderer.SetPositions(lineRenderAnchors);
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
