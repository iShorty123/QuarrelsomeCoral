﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MainController : MonoBehaviour
{
	public RandomEnemy REnemy = null;
    public RandomPlant RPlant = null;
    public Caves Caves = null;
    private Transform Submarine = null;

    public RuleTile Tile;
    public GameObject MapGrid = null;
    public GameObject SubmarinePrefab = null;

    public Camera MainCamera = null;
    public Camera FarCamera = null;

    // Start is called the first frame update
    void Start()
    {
        //put submarine in initial position
        PositionSubmarine();

        //create caves
        Caves.Setup(MainCamera, FarCamera, Tile, MapGrid);

        //SetupLastCave();
    }

    // Update is called once per frame
    void Update()
    {
        //if submarine is close to the left or right border of Caves, add a cave at that side.

        if ((Submarine.transform.position.x - Caves.GetLeftBorder()) < 75)
        {
            Caves.AddCaveToLeft();
            //SetupLastCave();
        }

        if ((Caves.GetRightBorder() - Submarine.transform.position.x) < 75) 
        {
            Caves.AddCaveToRight();
            //SetupLastCave();
        }

    }

    void SetupLastCave() {

        RandomCave lastCave = Caves.GetLastCave();
        Tilemap map = lastCave.GetMap();

        //add random plants
        RPlant.Setup(map, MainCamera, lastCave.transform.position + new Vector3(1 ,1 , 0));
        RPlant.AddPlants();

        //start random enemy spawn
        REnemy.Setup(map, MainCamera);
        REnemy.StartEnemySpawn();
    }

    void PositionSubmarine() {

        Vector3 position = new Vector3(0, 60, 0);

        GameObject submarine = Instantiate(SubmarinePrefab);
        submarine.transform.position = position;
        Submarine = submarine.transform.Find("Submarine");

    }

}
