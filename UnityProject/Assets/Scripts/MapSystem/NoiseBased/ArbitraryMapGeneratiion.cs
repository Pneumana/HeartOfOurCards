using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using System.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

public class ArbitraryMapGeneratiion : MonoBehaviour
{

    int placeAttempts;


    public List<Vector3Int> mapPositions = new List<Vector3Int>();

    //remove from this list when a room gets it's special type
    public List<Vector3Int> placedRooms = new List<Vector3Int>();

    //available neighboring rooms to spawn rooms
    public List<Vector3Int> openList = new List<Vector3Int>();
    //rooms the player can see
    public List<Vector3Int> explorableRooms = new List<Vector3Int>();
    public List<Vector3Int> revealedRooms = new List<Vector3Int>();
    public List<Vector3Int> completedRooms = new List<Vector3Int>();

    public List<List<Vector3Int>> adjacantRooms = new List<List<Vector3Int>>();

    public Sprite icon;

    public Tilemap world;
    public Tilemap fogOfWar;
    public Tile tile;
    public Tile endpoint;
    public Tile startpoint;
    public Tile fogBorder;
    public Tile fog;
    public Tile fogVisible;

    public Tile romance;
    public Tile encounter;
    public Tile shop;

    [Header("Generator Settings")]
    public int size;
    public Vector2 gridSize = new Vector2(5, 5);
    [Header("Leave blank for random")]
    public int seed;
    [Header("Number of special tiles the generator will attempt to spawn, in order")]
    public int romancesToSpawn;
    public int encountersToSpawn;
    public int shopsToSpawn;
    public float allowedRepeatDistance;

    public int discoverRadius = 1;

    //need a list of adjacant positions for each placed room, excluding positions that are already used
    public List<Vector3Int> firstadjacents = new List<Vector3Int>();

    private void Awake()
    {
        //set seed to whatever run manager says it is
        //set explorablerooms, revealed rooms, and completed rooms to the run manager's list
        var rm = RunManager.instance;
        explorableRooms = rm.explorableRooms;
        completedRooms = rm.completedRooms;
        revealedRooms = rm.revealedRooms;
        seed = AmbidexterousManager.Instance.seed;
    }

