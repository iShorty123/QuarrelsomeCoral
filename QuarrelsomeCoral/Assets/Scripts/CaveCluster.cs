using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CaveCluster : MonoBehaviour
{

    private bool touchesBorder = false;
    private Vector2Int center;
    private List<Vector2Int> tiles;

    public CaveCluster()
    {
        tiles = new List<Vector2Int>();
        center = new Vector2Int(0, 0);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup()
    {
        CalculateCenter();
    }

    void CalculateCenter()
    {
        int maxX = tiles[0].x;
        int minX = tiles[0].x;
        int maxY = tiles[0].y;
        int minY = tiles[0].y;

        foreach (Vector2Int tile in tiles) {
            if (tile.x > maxX) maxX = tile.x;
            if (tile.x < minX) minX = tile.x;
            if (tile.y > maxY) maxY = tile.y;
            if (tile.y < minY) minY = tile.y;
        }

        int centerX = (maxX + minX) / 2;
        int centerY = (maxY + minY) / 2;
        center = new Vector2Int(centerX,centerY);
    }

    public List<Vector2Int> GetTiles() {
        return tiles;
    }

    public void AddTile(Vector2Int tile) {
        tiles.Add(tile);
    }

    public void AddTiles(List<Vector2Int> cluster) {

        List<Vector2Int> newTiles = new List<Vector2Int>();
        foreach (Vector2Int tile in cluster) {
            newTiles.Add(tile);
        }

        tiles = newTiles;
    }

    public bool TouchesBorder() {
        return touchesBorder;
    }

    public void MarkTouchesBorder() {
        touchesBorder = true;
    }

    public Vector2Int GetCenter() {
        return center;
    }

    public Vector2Int GetClosestBorderTile(Vector2Int toMe) {
        float smallestDist = Vector2Int.Distance(toMe, tiles[0]);
        Vector2Int closestTile = tiles[0];
        foreach (Vector2Int tile in tiles) {
            float dist = Vector2Int.Distance(toMe, tile);

            if (dist < smallestDist) {
                smallestDist = dist;
                closestTile = tile;
            }
        }
        return closestTile;
    }

    public int GetClusterSize() {
        return tiles.Count;
    }

    public void DeleteCluster(Tilemap fromMap)
    {
        foreach (Vector2Int tile in tiles)
        {
            Vector3Int pos = new Vector3Int(tile.x + fromMap.cellBounds.xMin, tile.y + fromMap.cellBounds.yMin, 0);
            fromMap.SetTile(pos, null);
        }
    }

    public void CreateBridge(CaveCluster toMe, Tilemap onMe, RuleTile withMe) {
        Vector2Int pos1 = GetClosestBorderTile(toMe.center); //my border tile closest to other (center)
        Vector2Int pos2 = toMe.GetClosestBorderTile(center); //other's border tile closest to me (center)

        //create and add tiles between border tiles TODO
        Vector2Int curr = new Vector2Int(pos1.x, pos1.y);

        int dirX = (pos2.x - pos1.x);
        int dirY = (pos2.y - pos1.y);

        if (dirX != 0) dirX /= Mathf.Abs(dirX);
        if (dirY != 0) dirY /= Mathf.Abs(dirY);

        while (curr.x != pos2.x){
            curr.x += dirX;
            onMe.SetTile(new Vector3Int(curr.x - onMe.cellBounds.xMax, curr.y - onMe.cellBounds.yMax, 0), withMe);
            onMe.SetTile(new Vector3Int(curr.x - onMe.cellBounds.xMax, curr.y - onMe.cellBounds.yMax + 1, 0), withMe);
            onMe.SetTile(new Vector3Int(curr.x - onMe.cellBounds.xMax, curr.y - onMe.cellBounds.yMax - 1, 0), withMe);
            AddTile(curr);
        }
        while (curr.y != pos2.y){
            curr.y += dirY;
            onMe.SetTile(new Vector3Int(curr.x - onMe.cellBounds.xMax, curr.y - onMe.cellBounds.yMax , 0), withMe);
            onMe.SetTile(new Vector3Int(curr.x - onMe.cellBounds.xMax + 1, curr.y - onMe.cellBounds.yMax, 0), withMe);
            onMe.SetTile(new Vector3Int(curr.x - onMe.cellBounds.xMax - 1, curr.y - onMe.cellBounds.yMax, 0), withMe);
            AddTile(curr);
        }
    }

    public void ColorCluster(Tilemap map) {
        foreach (Vector2Int tile in tiles)
        {
            Vector3Int pos = new Vector3Int(tile.x + map.cellBounds.xMin, tile.y + map.cellBounds.yMin, 0);
            map.SetColor(pos, Color.red);
        }

    }

}
