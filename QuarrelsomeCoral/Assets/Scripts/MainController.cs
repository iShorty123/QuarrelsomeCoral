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
        REnemy.StartEnemySpawn();

        PositionSubmarine();
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
        //foreach (var pos in Map.cellBounds.allPositionsWithin)
        //{

        //    if(

        //}
        return transform.position;
    }
}