    private void Start()
    {
        if(seed ==0)
            seed = Random.Range(int.MinValue, int.MaxValue);
        Random.InitState(seed);
        for(int x = 0; x< gridSize.x; x++)
        {
            for(int y = 0; y < gridSize.y; y++)
            {
                mapPositions.Add(new Vector3Int(x, y));
            }
        }
        placeAttempts = size;
        Vector3Int center = new Vector3Int(Mathf.FloorToInt(gridSize.x / 2), Mathf.FloorToInt(gridSize.y / 2));
        var startneighbors = GetNeighborsAt(center.x,center.y);
        foreach (Vector3Int neighbor in startneighbors)
        {
            if (!placedRooms.Contains(neighbor))
            {
                openList.Add(neighbor);
            }
        }
        placedRooms.Add(center);
        while (placeAttempts > 0)
        {
            placeAttempts--;
            //create room

            //pick a random node to start on
            /*int a = (int)Random.Range(0, gridSize.x-1);
            int b = (int)Random.Range(0, gridSize.y - 1);
            mapPositions.Remove(new Vector3Int(a,b));
            placedRooms.Add(new Vector3Int(a, b));
            adjacantRooms.Add(CheckValidPosition(a,b));*/


            if (openList.Count == 0)
                continue;
            //List<Vector3Int> pickedList = adjacantRooms[Random.Range(0, adjacantRooms.Count-1)];
            var pick = openList[Random.Range(0, openList.Count - 1)];
            if (placedRooms.Contains(pick))
                continue;
            placedRooms.Add(pick);
            openList.Remove(pick);
            Debug.Log("removed " + pick);
            var a = pick.x;
            var b = pick.y;
            var neighbors = GetNeighborsAt((int)a,(int)b);
            foreach (Vector3Int neighbor in neighbors)
            {
                    if (!placedRooms.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
            }
/*            var worldPos = world.CellToWorld(pick);
            Debug.DrawLine(worldPos + new Vector3(0.5f, -0.5f), worldPos + new Vector3(-0.5f, 0.5f), Color.red, 5);
            Debug.DrawLine(worldPos + new Vector3(-0.5f, -0.5f), worldPos + new Vector3(0.5f, 0.5f), Color.red, 5);*/
            //find a random neighbor room
            //add to placed rooms
            //loop trhrough all neighbor lists and remove that position
            /*var mostrecentAddition = new Vector3(a * 0.8659766f, b);
            Debug.DrawLine(mostrecentAddition + new Vector3(0.5f, -0.5f), mostrecentAddition + new Vector3(-0.5f, 0.5f), Color.red, 10);
            Debug.DrawLine(mostrecentAddition + new Vector3(-0.5f, -0.5f), mostrecentAddition + new Vector3(0.5f, 0.5f), Color.red, 10);*/
        }
        Vector3Int furthestPair1 = Vector3Int.zero;
        Vector3Int furthestPair2 = Vector3Int.zero;
        float dist = 0;
        foreach(Vector3Int pos in placedRooms)
        {
            /*Debug.DrawLine(pos + new Vector3(0.5f, -0.5f), pos + new Vector3(-0.5f, 0.5f), Color.green, 10);
            Debug.DrawLine(pos + new Vector3(-0.5f, -0.5f), pos + new Vector3(0.5f, 0.5f), Color.green, 10);*/

/*                        var tile = new GameObject();
                        tile.name = "MapTile";
                        tile.transform.position = pos;
                        tile.AddComponent<SpriteRenderer>();
                        tile.GetComponent<SpriteRenderer>().sprite = icon;*/
            //setTile
            Vector3Int posInt = new Vector3Int(pos.x, pos.y);

            world.SetTile(posInt, tile);
            var worldPos = world.CellToWorld(posInt);
            /*Debug.DrawLine(worldPos + new Vector3(0.5f, -0.5f), worldPos + new Vector3(-0.5f, 0.5f), Color.red, 10);
            Debug.DrawLine(worldPos + new Vector3(-0.5f, -0.5f), worldPos + new Vector3(0.5f, 0.5f), Color.red, 10);*/
            foreach (Vector3Int ps in placedRooms)
            {
                if (ps != pos)
                {
                    var measure = Vector3.Distance(world.CellToWorld(pos), world.CellToWorld(ps));
                    if (measure > dist)
                    {
                        dist = measure;
                        furthestPair1 = ps;
                        furthestPair2 = pos;
                    }
                }
            }
        }
        DrawAdjacents();
        void DrawAdjacents()
        {
            var center = new Vector3Int(Mathf.FloorToInt(gridSize.x / 2), Mathf.FloorToInt(gridSize.y / 2));
            var str = world.CellToWorld(center);
            Debug.DrawLine(str + new Vector3(0.5f, -0.5f), str + new Vector3(-0.5f, 0.5f), Color.green, 10);
            Debug.DrawLine(str + new Vector3(-0.5f, -0.5f), str + new Vector3(0.5f, 0.5f), Color.green, 10);

            firstadjacents.Clear();

            firstadjacents.Add(center + new Vector3Int(0, 1));
            firstadjacents.Add(center + new Vector3Int(0, -1));
            firstadjacents.Add(center + new Vector3Int(1, 0));
            firstadjacents.Add(center + new Vector3Int(-1, 0));
            firstadjacents.Add(center + new Vector3Int(-1, 1));
            firstadjacents.Add(center + new Vector3Int(-1, -1));

            for (int i =0; i < firstadjacents.Count; i++)
            {
                Vector3Int first = firstadjacents[i];
                float part = (float)i / firstadjacents.Count;
                Color c = new Color(part,part,part);
                var worldPos = world.CellToWorld(first);
                Debug.DrawLine(worldPos + new Vector3(0.5f, -0.5f), worldPos + new Vector3(-0.5f, 0.5f), c, 10);
                Debug.DrawLine(worldPos + new Vector3(-0.5f, -0.5f), worldPos + new Vector3(0.5f, 0.5f), c, 10);
            }
        }
        foreach(List<Vector3Int> rooms in adjacantRooms)
        {
            foreach(Vector3Int pos in rooms)
            {
                var str = world.CellToWorld(pos);
                //Debug.DrawLine(str + new Vector3(0.5f, -0.5f), str + new Vector3(-0.5f, 0.5f), Color.cyan, 10);
                //Debug.DrawLine(str + new Vector3(-0.5f, -0.5f), str + new Vector3(0.5f, 0.5f), Color.cyan, 10);
            }
        }

/*            foreach (Vector3Int pos in placedRooms)
            {
                var str = world.CellToWorld(pos);
                Debug.DrawLine(str + new Vector3(0.5f, -0.5f), str + new Vector3(-0.5f, 0.5f), Color.green, 10);
                Debug.DrawLine(str + new Vector3(-0.5f, -0.5f), str + new Vector3(0.5f, 0.5f), Color.green, 10);
            }*/
        foreach (Vector3Int pos in openList)
        {
            if (!placedRooms.Contains(pos))
            {
                var str = world.CellToWorld(pos);
                Debug.DrawLine(str + new Vector3(0.25f, -0.25f), str + new Vector3(-0.25f, 0.25f), Color.magenta, 10);
                Debug.DrawLine(str + new Vector3(-0.25f, -0.25f), str + new Vector3(0.25f, 0.25f), Color.magenta, 10);
            }

        }

        

        //Vector3Int fp1Int = new Vector3Int(furthestPair1.x), Mathf.FloorToInt(furthestPair1.y));
        world.SetTile(furthestPair1, startpoint);
        //Vector3Int fp2Int = new Vector3Int(Mathf.FloorToInt(furthestPair2.x), Mathf.FloorToInt(furthestPair2.y));
        world.SetTile(furthestPair2, endpoint);

        foreach (Vector3Int pos in world.cellBounds.allPositionsWithin)
        {
            if (world.HasTile(pos))
            {
                bool isVisible = false;
                foreach (Vector3Int ps in completedRooms)
                {
                    Debug.Log(Vector3.Distance(world.CellToWorld(ps), world.CellToWorld(pos)));
                    if (Vector3.Distance(world.CellToWorld(ps), world.CellToWorld(pos)) <= discoverRadius)
                    {
                        
                        isVisible = true;
                        break;
                    }
                }
                if(!isVisible)
                    fogOfWar.SetTile(pos, fog);
                else
                    fogOfWar.SetTile(pos, fogVisible);
            }

        }

        StartCoroutine(ExploreTile(center.x,center.y));
        PickSpecialTiles();
        /*Debug.DrawLine(fp1Int + new Vector3(0.5f, -0.5f), fp1Int + new Vector3(-0.5f, 0.5f), Color.green, 10);
        Debug.DrawLine(fp1Int + new Vector3(-0.5f, -0.5f), fp1Int + new Vector3(0.5f, 0.5f), Color.green, 10);
        
        Debug.DrawLine(fp2Int + new Vector3(0.5f, -0.5f), fp2Int + new Vector3(-0.5f, 0.5f), Color.red, 10);
        Debug.DrawLine(fp2Int + new Vector3(-0.5f, -0.5f), fp2Int + new Vector3(0.5f, 0.5f), Color.red, 10);*/

        Debug.DrawLine(gridSize * new Vector3(0,0), gridSize * new Vector3(1, 0), Color.cyan, 30);
        Debug.DrawLine(gridSize * new Vector3(1, 0), gridSize * new Vector3(1, 1), Color.cyan, 30);
        Debug.DrawLine(gridSize * new Vector3(0, 1), gridSize * new Vector3(1, 1), Color.cyan, 30);
        Debug.DrawLine(gridSize * new Vector3(0, 0), gridSize * new Vector3(0, 1), Color.cyan, 30);

        foreach (Vector3Int pos in revealedRooms)
        {
            fogOfWar.SetTile(pos, fogVisible);
        }
        foreach (Vector3Int pos in explorableRooms)
        {
            fogOfWar.SetTile(pos, fogBorder);
        }
        foreach (Vector3Int pos in completedRooms)
        {
            fogOfWar.SetTile(pos, null);
        }
    }

    //creates objects in a grid until it runs out of place attempts;
    List<Vector3Int> GetNeighborsAt(int x, int y)
    {

        List<Vector3Int> neighbors = new List<Vector3Int>();
        List<Vector3Int> sides = new List<Vector3Int>();

        /*        var right = new Vector2( + 1, y);
                var left = new Vector2(x - 1, y);
                var up = new Vector2(x,y+1);
                var down = new Vector2(x, y - 1);*/
        var floatX = x * 0.8659766f;
        var upHex = new Vector3Int(x, y + 1);
        var downHex = new Vector3Int(x, y -1);
        var downLeftHex = new Vector3Int(x + 1, y );
        var upLeftHex = new Vector3Int(x - 1, y);
        var downRightHex = new Vector3Int(x - 1, y + 1);
        var upRightHex = new Vector3Int(x - 1, y - 1);

        var center = new Vector3Int(x, y);

                if (y % 2 == 1)
                {
                    //odd
                    sides.Add(center + new Vector3Int(1, 0));
                    sides.Add(center + new Vector3Int(1, 1));
                    sides.Add(center + new Vector3Int(0, 1));
                    sides.Add(center + new Vector3Int(-1, 0));
                    sides.Add(center + new Vector3Int(0, -1));
                    sides.Add(center + new Vector3Int(1, -1));
                }
                else
                {
                    //EVEN
                    sides.Add(center + new Vector3Int(1, 0));
                    sides.Add(center + new Vector3Int(0, 1));
                    sides.Add(center + new Vector3Int(-1, 1));
                    sides.Add(center + new Vector3Int(-1, 0));
                    sides.Add(center + new Vector3Int(-1, -1));
                    sides.Add(center + new Vector3Int(0, -1));
                }
            
        /*sides.Add(upHex);
        sides.Add(downHex);
        sides.Add(downRightHex);
        sides.Add(downLeftHex);
        sides.Add(upLeftHex);
        sides.Add(upRightHex);*/

/*        if (!placedRooms.Contains(right))
            neighbors.Add(right);
        if (!placedRooms.Contains(left))
            neighbors.Add(left);
        if (!placedRooms.Contains(up))
            neighbors.Add(up);
        if (!placedRooms.Contains(down))
            neighbors.Add(down);*/

        foreach(Vector3Int pos in sides)
        {
                if (pos.x < gridSize.x && pos.x >= 0 && pos.y < gridSize.y && pos.y >= 0)
                {
                    neighbors.Add(pos);
                    
                }
        }
        int guh = 0;
        Color randomColor = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));

