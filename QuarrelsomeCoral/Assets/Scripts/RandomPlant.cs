using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RandomPlant : MonoBehaviour
{

    Tilemap Map = null;
    Camera MainCamera = null;
    Vector3 Position;

    GameObject[] Plants = new GameObject[3];

    public int RandomNumber = 15;

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
    }

    public void AddPlants() {
        foreach (var pos in Map.cellBounds.allPositionsWithin) {
            SpawnPlant(pos);
            if (Random.Range(0, 3) == 1) SpawnPlant(pos);
            if (Random.Range(0, 6) == 1) SpawnPlant(pos);
        }

        this.gameObject.transform.position = Position;
    }

    void SpawnPlant(Vector3Int mapPosition) {

        if (Map.GetTile(mapPosition) == null) return;

        bool shouldRotate = false;

        Vector3 newPosition = mapPosition;

        int rand = Random.Range(0, 2);

        if (checkIfEmpty("T", mapPosition) && rand == 0) {
            //Map.SetColor(mapPosition, Color.green);
            newPosition.x -= Random.Range(0.2f, 0.8f);
            newPosition.y += 0.2f;
        }
        else if (checkIfEmpty("B", mapPosition) && rand == 1)
        {
            //Map.SetColor(mapPosition, Color.blue);
            newPosition.x -= Random.Range(0.2f, 0.8f);
            newPosition.y -= 1.2f;
            shouldRotate = true;
        }
        //else if (checkIfEmpty("L", mapPosition) && rand == 2) {
        //    Map.SetColor(mapPosition, Color.yellow);
        //}
        //else if (checkIfEmpty("R", mapPosition) && rand == 3) {
        //    Map.SetColor(mapPosition, Color.red);
        //}

        if (newPosition == mapPosition || newPosition.y < Map.cellBounds.yMin) return;

        newPosition.z = -4;
        GameObject plant = Instantiate(Plants[Random.Range(0, 3)]);
        plant.transform.position = newPosition;
        if (shouldRotate) plant.transform.localRotation *= Quaternion.Euler(0, 0, 180);
        plant.transform.parent = this.transform;

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
