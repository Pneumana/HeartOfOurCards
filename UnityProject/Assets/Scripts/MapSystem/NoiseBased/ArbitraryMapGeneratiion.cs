using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class ArbitraryMapGeneratiion : MonoBehaviour
{
    public int size;
    public Vector2 gridSize = new Vector2(5, 5);
    int placeAttempts;

    public List<Vector3> mapPositions = new List<Vector3>();
    public List<Vector3> placedRooms = new List<Vector3>();

    public List<List<Vector3>> adjacantRooms = new List<List<Vector3>>();

    public Sprite icon;
    //need a list of adjacant positions for each placed room, excluding positions that are already used

    private void Start()
    {
        for(int x = 0; x< gridSize.x; x++)
        {
            for(int y = 0; y < gridSize.y; y++)
            {
                mapPositions.Add(transform.position+ new Vector3(x, y));
            }
        }
        placeAttempts = size;
        while(placeAttempts > 0)
        {

            //create room
            if (placeAttempts == size)
            {
                //pick a random node to start on
                int a = (int)Random.Range(0, gridSize.x-1);
                int b = (int)Random.Range(0, gridSize.y - 1);
                mapPositions.Remove(new Vector2(a,b));
                placedRooms.Add(new Vector2(a, b));
                /*List<Vector3> neighbors = new List<Vector3>();
                if(!placedRooms.Contains(new Vector2(a + 1, b)))
                    neighbors.Add(new Vector2(a + 1, b));
                if (!placedRooms.Contains(new Vector2(a - 1, b)))
                    neighbors.Add(new Vector2(a - 1, b));
                if (!placedRooms.Contains(new Vector2(a, b + 1)))
                    neighbors.Add(new Vector2(a, b + 1));
                if (!placedRooms.Contains(new Vector2(a, b - 1)))
                    neighbors.Add(new Vector2(a, b - 1));*/

                adjacantRooms.Add(CheckValidPosition(a,b));
            }
            else
            {
                List<Vector3> pickedList = adjacantRooms[Random.Range(0, adjacantRooms.Count-1)];
                if (pickedList.Count == 0)
                    continue;
                var pick = pickedList[Random.Range(0, pickedList.Count - 1)];

                placedRooms.Add(pick);
                foreach(List<Vector3> list in adjacantRooms)
                {
                    if (!list.Contains(pick))
                        continue;
                    list.Remove(pick);
                }
                List<Vector3> neighbors = new List<Vector3>();
                var a = pick.x;
                var b = pick.y;

                adjacantRooms.Add(CheckValidPosition((int)a,(int)b));
                //find a random neighbor room
                //add to placed rooms
                //loop trhrough all neighbor lists and remove that position
            }
            placeAttempts--;
        }
        Vector3 furthestPair1 = Vector3.zero;
        Vector3 furthestPair2 = Vector3.zero;
        float dist = 0;
        foreach(Vector3 pos in placedRooms)
        {
            /*Debug.DrawLine(pos + new Vector3(0.5f, -0.5f), pos + new Vector3(-0.5f, 0.5f), Color.green, 10);
            Debug.DrawLine(pos + new Vector3(-0.5f, -0.5f), pos + new Vector3(0.5f, 0.5f), Color.green, 10);*/
            var tile = new GameObject();
            tile.name = "MapTile";
            tile.transform.position = pos;
            tile.AddComponent<SpriteRenderer>();
            tile.GetComponent<SpriteRenderer>().sprite = icon;
            foreach(Vector3 ps in placedRooms)
            {
                if (ps != pos)
                {
                    var measure = Vector3.Distance(pos, ps);
                    if (measure > dist)
                    {
                        dist = measure;
                        furthestPair1 = ps;
                        furthestPair2 = pos;
                    }
                }
            }
        }
        Debug.DrawLine(furthestPair1 + new Vector3(0.5f, -0.5f), furthestPair1 + new Vector3(-0.5f, 0.5f), Color.green, 10);
        Debug.DrawLine(furthestPair1 + new Vector3(-0.5f, -0.5f), furthestPair1 + new Vector3(0.5f, 0.5f), Color.green, 10);

        Debug.DrawLine(furthestPair2 + new Vector3(0.5f, -0.5f), furthestPair2 + new Vector3(-0.5f, 0.5f), Color.red, 10);
        Debug.DrawLine(furthestPair2 + new Vector3(-0.5f, -0.5f), furthestPair2 + new Vector3(0.5f, 0.5f), Color.red, 10);

        Debug.DrawLine(gridSize * new Vector3(0,0), gridSize * new Vector3(1, 0), Color.cyan, 30);
        Debug.DrawLine(gridSize * new Vector3(1, 0), gridSize * new Vector3(1, 1), Color.cyan, 30);
        Debug.DrawLine(gridSize * new Vector3(0, 1), gridSize * new Vector3(1, 1), Color.cyan, 30);
        Debug.DrawLine(gridSize * new Vector3(0, 0), gridSize * new Vector3(0, 1), Color.cyan, 30);
    }

    //creates objects in a grid until it runs out of place attempts;
    List<Vector3> CheckValidPosition(int x, int y)
    {
        List<Vector3> neighbors = new List<Vector3>();
        List<Vector3> sides = new List<Vector3>();

        var right = new Vector2(x + 1, y);
        var left = new Vector2(x - 1, y);
        var up = new Vector2(x,y+1);
        var down = new Vector2(x, y - 1);
        sides.Add(right);
        sides.Add(up);
        sides.Add(left);
        sides.Add(down);

/*        if (!placedRooms.Contains(right))
            neighbors.Add(right);
        if (!placedRooms.Contains(left))
            neighbors.Add(left);
        if (!placedRooms.Contains(up))
            neighbors.Add(up);
        if (!placedRooms.Contains(down))
            neighbors.Add(down);*/

        foreach(Vector3 pos in sides)
        {
            if (!placedRooms.Contains(pos))
            {
                if (pos.x < gridSize.x && pos.x >= 0 && pos.y < gridSize.y && pos.y >= 0)
                {
                    neighbors.Add(pos);
                }
            }
        }

        return neighbors;
    }
}
