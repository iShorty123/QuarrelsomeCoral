using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Caves : MonoBehaviour
{
    //TODO add tilemap per caves
    public Transform BgImage = null;
    public Transform MidImage = null;

    public int minSize = 100; //cluster smallest allowed size
    public float speed = 5.0f;

    [Range(0, 100)]
    public int iniChance;
    [Range(1, 8)]
    public int birthLimit;
    [Range(1, 8)]
    public int deathLimit;

    [Range(1, 12)]
    public int numR;

    public Vector3Int tmpSize;

    private List<RandomCave> caves = null;

    private int borderLeft = -80;
    private int borderRight = 80;

    public int diff = 1;

    RuleTile Tile;
    GameObject MapGrid;
    Camera FarCamera = null;
    Camera MainCamera = null;

    RandomCave lastCave = null;

    public void Setup(Camera mainCamera, Camera farCamera, RuleTile tile, GameObject mapGrid)
    {
        MainCamera = mainCamera;
        FarCamera = farCamera;
        Tile = tile;
        MapGrid = mapGrid;

        caves = new List<RandomCave>();

        //create first cave
        GameObject cave = new GameObject();
        cave.SetActive(false);
        RandomCave caveScript = cave.AddComponent<RandomCave>();
        caveScript.Init(iniChance, birthLimit, deathLimit, numR, tmpSize, BgImage, MidImage, new Vector3Int(0, 0, 0));
        cave.SetActive(true);

        caves.Add(caveScript);
        cave.transform.parent = this.transform;
        cave.name = "RandomCave";

        //setup and create cave
        caveScript.Setup(mapGrid, MainCamera, FarCamera, Tile);
        caveScript.ConstructCave();

        lastCave = caveScript;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<RandomCave> GetCaves() {
        return caves;
    }

    public RandomCave GetLastCave() {
        return lastCave;
    }

    void AddCaveAt(Vector3Int pos, string dir) {


        //create first cave
        GameObject cave = caves[0].gameObject;//new GameObject();
        cave.SetActive(false);
        RandomCave caveScript = cave.AddComponent<RandomCave>();

        //expand border
        if (dir == "right") borderRight += tmpSize.x - diff;
        else borderLeft -= tmpSize.x + diff;

        caveScript.Init(iniChance, birthLimit, deathLimit, numR, tmpSize, BgImage, MidImage, pos);
        cave.SetActive(true);

        caves.Add(caveScript);
        cave.transform.parent = this.transform;
        cave.name = "RandomCave";

        //setup and create cave
        caveScript.Setup(MapGrid, MainCamera, FarCamera, Tile);
        caveScript.ConstructCave();

        lastCave = caveScript;
    }

    public int GetLeftBorder() {
        return borderLeft;
    }

    public int GetRightBorder()
    {
        return borderRight;
    }

    public void AddCaveToLeft(){
        Vector3Int position = new Vector3Int(borderLeft - tmpSize.x / 2 + diff, 0, 0);
        AddCaveAt(position, "left");
        print("Left*: " + borderLeft);
        print("Right: " + borderRight);
    }

    public void AddCaveToRight(){
        Vector3Int position = new Vector3Int(borderRight + tmpSize.x / 2 - diff, 0, 0);
        AddCaveAt(position, "right");
        print("Left: " + borderLeft);
        print("Right*: " + borderRight);
    }
}
