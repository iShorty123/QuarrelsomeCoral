using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MainController : MonoBehaviour
{
	public RandomEnemy REnemy = null;
    public Bosses RBoss = null;
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

        //start boss spawn (random for now)
        RBoss.Setup(MapGrid, MainCamera);
        RBoss.StartBossSpawn();
    }

    // Update is called once per frame
    void Update()
    {
        //if submarine is close to the left or right border of Caves, add a cave at that side.
        if ((Submarine.transform.position.x - Caves.GetLeftBorder()) < 140) Caves.AddCaveToLeft(); //here
        if ((Caves.GetRightBorder() - Submarine.transform.position.x) < 140) Caves.AddCaveToRight(); //here

        SetupTwoFirstCaves();

        SetupLastCave();
    }

    void SetupTwoFirstCaves()
    {
        if (!Caves.GetFirstCave().IsDone()) return;

        List<RandomCave> caves = Caves.GetCaves();

        foreach (RandomCave cave in caves)
        {
            SetupCave(cave);
            cave.IsDone();
        }
    }

    void SetupLastCave() {

        RandomCave lastCave = Caves.GetLastCave();
        if (lastCave.IsDone()) SetupCave(lastCave);
       
    }

    void SetupCave(RandomCave cave) {
        Tilemap map = cave.GetMap();
        GameObject Plant = new GameObject();
        Plant.SetActive(false);
        RandomPlant plantScript = Plant.AddComponent<RandomPlant>();
        plantScript.Setup(map, MainCamera, map.transform.position + new Vector3(1, 1, 0), Plants.GetPlantTypes());
        plantScript.AddPlants();
        Plant.SetActive(true);
        Plant.name = "Plant" + Plants.transform.childCount.ToString();
        Plant.transform.parent = Plants.gameObject.transform;
    }

    void PositionSubmarine() {

        Vector3 position = new Vector3(80, 92, 0);

        GameObject submarine = Instantiate(SubmarinePrefab);
        submarine.GetComponent<SubmarineManager>().m_Submarine.m_RigidBody.transform.position = position;
        submarine.GetComponent<SubmarineManager>().m_Submarine.transform.position = position;

        Submarine = submarine.transform.Find("Submarine");

    }

}
