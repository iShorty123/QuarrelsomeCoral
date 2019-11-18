﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bosses : MonoBehaviour
{

    public GameObject BossPrefab = null;

    GameObject MapGrid = null;
    Camera MainCamera = null;

    public int SpawnTime = 15;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup(GameObject mapGrid, Camera mainCamera)
    {
        MapGrid = mapGrid;
        MainCamera = mainCamera;
    }

    public void StartBossSpawn() {
        StartCoroutine(BossSpawner());
    }

    IEnumerator BossSpawner()
    {
        while (true)
        {
            yield return new WaitForSeconds(SpawnTime);
            SpawnBoss();
        }
    }

    public void SpawnBoss()
    {
        Vector3 position = getRandomScreenPosition();
        Vector3Int mapPosition = Vector3Int.RoundToInt(position);

        GameObject boss = Instantiate(BossPrefab);
        BossBehaviour bossScript = boss.AddComponent<BossBehaviour>();
        bossScript.Setup(MapGrid);
        boss.transform.position = position;
        boss.transform.localScale = new Vector3(boss.transform.localScale.x * 3, boss.transform.localScale.y * 3, 1);
        boss.transform.parent = this.transform;
    }

    Vector3 getRandomScreenPosition()
    {

        float halfHeight = MainCamera.orthographicSize;
        float halfWidth = MainCamera.aspect * halfHeight;
        Vector3 camPosition = MainCamera.transform.position;

        //small square -> aprox size of the submarine
        float minX = 10;
        float minY = 5;

        //large square
        float maxX = halfWidth * 3;
        float maxY = halfHeight * 3;

        float xLeft = camPosition.x - Random.Range(minX, maxX);
        float xRight = camPosition.x + Random.Range(minX, maxX);
        float x = Random.Range(-1f, 1f) < 0 ? xRight : xLeft;

        float y = camPosition.y - Random.Range(minY, maxY);
        if (y > 92) y = 87;

        Vector3 screenPosition = new Vector3(x, y, -4);
        return screenPosition;
    }

}