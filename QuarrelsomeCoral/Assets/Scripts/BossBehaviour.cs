using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BossBehaviour : MonoBehaviour
{

    GameObject MapGrid = null;

    Vector2Int[] adj = new[] { new Vector2Int(-1, 1), new Vector2Int(0,1), new Vector2Int(1,1), new Vector2Int(-1,0), new Vector2Int(1,0), new Vector2Int(-1,-1),
        new Vector2Int(0,-1), new Vector2Int(1,-1), new Vector2Int(-1, 2), new Vector2Int(0,2), new Vector2Int(1,2), new Vector2Int(-2,0), new Vector2Int(2,0),
        new Vector2Int(-1,-2), new Vector2Int(0,-2), new Vector2Int(1,-2), new Vector2Int(0,3), new Vector2Int(0,-3), new Vector2Int(-3,0), new Vector2Int(3,0),
        new Vector2Int(-2,-1), new Vector2Int(-2,1), new Vector2Int(2,-1), new Vector2Int(2,1)};

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Setup(GameObject mapGrid)
    {
        MapGrid = mapGrid;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        print(collision.collider.name);
        Tilemap map = collision.collider.GetComponent<Tilemap>();
        //print(map);
        if (map != null)
        {

            Vector3 collisionPoint = new Vector3(collision.GetContact(0).point.x, collision.GetContact(0).point.y, 0);
            Vector3Int pos = Vector3Int.FloorToInt(collisionPoint - map.transform.position);

            char number = map.name[map.name.Length - 1];
            GameObject plantObject = GameObject.Find("Plant" + number);
            RandomPlant plant = plantObject.GetComponent<RandomPlant>();
            GameObject[,] plantArray = plant.GetPlants();

            //delete hit tile
            map.SetTile(pos, null);
            Vector2Int plantPos = new Vector2Int(pos.x + map.size.x / 2, pos.y + map.size.y / 2);
            if (plantArray[plantPos.x, plantPos.y] != null) Destroy(plantArray[plantPos.x, plantPos.y]);

            //delete surrounding tiles
            for (int i = 0; i < 24; i++)
            {
                Vector3Int adjPos = new Vector3Int(pos.x + adj[i].x, pos.y + adj[i].y, 0);
                map.SetTile(adjPos, null);
                plantPos = new Vector2Int(adjPos.x + map.size.x / 2, adjPos.y + map.size.y / 2);
                if (plantArray[plantPos.x, plantPos.y] != null) Destroy(plantArray[plantPos.x, plantPos.y]);
            }

        }
    }
}