        Debug.Log("added " + guh + " new adjacants. open list has " + openList.Count + " entries");
        return neighbors;
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftControl))
        {
            //SceneManager.LoadSceneAsync("Map", LoadSceneMode.Single);
        }
        if (Input.GetKey(KeyCode.Mouse0))
        {
            //ClickToExplore(In);
        }
    }
    IEnumerator ExploreTile(int x, int y)
    {
        var cell = new Vector3Int(x, y);
        //fogOfWar.SetTile(cell, null);
        if (completedRooms.Contains(cell))
        {
            Debug.Log("this rooom has been completed");
            yield return null;
        }
        explorableRooms.Remove(cell);
        completedRooms.Add(cell);
        var adj = GetNeighborsAt(x, y);
        foreach(Vector3Int pos in adj)
        {
            if (world.HasTile(pos))
            {
                if (!completedRooms.Contains(pos))
                    explorableRooms.Add(pos);

                //fogOfWar.SetTile(pos, fogBorder);
            }
        }
        foreach(Vector3Int fogTile in fogOfWar.cellBounds.allPositionsWithin)
        {
            if (fogOfWar.HasTile(fogTile) && !revealedRooms.Contains(fogTile))
            {
                var dist = Vector3.Distance(fogOfWar.CellToWorld(fogTile), fogOfWar.CellToWorld(cell));
                if(dist < discoverRadius)
                {
                    revealedRooms.Add(fogTile);
                }
            }
        }
        foreach (Vector3Int pos in revealedRooms)
        {
            fogOfWar.SetTile(pos, fogVisible);
        }
        foreach (Vector3Int pos in explorableRooms)
        {
                fogOfWar.SetTile(pos, fogBorder);
        }
        foreach (Vector3Int pos in completedRooms)
        {
                fogOfWar.SetTile(pos, null);
        }

        yield return null;
    }
    public void ClickToExplore(Vector3 mouse)
    {
        var mouseCast = Camera.main.ScreenToWorldPoint(mouse);
        mouseCast.z = 0;
        Vector3Int clickedTile = Vector3Int.zero;
        bool clicked =  false;
        foreach(Vector3Int pos in explorableRooms)
        {
            if (fogOfWar.HasTile(pos))
            {
                if (fogOfWar.GetTile(pos) == fogBorder)
                {
                    if(Vector3.Distance(fogOfWar.CellToWorld(pos), mouseCast) < 0.5f)
                    {
                        Debug.DrawLine(fogOfWar.CellToWorld(pos), mouseCast, Color.red, 10);
                        clickedTile = pos;
                        clicked = true;
                        break;
                    }
                }
            }
        }
        if (clicked)
        {
            StartCoroutine(ExploreTile(clickedTile.x, clickedTile.y));
            var cell = clickedTile;
            if (world.GetTile(cell) == romance)
            {
                AmbidexterousManager.Instance.ChangeScene("SampleScene");
            }
            else if (world.GetTile(cell) == shop)
            {
                //AmbidexterousManager.Instance.ChangeScene("SampleScene");
            }
            else
            {
                AmbidexterousManager.Instance.ChangeScene("ConnorTest");
            }
        }
    }

    void PickSpecialTiles()
    {
        Random.InitState(seed);
        var romances = romancesToSpawn;
        var encounters = encountersToSpawn;
        var shops = shopsToSpawn;
        Vector3Int previousPick = Vector3Int.zero;
        Vector3Int center = new Vector3Int(Mathf.FloorToInt(gridSize.x / 2), Mathf.FloorToInt(gridSize.y / 2));
        placedRooms.Remove(center);
        List<Vector3Int> placedSpecialRooms = new List<Vector3Int>();
        placedSpecialRooms.Add(Vector3Int.zero);
        var attempts = 1000;
        while (romances > 0 && placedRooms.Count > 0)
        {
            romances--;
            var pick = placedRooms[Random.Range(0, placedRooms.Count)];
            Debug.Log("romance " + romances +" " + Vector3Int.Distance(pick, previousPick));
            bool placed = true;
            foreach(Vector3Int pos in placedSpecialRooms)
            {
                var test = Vector3Int.Distance(pick, pos);
                if(test > allowedRepeatDistance){}
                else
                {
                    placed = false;
                }
            }
            if (!placed)
                romances++;
            else
            {
                placedRooms.Remove(pick);
                world.SetTile(pick, romance);
                placedSpecialRooms.Add(pick);
                Debug.Log("added " + pick + " to placedSpecialRooms " + placedSpecialRooms.Count);
                
            }
            if (attempts <= 0)
                break;
            attempts--;
        }
        attempts = 1000;
        placedSpecialRooms.Clear();
        placedSpecialRooms.Add(Vector3Int.zero);
        while (encounters > 0 && placedRooms.Count > 0)
        {
            encounters--;
            var pick = placedRooms[Random.Range(0, placedRooms.Count)];
            bool placed = true;
            foreach (Vector3Int pos in placedSpecialRooms)
            {
                var test = Vector3Int.Distance(pick, pos);
                if (test > allowedRepeatDistance) { }
                else
                {
                    placed = false;
                }
            }
            if (!placed)
                encounters++;
            else
            {
                placedRooms.Remove(pick);
                world.SetTile(pick, encounter);
                placedSpecialRooms.Add(pick);
                Debug.Log("added " + pick + " to placedSpecialRooms " + placedSpecialRooms.Count);

            }
            if (attempts <= 0)
                break;
            attempts--;
        }
        attempts = 1000;
        placedSpecialRooms.Clear();
        placedSpecialRooms.Add(Vector3Int.zero);
        while (shops > 0 && placedRooms.Count > 0)
        {
            shops--;
            var pick = placedRooms[Random.Range(0, placedRooms.Count)];
            bool placed = true;
            foreach (Vector3Int pos in placedSpecialRooms)
            {
                var test = Vector3Int.Distance(pick, pos);
                if (test > allowedRepeatDistance) { }
                else
                {
                    placed = false;
                }
            }
            if (!placed)
                shops++;
            else
            {
                placedRooms.Remove(pick);
                world.SetTile(pick, shop);
                placedSpecialRooms.Add(pick);
                Debug.Log("added " + pick + " to placedSpecialRooms " + placedSpecialRooms.Count);

            }
            if (attempts <= 0)
                break;
            attempts--;
        }
    }
    private void OnDestroy()
    {
        var rm = RunManager.instance;
        rm.explorableRooms = explorableRooms;
        rm.completedRooms = completedRooms;
        rm.revealedRooms = revealedRooms;
    }
}
