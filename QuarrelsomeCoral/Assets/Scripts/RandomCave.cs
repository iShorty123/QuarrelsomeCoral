using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

//Based on tutorial: https://www.youtube.com/watch?v=xNqqfABXTNQ

//Instructions:
//Click "C" to toggle far & near cameras
//When in near camera, use arrows to move around


public class RandomCave : MonoBehaviour
{

    Camera FarCamera = null;
    Camera MainCamera = null;
    public Transform BgImage = null;
    public Transform MidImage = null;

    private CaveCluster borderTile = null;

    //cave creation support
    private List<CaveCluster> clusters;
    private bool[,] claimed; //whether a tile has been claimed by a cluster already
    int minSize = 100; //cluster smallest allowed size

    Vector2Int[] adj = new[] { new Vector2Int(-1, 1), new Vector2Int(0,1), new Vector2Int(1,1), new Vector2Int(-1,0), new Vector2Int(1,0), new Vector2Int(-1,-1),
        new Vector2Int(0,-1), new Vector2Int(1,-1)};

    private bool currCamera = false; //0 is main, 1 is far
    public float speed = 5.0f;

    [Range(0, 100)]
    public int iniChance;
    [Range(1, 8)]
    public int birthLimit;
    [Range(1, 8)]
    public int deathLimit;

    [Range(1, 12)]
    public int numR;

    private int[,] terrainMap;
    public Vector3Int tmpSize;
    Tilemap topMap;
    RuleTile topTile;

    int width;
    int height;

    Vector3Int origin;

    bool isDone = false;
    bool isFirstCave = false;

    //WaitForSeconds waitFor = new WaitForSeconds(.07f);
    int waitFor = 0;

    private List<CaveCluster> visitedClusters;

    public RandomCave() {
        clusters = new List<CaveCluster>();
    }

    public void Init(int iniC, int birthL, int deathL, int nR, Vector3Int tmp, Transform bgImage, Transform midImage, Vector3Int pos, int minS)
    {
        iniChance = iniC;
        birthLimit = birthL;
        deathLimit = deathL;
        numR = nR;
        tmpSize = tmp;

        BgImage = bgImage;
        MidImage = midImage;

        origin = pos;

        minSize = minS;

        clusters = new List<CaveCluster>();
    }

    public void Setup(GameObject mapGrid, Camera mainCamera, Camera farCamera, RuleTile tile)
    {
        GameObject tileMap = new GameObject();
        topMap = tileMap.AddComponent<Tilemap>();
        tileMap.AddComponent<TilemapRenderer>();
        tileMap.AddComponent<TilemapCollider2D>();
        //tileMap.AddComponent<Rigidbody2D>();
        tileMap.name = "Tilemap";
        tileMap.transform.parent = mapGrid.transform;
        tileMap.transform.position = origin;

        MainCamera = mainCamera;
        FarCamera = farCamera;
        topTile = tile;
    }

    public Tilemap GetMap() {
        return topMap;
    }

    public bool IsDone() {
        bool ret = isDone;
        isDone = false;
        return ret;
    }

    public void SetAsFirstCave() {
        isFirstCave = true;
    }

    IEnumerator doSim(int nu)
    {
        clearMap(false);
        width = tmpSize.x;
        height = tmpSize.y;

        if (terrainMap == null)
        {
            terrainMap = new int[width, height];
            initPos();
        }


        for (int i = 0; i < nu; i++)
        {
            terrainMap = genTilePos(terrainMap);
            if (!isFirstCave) yield return waitFor;
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (terrainMap[x, y] == 1)
                    topMap.SetTile(new Vector3Int(x - width / 2, y - height / 2, 0), topTile);
            }
            if (x % 10 == 0 && !isFirstCave) yield return waitFor;
        }

