using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapNodePlacer : MonoBehaviour
{
    public int spawns;


    List<GameObject> openList = new List<GameObject>();
    List<GameObject> closedList = new List<GameObject>();

    //generate a grid of nodes, delete all but one-3 of them


    List<List<GameObject>> arrayList = new List<List<GameObject>>();
    public int gridX;
    public int gridY;

    private void Start()
    {
        for (int x = 0; x < gridX; x++)
        {
            arrayList.Add(new List<GameObject>());
            for (int y = 0; y < gridY; y++)
            {
                var newGo = new GameObject();
                newGo.name = "MapNode";
                newGo.tag = "MapNode";
                newGo.AddComponent<MapNode>();
                //newGo.GetComponent<MapNode>().parentNode = go;
                newGo.transform.position = new Vector3(x * 1.5f,y * 1.5f);
                arrayList[x].Add(newGo);
            }
        }
        for (int x = 1; x < gridX; x++)
        {
            var parentNode = Random.Range(0, gridY);
            arrayList[x - 1][parentNode].transform.position += Vector3.right * 0.75f;
            if (x == 1)
            {
                var mm = GameObject.Find("Mover").GetComponent<MapMover>();
                mm.startPosition = arrayList[x - 1][parentNode];
                mm.currentTarget = arrayList[x - 1][parentNode];
            }
            foreach(GameObject go in arrayList[x - 1])
            {
                if (go != arrayList[x - 1][parentNode])
                {
                    go.GetComponent<MapNode>().convergentNode = arrayList[x - 1][parentNode];
                }
            }
            Debug.Log("picked y " + parentNode + " to be the parent in x " + (x-1));
            for (int y = 0; y < gridY; y++)
            {
                var currentNode = arrayList[x][y];
                currentNode.GetComponent<MapNode>().parentNode = arrayList[x-1][parentNode];
                arrayList[x - 1][parentNode].GetComponent<MapNode>().childNodes.Add(currentNode);
                currentNode.transform.SetParent(arrayList[x - 1][parentNode].transform);
            }
        }
    }

    private void Update()
    {

        /*if (spawns > 0)
        {
            spawns--;
            openList = GameObject.FindGameObjectsWithTag("MapNode").ToList();

            for(int i = 0; i<openList.Count; i++)
            {
                var go = openList[i];
                var mapNode = go.GetComponent<MapNode>();
                if (closedList.Contains(go))
                {
                    continue;
                }
                if(mapNode.childNodes.Count > 2)
                {
                    closedList.Add(go);
                    continue;
                }
                else
                {
                    var newGo = new GameObject();
                    newGo.name = "MapNode";
                    newGo.tag = "MapNode";
                    newGo.AddComponent<MapNode>();
                    newGo.GetComponent<MapNode>().parentNode = go;
                    newGo.transform.position = go.transform.position + Vector3.right + (Vector3.down * mapNode.childNodes.Count);
                    mapNode.childNodes.Add(newGo);
                    break;
                }
            }
        }*/
    }
}
