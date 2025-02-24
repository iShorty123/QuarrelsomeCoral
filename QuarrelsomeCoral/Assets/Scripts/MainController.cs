﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MainController : MonoBehaviour
{

    public GameObject LoadingScreen = null;

    public RandomEnemy REnemy = null;
    public Bosses RBoss = null;
    public Caves Caves = null;
    public Plants Plants = null;

    public PointsOfInterest PointsOfInterest = null;

    private Transform Submarine = null;

    public RuleTile Tile;
    public GameObject MapGrid = null;
    public GameObject SubmarinePrefab = null;

    public Camera MainCamera = null;
    public Camera FarCamera = null;

    bool submarinePositioned = false;

    private static MainController m_Instance;

    private void Awake()
    {
        m_Instance = this;

        showLoadingView();
    }

    public static MainController GetInstance()
    {
        if (m_Instance != null)
        {
            return m_Instance;
        }
        else
        {
            Debug.LogError("Main Controller is not initialized.");
        }
        return null;
    }

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

        SetupFirstCaves();

        StartCoroutine(SetupLastCave());
    }

    void showLoadingView(){
        if (LoadingScreen != null) LoadingScreen.SetActive(true);
    }

    void hideLoadingView()
    {
        if (LoadingScreen != null) LoadingScreen.SetActive(false);
    }

    void SetupFirstCaves()
    {
        if (!Caves.GetFirstCave().IsDoneBuilding()) return;

        //here stop progress

        List<RandomCave> caves = Caves.GetCaves();

        foreach (RandomCave cave in caves)
        {
            Debug.Log("h");
            if (!cave.IsDoneBuilding() && !cave.FinishedOnceAlready()) { Debug.Log("r"); return; }
        }

        foreach (RandomCave cave in caves)
        {
            if (!cave.plantsDone) SetupCave(cave);          
        }

        foreach (RandomCave cave in caves)
        {
            SetupPoints(cave);
            cave.IsDone();
        }

        hideLoadingView();

    }

    IEnumerator SetupLastCave() {

        RandomCave lastCave = Caves.GetLastCave();
        if (lastCave.IsDone())
        {
            SetupCave(lastCave);
            yield return new WaitForEndOfFrame();
            SetupPoints(lastCave);
        }

       
    }


    void SetupCave(RandomCave cave) {
        Tilemap map = cave.GetMap();
        GameObject Plant = new GameObject();
        Plant.SetActive(false);
        RandomPlant plantScript = Plant.AddComponent<RandomPlant>();
        plantScript.Setup(map, MainCamera, map.transform.position + new Vector3(1, 1, 0), Plants.GetPlantTypes());
        plantScript.AddPlants();
        Plant.SetActive(true);
        Plant.name = "Plant" + map.name[map.name.Length - 1];
        Plant.transform.parent = Plants.gameObject.transform;

        cave.plantsDone = true;
    }

    public void SetupPoints(RandomCave cave)
    {
        Tilemap map = cave.GetMap();
        GameObject PointOfInterest = new GameObject();
        PointOfInterest.SetActive(false);
        RandomPointOfInterest pointOfInterestScript = PointOfInterest.AddComponent<RandomPointOfInterest>();
        pointOfInterestScript.Setup(map, MainCamera, map.transform.position + new Vector3(1, 1, 0), PointsOfInterest.GetPointOfInterestTypes());
        pointOfInterestScript.AddPointsOfInterest();
        PointOfInterest.SetActive(true);
        PointOfInterest.name = "PointOfInterest" + map.name[map.name.Length - 1];
        PointOfInterest.transform.parent = PointsOfInterest.gameObject.transform;

        cave.finishedOnceAlready = true;
    }

    void PositionSubmarine() {

        Vector3 position = new Vector3(0, 80f, 0);

        GameObject submarine = Instantiate(SubmarinePrefab);
        submarine.GetComponent<SubmarineManager>().m_Submarine.m_RigidBody.transform.position = position;
        submarine.GetComponent<SubmarineManager>().m_Submarine.transform.position = position;

        Submarine = submarine.transform.Find("Submarine");

    }

}
