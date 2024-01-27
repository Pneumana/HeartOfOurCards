using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
//using System.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

public class ArbitraryMapGeneratiion : MonoBehaviour
{
    public int size;
    public Vector2 gridSize = new Vector2(5, 5);
    int placeAttempts;

    public List<Vector3Int> mapPositions = new List<Vector3Int>();
    public List<Vector3Int> placedRooms = new List<Vector3Int>();

    //available neighboring rooms
    public List<Vector3Int> openList = new List<Vector3Int>();


    public List<List<Vector3Int>> adjacantRooms = new List<List<Vector3Int>>();

    public Sprite icon;

    public Tilemap world;
    public Tile tile;
    public Tile endpoint;
    public Tile startpoint;
    //need a list of adjacant positions for each placed room, excluding positions that are already used
    public List<Vector3Int> firstadjacents = new List<Vector3Int>();
    private void Start()
    {
        for(int x = 0; x< gridSize.x; x++)
        {
            for(int y = 0; y < gridSize.y; y++)
            {
                mapPositions.Add(new Vector3Int(x, y));
            }
        }
        placeAttempts = size;
        Vector3Int center = new Vector3Int(Mathf.FloorToInt(gridSize.x / 2), Mathf.FloorToInt(gridSize.y / 2));
        AddNeighborsAt(center.x,center.y);
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
            AddNeighborsAt((int)a,(int)b);
            var worldPos = world.CellToWorld(pick);
            Debug.DrawLine(worldPos + new Vector3(0.5f, -0.5f), worldPos + new Vector3(-0.5f, 0.5f), Color.red, 5);
            Debug.DrawLine(worldPos + new Vector3(-0.5f, -0.5f), worldPos + new Vector3(0.5f, 0.5f), Color.red, 5);
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
        /*Debug.DrawLine(fp1Int + new Vector3(0.5f, -0.5f), fp1Int + new Vector3(-0.5f, 0.5f), Color.green, 10);
        Debug.DrawLine(fp1Int + new Vector3(-0.5f, -0.5f), fp1Int + new Vector3(0.5f, 0.5f), Color.green, 10);

        Debug.DrawLine(fp2Int + new Vector3(0.5f, -0.5f), fp2Int + new Vector3(-0.5f, 0.5f), Color.red, 10);
        Debug.DrawLine(fp2Int + new Vector3(-0.5f, -0.5f), fp2Int + new Vector3(0.5f, 0.5f), Color.red, 10);*/

        Debug.DrawLine(gridSize * new Vector3(0,0), gridSize * new Vector3(1, 0), Color.cyan, 30);
        Debug.DrawLine(gridSize * new Vector3(1, 0), gridSize * new Vector3(1, 1), Color.cyan, 30);
        Debug.DrawLine(gridSize * new Vector3(0, 1), gridSize * new Vector3(1, 1), Color.cyan, 30);
        Debug.DrawLine(gridSize * new Vector3(0, 0), gridSize * new Vector3(0, 1), Color.cyan, 30);
    }

    //creates objects in a grid until it runs out of place attempts;
    void AddNeighborsAt(int x, int y)
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

        if(y % 2 == 1)
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
        foreach(Vector3Int pos in neighbors)
        {
            Debug.DrawLine(world.CellToWorld(pos), world.CellToWorld(center), Color.cyan, 15);
            if (!placedRooms.Contains(pos))
            {
                openList.Add(pos);
                guh++;
            }

        }
        Debug.Log("added " + guh + " new adjacants. open list has " + openList.Count + " entries");
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftControl))
        {
            SceneManager.LoadSceneAsync("Map", LoadSceneMode.Single);
        }
    }
}
