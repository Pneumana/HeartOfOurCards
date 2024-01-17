using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArbitraryMapGeneratiion : MonoBehaviour
{
    public int size;
    public Vector2 gridSize = new Vector2(5, 5);
    int placeAttempts;

    public List<Vector3> mapPositions = new List<Vector3>();
    public List<Vector3> placedRooms = new List<Vector3>();

    public List<List<Vector3>> adjacantRooms = new List<List<Vector3>>();
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
                List<Vector3> neighbors = new List<Vector3>();
                if(!placedRooms.Contains(new Vector2(a + 1, b)))
                    neighbors.Add(new Vector2(a + 1, b));
                if (!placedRooms.Contains(new Vector2(a - 1, b)))
                    neighbors.Add(new Vector2(a - 1, b));
                if (!placedRooms.Contains(new Vector2(a, b + 1)))
                    neighbors.Add(new Vector2(a, b + 1));
                if (!placedRooms.Contains(new Vector2(a, b - 1)))
                    neighbors.Add(new Vector2(a, b - 1));

                adjacantRooms.Add(neighbors);
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
                if (!placedRooms.Contains(new Vector2(a + 1, b)))
                    neighbors.Add(new Vector2(a + 1, b));
                if (!placedRooms.Contains(new Vector2(a - 1, b)))
                    neighbors.Add(new Vector2(a - 1, b));
                if (!placedRooms.Contains(new Vector2(a, b + 1)))
                    neighbors.Add(new Vector2(a, b + 1));
                if (!placedRooms.Contains(new Vector2(a, b - 1)))
                    neighbors.Add(new Vector2(a, b - 1));

                adjacantRooms.Add(neighbors);
                //find a random neighbor room
                //add to placed rooms
                //loop trhrough all neighbor lists and remove that position
            }
            placeAttempts--;
        }
        foreach(Vector3 pos in placedRooms)
        {
            Debug.DrawLine(pos + new Vector3(0.5f, -0.5f), pos + new Vector3(-0.5f, 0.5f), Color.green, 10);
            Debug.DrawLine(pos + new Vector3(-0.5f, -0.5f), pos + new Vector3(0.5f, 0.5f), Color.green, 10);
        }
    }

    //creates objects in a grid until it runs out of place attempts;

}
