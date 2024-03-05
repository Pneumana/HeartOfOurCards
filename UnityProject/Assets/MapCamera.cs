using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCamera : MonoBehaviour
{
    public Camera cam;
    ArbitraryMapGeneratiion map;
    public float cameraPanSpeed = 5;

    public Vector2 mouseClickedPoint;

    // Start is called before the first frame update
    void Start()
    {
        if(cam==null)
            cam = Camera.main;
        map = GameObject.Find("ArbitraryMap").GetComponent<ArbitraryMapGeneratiion>();
        if(RunManager.instance.localMapCamPos != Vector2.zero && cam.orthographicSize != 0)
        {
            cam.transform.position = new Vector3(RunManager.instance.localMapCamPos.x, RunManager.instance.localMapCamPos.y, -10);
            cam.orthographicSize = RunManager.instance.localMapCamZoom;
        }
        cam.orthographicSize = 10f;
        var center = new Vector3(Mathf.FloorToInt(map.gridSize.x / 2), Mathf.FloorToInt(map.gridSize.y / 2));
        cam.transform.position = new Vector3(center.x, center.y, -10);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            cam.orthographicSize -= 0.1f;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            cam.orthographicSize += 0.1f;
        }


        if (Input.GetKey(KeyCode.Mouse0))
        {
            cam.transform.position -= Input.mousePositionDelta * cameraPanSpeed * (cam.orthographicSize / 10f) * Time.deltaTime; 
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CenterCamera();
        }
        if (Input.GetKey(KeyCode.W))
        {
            cam.transform.position += Vector3.up * cameraPanSpeed * (cam.orthographicSize/10f) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            cam.transform.position += Vector3.left * cameraPanSpeed * (cam.orthographicSize / 10f) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            cam.transform.position += Vector3.down * cameraPanSpeed * (cam.orthographicSize / 10f) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            cam.transform.position += Vector3.right * cameraPanSpeed * (cam.orthographicSize / 10f) * Time.deltaTime;
        }

        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 1, 10);
    }

    public void CenterCamera()
    {
        cam.orthographicSize = 10f;
        var center = new Vector3(Mathf.FloorToInt(map.gridSize.x / 2), Mathf.FloorToInt(map.gridSize.y / 2));
        cam.transform.position = new Vector3(center.x, center.y, -10);
    }
    private void OnDestroy()
    {
        RunManager.instance.localMapCamPos = new Vector2(cam.transform.position.x, cam.transform.position.y);
        RunManager.instance.localMapCamZoom = cam.orthographicSize;
    }
}
