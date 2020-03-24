using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public Region region1;
    public Region region2;
    public Region region3;

    public GameObject cubeParent;
    public GameObject cubePrefab;
    private List<GameObject> cubes;
    

    bool isColliding(GameObject cubeObject)
    {
        Collider[] hitColliders = Physics.OverlapBox(cubeObject.transform.position, transform.localScale / 2, Quaternion.identity);

        //Debug.Log(hitColliders.Length);
        if(hitColliders.Length > 1)
        {
            return true;
        }

        return false;
    }

    void createCube(Vector3 Position)
    {

        var cube = Instantiate(cubePrefab, Position, Quaternion.identity, cubeParent.transform);

        if (isColliding(cube))
        {
            Debug.Log("destroyed");
            Destroy(cube);
            return;
        }

        cubes.Add(cube); 
    }


    void genAlea(Region region)
    {
        float rand1 = 0;
        float rand2 = 0;
        float rand3 = 0;

        float theta = 0;
        float radius = 0;


        for (int i = 0; i < 100; i++)
        {
            rand3 = Random.Range(0f, 1f);
            if(rand3 >= region.spawnPercent)
            {
                rand1 = Random.Range(0f, 1f);
                rand2 = Random.Range(region.minRadius, region.maxRadius);

                theta = rand1 * 2.0f * Mathf.PI;
                radius = rand2;

                Vector3 position = new Vector3(radius * Mathf.Cos(theta), 0, radius * Mathf.Sin(theta));

                createCube(position);
            }

        }
    }

        /*void genAlea()
    {
        float rand1 = 0;
        float rand2 = 0;

        float theta = 0;
        float radius = 0;

        
        rand1 = Random.Range(0f, 1f);
        rand2 = Random.Range(region.minRadius, region.maxRadius);

        theta = rand1 * 2.0f * Mathf.PI;
        radius = rand2;

        Vector3 basePosition = new Vector3(radius * Mathf.Cos(theta), 0, radius * Mathf.Sin(theta));

        createCube(basePosition);

        for (int i = 0; i < 50; i++)
        {

            rand1 = Random.Range(0f, 1f);
            rand2 = Random.Range(basePosition.x + region.minBuildSpace, basePosition.z + region.maxBuildSpace);

            theta = rand1 * 2.0f * Mathf.PI;
            radius = rand2;

            Vector3 position = new Vector3(radius * Mathf.Cos(theta), 0, radius * Mathf.Sin(theta));

            createCube(position);

            basePosition = position;
        }
    }*/

    void Start()
    {
        cubes = new List<GameObject>();
        Debug.Log("PremièreGen");
        genAlea(region1);

        Debug.Log("DeuxièmeGen");
        genAlea(region2);

        Debug.Log("TroisièmeGen");
        genAlea(region3);
    }

}
