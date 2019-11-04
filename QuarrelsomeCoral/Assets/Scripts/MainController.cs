using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MainController : MonoBehaviour
{
	public RandomEnemy REnemy = null;
    public RandomCave RCave = null;
    public RandomPlant RPlant = null;

    public RuleTile Tile;
    public Tilemap Map = null;
    public GameObject SubmarinePrefab = null;

    public Camera MainCamera = null;
    public Camera FarCamera = null;

    // Start is called the first frame update
    void Start()
    {

        RCave.Setup(Map, MainCamera, FarCamera, Tile);
        RPlant.Setup(Map, MainCamera);
        REnemy.Setup(Map, MainCamera);

        RCave.ConstructCave();
        RPlant.AddPlants();
        PositionSubmarine();
        REnemy.StartEnemySpawn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PositionSubmarine() {
        Vector3 position = FindEmpty();

        GameObject submarine = Instantiate(SubmarinePrefab);
        submarine.transform.position = position;
    }

    Vector3 FindEmpty(){

        Vector3 position = new Vector3(0, 0, -4);
        print(Map.cellBounds.yMin);
        foreach (Vector3Int pos in Map.cellBounds.allPositionsWithin) {

            if (pos.y < Map.cellBounds.yMin + 6 || pos.y > Map.cellBounds.yMax - 6 || pos.x < Map.cellBounds.xMin + 8 || pos.x > Map.cellBounds.xMax - 8) continue;

            if (Map.GetTile(pos) == null)
            {
                if (CheckSurroundings(pos))
                {
                    position = pos;
                    position.z = -4;

                    MainCamera.transform.position = new Vector3(pos.x, pos.y, -10);

                    //break;
                }
            }
        }
           
            
        return position;
    }

    bool CheckSurroundings(Vector3Int pos){

        for (int i = pos.x - 7; i < pos.x + 7; i++) {
            for (int j = pos.y - 5; j < pos.y + 5; j++)
            {
                Vector3Int p = new Vector3Int(i, j, pos.z);
                if (Map.GetTile(p)) {
                    return false;
                }
            }
        }

        return true;
    }
}
