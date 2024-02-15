using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowProjectedMapBounds : MonoBehaviour
{
    [SerializeField]ArbitraryMapGeneratiion map;
    private void OnDrawGizmosSelected()
    {
        if (map == null)
            return;
        Debug.DrawLine(transform.position + transform.up * map.gridSize.y, transform.position, Color.cyan, Time.deltaTime);
        Debug.DrawLine(transform.position + transform.up * map.gridSize.y, transform.position + transform.up * map.gridSize.y + transform.right * map.gridSize.x, Color.cyan, Time.deltaTime);
        Debug.DrawLine(transform.position + transform.right * map.gridSize.x, transform.position, Color.cyan, Time.deltaTime);
        Debug.DrawLine(transform.position + transform.right * map.gridSize.x, transform.position + transform.up * map.gridSize.y + transform.right * map.gridSize.x, Color.cyan, Time.deltaTime);
    }
}
