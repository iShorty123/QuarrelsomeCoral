using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MainController : MonoBehaviour
{
	public RandomEnemy REnemy = null;
    public RandomPlant RPlant = null;
    public Caves Caves = null;
    public Plants Plants = null;
    private Transform Submarine = null;

    public RuleTile Tile;
    public GameObject MapGrid = null;
    public GameObject SubmarinePrefab = null;

    public Camera MainCamera = null;
    public Camera FarCamera = null;

    bool submarinePositioned = false;

    // Start is called the first frame update
    void Start()
    {
        //put submarine in initial position
        PositionSubmarine();

        //create caves
        Caves.Setup(MainCamera, FarCamera, Tile, MapGrid);

        //start random enemy spawn
        REnemy.Setup(MapGrid, MainCamera);
        REnemy.StartEnemySpawn();
    }

    // Update is called once per frame
    void Update()
    {
        //if submarine is close to the left or right border of Caves, add a cave at that side.
        if ((Submarine.transform.position.x - Caves.GetLeftBorder()) < 140) Caves.AddCaveToLeft(); //here
        if ((Caves.GetRightBorder() - Submarine.transform.position.x) < 140) Caves.AddCaveToRight(); //here

        SetupLastCave();
    }

    void SetupLastCave() {

        RandomCave lastCave = Caves.GetLastCave();
        if (lastCave.IsDone())
        {

            Tilemap map = lastCave.GetMap();
            //print(map.transform.position);
            //add random plants
            GameObject Plant = new GameObject();
            Plant.SetActive(false);
            RandomPlant plantScript = Plant.AddComponent<RandomPlant>();
            plantScript.Setup(map, MainCamera, map.transform.position + new Vector3(1, 1, 0), Plants.GetPlantTypes());
            plantScript.AddPlants();
            Plant.SetActive(true);
            Plant.transform.parent = Plants.gameObject.transform;
            Plant.name = "Plant";
        }
    }

    void PositionSubmarine() {

        Vector3 position = new Vector3(80, 92, 0);

        GameObject submarine = Instantiate(SubmarinePrefab);
        submarine.GetComponent<SubmarineManager>().m_Submarine.m_RigidBody.transform.position = position;
        submarine.GetComponent<SubmarineManager>().m_Submarine.transform.position = position;

        Submarine = submarine.transform.Find("Submarine");

    }

}