        MakeCaves();
    }

    public void initPos()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (y > height - 10 || x < 3 || x > width - 3) terrainMap[x, y] = 0;
                else terrainMap[x, y] = Random.Range(1, 101) < iniChance ? 1 : 0;
            }
        }
    }


    public int[,] genTilePos(int[,] oldMap)
    {
        int[,] newMap = new int[width, height];
        int neighb;
        BoundsInt myB = new BoundsInt(-1, -1, 0, 3, 3, 1);


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                neighb = 0;
                foreach (var b in myB.allPositionsWithin)
                {
                    if (b.x == 0 && b.y == 0) continue;
                    if (x + b.x >= 0 && x + b.x < width && y + b.y >= 0 && y + b.y < height)
                    {
                        neighb += oldMap[x + b.x, y + b.y];
                    }
                    else
                    {
                        neighb++;
                    }
                }

                if (oldMap[x, y] == 1)
                {
                    if (neighb < deathLimit) newMap[x, y] = 0;

                    else
                    {
                        newMap[x, y] = 1;

                    }
                }

                if (oldMap[x, y] == 0)
                {
                    if (neighb > birthLimit) newMap[x, y] = 1;

                    else
                    {
                        newMap[x, y] = 0;
                    }
                }
            }
        }
        return newMap;
    }

    private void Start()
    {

    }


    void Update()
    {

        if (Input.GetKeyDown("p"))
        {
            ToggleCamera();
        }

    }

    public void ConstructCave() { 
        StartCoroutine(doSim(numR));
        //MakeCaves();
    }

    void ToggleCamera() {
        if (currCamera)
        {
            MainCamera.enabled = false;
            MainCamera.enabled = true;
        }
        else
        {
            MainCamera.enabled = true;
            MainCamera.enabled = false;
        }

        BgImage.gameObject.GetComponent<SpriteRenderer>().enabled ^= true;
        MidImage.gameObject.GetComponent<SpriteRenderer>().enabled ^= true;

        currCamera = !currCamera;
    }

    public void clearMap(bool complete)
    {
        topMap.ClearAllTiles();
        if (complete)
        {
            terrainMap = null;
        }
    }

    IEnumerator MakeClusters()
    {
        claimed = new bool[width, height];

        for (int i = 0; i < width; i++) {
            for (int j = 0; j < height; j++)
            {
                if(!claimed[i,j]) {
                    MakeClusterDFS(new Vector2Int(i, j));
                }
            }
            if (!isFirstCave) yield return waitFor;
        }

        borderTile = clusters[0];

        //2. for each cluster, if not connected to border, connect to closest cluster
        StartCoroutine(BuildCaves());
    }

    void MakeClusterDFS(Vector2Int start) {

        CaveCluster cluster = new CaveCluster();

        //add neighbors to queue
        Stack<Vector2Int> st = new Stack<Vector2Int>();
        st.Push(start);

        //loop through all nodes in queue
        while (st.Count != 0)
        {

            Vector2Int curr = st.Pop();
            cluster.AddTile(curr);

            int x = curr.x;
            int y = curr.y;
            claimed[x, y] = true;
            //if(x==9 && y==0) topMap.SetColor(new Vector3Int(x - width / 2, y - height / 2, 0), Color.red);

            for (int i = 0; i < 8; i++) {
                Vector2Int ad = new Vector2Int(x + adj[i].x, y + adj[i].y);
                Vector3Int locPos = new Vector3Int(ad.x - width / 2, ad.y - height / 2, 0);

                //if out of bounds, then say cluster touches border and skip this one
                if (locPos.x < topMap.cellBounds.xMin || locPos.x >= topMap.cellBounds.xMax || locPos.y < topMap.cellBounds.yMin || locPos.y >= topMap.cellBounds.yMax) {
                    cluster.MarkTouchesBorder();
                }
                else if (!claimed[ad.x, ad.y] && topMap.HasTile(locPos)) //terrainMap[ad.x, ad.y] == 1
                {
                    claimed[ad.x, ad.y] = true;
                    st.Push(ad);
                }
            }
        }

        if (cluster.TouchesBorder() && cluster.GetClusterSize() < 100) {
            cluster.DeleteCluster(topMap);
        }
        else if (cluster.GetClusterSize() < minSize)
        {
            cluster.DeleteCluster(topMap);
        }
        else
        {
            cluster.Setup();
            clusters.Add(cluster);
        }

    }

    void MakeCaves() 
    {
        //1. find clusters & eliminate small ones
        //MakeClusters(); //TODO add coroutine
        StartCoroutine(MakeClusters());

        //borderTile = clusters[0];

        //2. for each cluster, if not connected to border, connect to closest cluster
        //BuildCaves(); //TODO add coroutine
    }

    IEnumerator BuildCaves() {

        for (int i = 0; i < clusters.Count; i++){
           //if (!clusters[i].ShouldBeRemoved()) 
            BuildCavesR(clusters[i], clusters[(i + 1) % clusters.Count]);
            if (!isFirstCave) yield return waitFor;
        }

        //foreach (CaveCluster cluster in clusters.ToList())
        //{
        //    if (cluster.ShouldBeRemoved()) clusters.Remove(cluster);
        //}

        isDone = true;
    }

    void BuildCavesR(CaveCluster cluster, CaveCluster neighb)
    {
        if (cluster.TouchesBorder()) {
            borderTile = cluster;
        } else {
            //CaveCluster closest = GetClosestClusterTo(cluster);
            ConnectClusters(cluster, neighb);
        }
    }

    void ConnectClusters(CaveCluster a, CaveCluster b)
    {

        //create tiles in between and add to a
        a.CreateBridge(b, topMap, topTile);

        //merge b into a
        //a.AddTiles(b.GetTiles());

        //remove b from cluster list
        // clusters.Remove(b);
        //b.setShouldBeRemoved(true);

        //recalculate values (like cluster's center)
        a.Setup();

        //if (b.TouchesBorder()) a.MarkTouchesBorder();
    }

    //CaveCluster GetClosestClusterTo(CaveCluster fromHere)
    //{
    //    float smallestDist = Vector2Int.Distance(fromHere.GetCenter(), clusters[0].GetCenter());
    //    CaveCluster closestCluster = clusters[0];

    //    foreach (CaveCluster cluster in clusters)
    //    {
    //        if (cluster.ShouldBeRemoved() || !cluster.IsConnected(cluster)) continue;
    //        float dist = Vector2Int.Distance(fromHere.GetCenter(), cluster.GetCenter());
    //        if (dist != 0f && dist <= smallestDist)
    //        {
    //            smallestDist = dist;
    //            closestCluster = cluster;
    //        }
    //    }
    //    return closestCluster;
    //}


}
