﻿using System.Collections;
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
    public Material m_LightMaterial;

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

        AddFirstCave();
        //AddCaveToRight(true);
        //AddCaveToLeft(true);
        //AddCaveToRight(true);
        //AddCaveToLeft(true);
        AddCaveToRight(false);
        AddCaveToLeft(false);
        AddCaveToRight(false);
        AddCaveToLeft(false);

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

    public RandomCave GetFirstCave()
    {
        return caves[0];
    }

    void AddFirstCave() {

        //create first cave
        GameObject cave = new GameObject();
        cave.SetActive(false);
        RandomCave caveScript = cave.AddComponent<RandomCave>();
        caveScript.Init(iniChance, birthLimit, deathLimit, numR, tmpSize, BgImage, MidImage, new Vector3Int(0, 0, 0), minSize, m_LightMaterial);
        cave.SetActive(true);

        caves.Add(caveScript);
        cave.name = "RandomCave" + transform.childCount.ToString();
        cave.transform.parent = this.transform;

        //setup and create cave
        //caveScript.SetAsFirstCave();
        caveScript.Setup(MapGrid, MainCamera, FarCamera, Tile);
        caveScript.ConstructCave();

        lastCave = caveScript;

    }

    void AddCaveAt(Vector3Int pos, string dir, bool isFirst) {

        //create cave
        GameObject cave = new GameObject();
        cave.SetActive(false);
        RandomCave caveScript = cave.AddComponent<RandomCave>();

        //expand border
        if (dir == "right") borderRight += tmpSize.x - diff;
        else borderLeft -= tmpSize.x + diff;

        caveScript.Init(iniChance, birthLimit, deathLimit, numR, tmpSize, BgImage, MidImage, pos, minSize, m_LightMaterial);
        cave.SetActive(true);

        caves.Add(caveScript);
        cave.name = "RandomCave" + transform.childCount.ToString();
        cave.transform.parent = this.transform;

        //setup and create cave
        if (isFirst) caveScript.SetAsFirstCave();
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

    public void AddCaveToLeft(bool isFirst = false)
    {
        Vector3Int position = new Vector3Int(borderLeft - tmpSize.x / 2 + diff, 0, 0);
        AddCaveAt(position, "left", isFirst);
    }

    public void AddCaveToRight(bool isFirst = false)
    {
        Vector3Int position = new Vector3Int(borderRight + tmpSize.x / 2 - diff, 0, 0);
        AddCaveAt(position, "right", isFirst);
    }
}
