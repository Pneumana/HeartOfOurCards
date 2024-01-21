using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class HexGridMap : MonoBehaviour
{
    Dictionary<Vector3, List<Vector3>> neighbors = new Dictionary<Vector3, List<Vector3>>();

    List<Vector3> travelablePositions = new List<Vector3>();
    List<Vector3> completedRooms = new List<Vector3>();

    public Vector3 startingPosition;

    public Tilemap world;
    public Tilemap movableTiles;

    public Tile movableTile;
    private void Start()
    {
        foreach (Vector3 pos in world.cellBounds.allPositionsWithin)
        {
            if (!neighbors.ContainsKey(pos))
            {
                List<Vector3> neighborList = new List<Vector3>();
                foreach(Vector3 pos1 in world.cellBounds.allPositionsWithin)
                {
                    if (pos1 == pos)
                        continue;
                    Vector3Int posInt = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
                    Vector3Int pos1Int = new Vector3Int(Mathf.FloorToInt(pos1.x), Mathf.FloorToInt(pos1.y));
                    if (Vector3.Distance(world.CellToWorld(posInt), world.CellToWorld(pos1Int)) < 1.2f)
                    {
                        
                        //Debug.DrawLine(world.CellToWorld(posInt), world.CellToWorld(pos1Int), Color.green, 10);

                        neighborList.Add(pos1);
                    }

                }
                neighbors.Add(pos, neighborList);
            }
        }
        //travelablePositions.Add(startingPosition);
        foreach(Vector3 pos in neighbors[startingPosition])
        {
            travelablePositions.Add(pos);
            Vector3Int posInt = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
            Vector3Int pos1Int = new Vector3Int(Mathf.FloorToInt(startingPosition.x), Mathf.FloorToInt(startingPosition.y));
            Debug.DrawLine(world.CellToWorld(posInt), world.CellToWorld(pos1Int), Color.green, 10);
            Debug.Log(Vector3.Distance(pos, startingPosition));
        }
        UpdateMovableTiles();
    }
    void UpdateMovableTiles()
    {
        movableTiles.ClearAllTiles();
        foreach(Vector3 pos in travelablePositions)
        {
            Vector3Int posInt = new Vector3Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));
            movableTiles.SetTile(posInt, movableTile);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            var worldClick = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        }
    }
}
