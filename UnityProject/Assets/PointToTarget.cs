using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public class PointToTarget : MonoBehaviour
{
    public int length = 5;
    LineRenderer line;
    public Vector3 endpoint;
    public Vector3 cornerpoint;
    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = length;
        var targetDir = endpoint - transform.position;
        Debug.Log("targeting " + targetDir);
    }

    // Update is called once per frame
    void Update()
    {
        /*Vector3 dir = Vector3.zero;
        Vector3 worldposition = transform.position;

        var targetAngle = Vector3.Angle(transform.position, transform.position + (endpoint - transform.position));
        float prevangle = Vector3.Angle(transform.position, transform.forward);

        var lastDir = transform.forward;

        for (int i = 0; i<length; i++)
        {
            float lerpTime = 0;
            if (i != 0)
            {
                lerpTime = 0.5f / i;
                prevangle = Vector3.Angle(transform.position + worldposition, lastDir);
            }
            var angleDiff = targetAngle - prevangle;

            var lerp = lastDir* angleDiff;
            lastDir = lerp;
            worldposition += lerp;
            line.SetPosition(i,lerp);
        }*/

        Ray ray;
        RaycastHit hit;
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.GetComponent<CardEnemyController>()!=null)
                {
                    endpoint = hit.collider.gameObject.transform.position;
                }
            }
        }
        cornerpoint = transform.forward * 2;
            DrawQuadraticBezierCurve(transform.position, cornerpoint, endpoint);
    }
    void DrawQuadraticBezierCurve(Vector3 point0, Vector3 point1, Vector3 point2)
    {
        line.positionCount = length;
        float t = 0f;
        Vector3 B = new Vector3(0, 0, 0);
        for (int i = 0; i < line.positionCount; i++)
        {
            B = (1 - t) * (1 - t) * point0 + 2 * (1 - t) * t * point1 + t * t * point2;
            line.SetPosition(i, B);
            t += (1 / (float)line.positionCount);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position, 0.1f);
        Gizmos.DrawSphere(cornerpoint, 0.1f);
        Gizmos.DrawSphere(endpoint, 0.1f);
    }
    public void Hide()
    {
        GetComponent<LineRenderer>().enabled = false;
    }
    public void Display()
    {
        GetComponent<LineRenderer>().enabled = true;
    }
}
