using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AutomaticEnemy : MonoBehaviour
{
    public Tilemap Map = null;
    public Camera MainCamera = null;

    public GameObject BlueFish = null;

    public int SpawnTime = 3;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EnemySpawner());
        //SpawnEnemy();
    }

    // Update is called once per frame
    void Update()
    {

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
        Vector3Int mapPosition = new Vector3Int(Mathf.RoundToInt(position.x + Map.cellBounds.xMin), Mathf.RoundToInt(position.y + Map.cellBounds.yMin), 0);

        while (Map.GetTile(mapPosition) != null) {
            position = getRandomScreenPosition();
            mapPosition = new Vector3Int(Mathf.RoundToInt(position.x + Map.cellBounds.xMin), Mathf.RoundToInt(position.y + Map.cellBounds.yMin), 0);
        }

        GameObject enemy = Instantiate(BlueFish);
        enemy.transform.position = position;
        enemy.transform.parent = this.transform;
    }

    Vector3 getRandomScreenPosition() {

        float halfHeight = MainCamera.orthographicSize;
        float halfWidth = MainCamera.aspect * halfHeight;

        float y = Random.Range(-halfHeight, halfHeight);
        float x = Random.Range(-halfWidth, halfWidth);

        Vector2 CameraPos = new Vector2(MainCamera.transform.position.x, MainCamera.transform.position.y);
        Vector3 screenPosition = new Vector3(CameraPos.x + x, CameraPos.y + y, -4);

        return screenPosition;
    }
}
