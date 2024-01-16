using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode : MonoBehaviour
{
    public List<GameObject> childNodes = new List<GameObject>();
    public GameObject parentNode;
    public GameObject convergentNode;
    public enum NodeType
    {
        Combat,
        Store,
        Event
    }

    public NodeType nodeType;

    public void OnDrawGizmos()
    {
        if(parentNode == null)
            return;
        Debug.DrawLine(transform.position, parentNode.transform.position, Color.cyan, Time.deltaTime);
        if(convergentNode!=null)
            Debug.DrawLine(transform.position, convergentNode.transform.position, Color.magenta, Time.deltaTime);
        if (childNodes.Count == 0)
            return;
        foreach(GameObject go in childNodes)
        {
            Debug.DrawLine(transform.position, go.transform.position, Color.blue, Time.deltaTime);
        }
        
    }
}
