using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RandomEnemy : MonoBehaviour
{
    GameObject MapGrid = null;
    Camera MainCamera = null;

    public GameObject BlueFish = null;

    public int SpawnTime = 1;

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

    public void StartEnemySpawn() {
        StartCoroutine(EnemySpawner());
    }

    IEnumerator EnemySpawner()
    {
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(SpawnTime);
        }
    }

    public void SpawnEnemy()
    {
        Vector3 position = getRandomScreenPosition();
        Vector3Int mapPosition = new Vector3Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), 0);

        //Tilemap map = null;
        //Vector3 maxDist = new Vector3(float.MaxValue, float.MaxValue, 0);

        //foreach (Transform mapObj in MapGrid.transform) {

        //    Tilemap m = mapObj.gameObject.GetComponent<Tilemap>();
        //    if (Vector3.Distance(m.transform.position, position) < Vector3.Distance(maxDist, position)) {
        //        maxDist = m.transform.position;
        //        map = m;
        //    }
        //}

        //map needs to be closest map to the
        bool enemyOnMap = false;
        Tilemap[] maps = FindObjectsOfType<Tilemap>();

        foreach (Tilemap map in maps) {
            if (map.GetTile(mapPosition) != null) {
                enemyOnMap = true;
                break;
            }
        }

        while (enemyOnMap) {
            position = getRandomScreenPosition();
            mapPosition = new Vector3Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), 0);

            enemyOnMap = false;
            foreach (Tilemap map in maps)
            {
                if (map.GetTile(mapPosition) != null)
                {
                    enemyOnMap = true;
                    break;
                }
            }
        }

        GameObject enemy = Instantiate(BlueFish);
        enemy.transform.position = position;
        enemy.transform.parent = this.transform;
    }

    Vector3 getRandomScreenPosition() {

        float halfHeight = MainCamera.orthographicSize;
        float halfWidth = MainCamera.aspect * halfHeight;
        Vector3 camPosition = MainCamera.transform.position;

        //small square
        float minX = halfWidth * 2;
        float minY = halfHeight * 2;

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
