using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RandomPointOfInterest : MonoBehaviour
{

    Tilemap Map = null;
    Camera MainCamera = null;
    Vector3 Position;

    GameObject[] PointsOfInterest = new GameObject[4];

    public int RandomNumber = 15;

    GameObject[,] PointOfInterestObjects;

    private int m_Count;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Setup(Tilemap map, Camera mainCamera, Vector3 position, GameObject[] pointsOfInterest)
    {
        Map = map;
        MainCamera = mainCamera;
        Position = position;
        PointsOfInterest = pointsOfInterest;

        PointOfInterestObjects = new GameObject[map.size.x, map.size.y]; //make sure these are the right values
    }

    public void AddPointsOfInterest()
    {
        foreach (var pos in Map.cellBounds.allPositionsWithin)
        {
            PointOfInterestObjects[pos.x + Map.size.x / 2, pos.y + Map.size.y / 2] = SpawnPointOfInterest(pos);
        }

        this.gameObject.transform.position = Position;
    }

    public GameObject[,] GetPointsOfInterest()
    {
        return PointOfInterestObjects;
    }

    GameObject SpawnPointOfInterest(Vector3Int mapPosition)
    {

        if (Map.GetTile(mapPosition) == null) return null;

        bool shouldRotate = false;

        Vector3 newPosition = mapPosition;

        int rand = Random.Range(0, 2);

        if (checkIfEmpty("T", mapPosition) && rand == 0)
        {
            newPosition.x -= Random.Range(0.2f, 0.8f);
            newPosition.y += 0.2f;
        }
        else if (checkIfEmpty("B", mapPosition) && rand == 1)
        {
            newPosition.x -= Random.Range(0.2f, 0.8f);
            newPosition.y -= 1.2f;
            shouldRotate = true;
        }

        if (newPosition == mapPosition || newPosition.y < Map.cellBounds.yMin) return null;
        newPosition.z = 0;

        m_Count++;
        if (m_Count % 75 == 0) //Spawns about 4 - 5
        {
            GameObject pointOfInterest = Instantiate(PointsOfInterest[Random.Range(0, PointsOfInterest.Length)]); 
            pointOfInterest.transform.position = newPosition;
            if (shouldRotate) pointOfInterest.transform.localRotation *= Quaternion.Euler(0, 0, 180);
            pointOfInterest.transform.parent = this.transform;
            return pointOfInterest;
        }
        else
        {
            return null;
        }
    }


    bool checkIfEmpty(string place, Vector3Int position)
    {

        Vector3Int pos = position;

        switch (place)
        {
            case "L":
                pos.x = pos.x - 1;
                if (Map.GetTile(pos) == null) return true;
                break;
            case "R":
                pos.x = pos.x + 1;
                if (Map.GetTile(pos) == null) return true;
                break;
            case "T":
                pos.y = pos.y + 1;
                if (Map.GetTile(pos) == null) return true;
                break;
            case "B":
                pos.y = pos.y - 1;
                if (Map.GetTile(pos) == null) return true;
                break;
        }

        return false;
    }
}
