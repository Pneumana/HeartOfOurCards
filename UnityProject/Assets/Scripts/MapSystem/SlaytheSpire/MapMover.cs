using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMover : MonoBehaviour
{
    public GameObject currentTarget;
    public GameObject startPosition;
    public float speed;

    private void Start()
    {
        
    }

    private void Update()
    {
        var newPos = Vector3.MoveTowards(transform.position, currentTarget.transform.position, speed * Time.deltaTime);
        transform.position = newPos;
        if(Vector3.Distance(transform.position, currentTarget.transform.position) <0.05f)
        {
            //do that thang
            GameObject result;
            var targetPN = currentTarget.GetComponent<MapNode>();
            if (targetPN.childNodes.Count == 0)
            {
                if (targetPN.convergentNode!=null)
                {
                    Debug.Log("convergent node");
                    result = targetPN.convergentNode;
                }
                else
                {
                    Debug.Log("start node");
                    result = startPosition;
                    transform.position = result.transform.position;
                }
            }
            else
            {
                Debug.Log("parent node");
                result = targetPN.childNodes[Random.Range(0,targetPN.childNodes.Count-1)];
            }
            if (result != null)
                currentTarget = result;
        }
    }
}
