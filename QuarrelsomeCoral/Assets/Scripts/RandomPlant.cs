using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RandomPlant : MonoBehaviour
{

    Tilemap Map = null;
    Camera MainCamera = null;
    Vector3 Position;

    GameObject[] Plants = new GameObject[8];

    public int RandomNumber = 15;

    GameObject[,] PlantObjects;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup(Tilemap map, Camera mainCamera, Vector3 position, GameObject[] plants) {
        Map = map;
        MainCamera = mainCamera;
        Position = position;
        Plants = plants;

        PlantObjects = new GameObject[map.size.x,map.size.y]; //make sure these are the right values
    }

    public void AddPlants() {
        foreach (var pos in Map.cellBounds.allPositionsWithin) {
            PlantObjects[pos.x + Map.size.x / 2, pos.y + Map.size.y / 2] = SpawnPlant(pos);
        }

        this.gameObject.transform.position = Position;
    }

    public GameObject[,] GetPlants() {
        return PlantObjects;
    }

    GameObject SpawnPlant(Vector3Int mapPosition) {

        if (Map.GetTile(mapPosition) == null) return null;

        bool shouldRotate = false;

        Vector3 newPosition = mapPosition;

        int rand = Random.Range(0, 2);

        if (checkIfEmpty("T", mapPosition) && rand == 0) {
            newPosition.x -= Random.Range(0.2f, 0.8f);
            newPosition.y += 0.3f;
        }
        else if (checkIfEmpty("B", mapPosition) && rand == 1)
        {
            newPosition.x -= Random.Range(0.2f, 0.8f);
            newPosition.y -= 1.3f;
            shouldRotate = true;
        }

        if (newPosition == mapPosition || newPosition.y < Map.cellBounds.yMin) return null;

        newPosition.z = -4;
        GameObject plant = Instantiate(Plants[Random.Range(0, Plants.Length)]);
        plant.transform.position = newPosition;
        if (shouldRotate) plant.transform.localRotation *= Quaternion.Euler(0, 0, 180);
        plant.transform.parent = this.transform;

        return plant;
    }


    bool checkIfEmpty(string place, Vector3Int position) {

        Vector3Int pos = position;

        switch (place){
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
