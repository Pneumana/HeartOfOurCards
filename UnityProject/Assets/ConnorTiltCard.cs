using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnorTiltCard : MonoBehaviour
{
    public float constraints;
    public float speed;
    float x;
    float z;
    public Vector3 offset;

    Ray ray;
    RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        offset = transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 input= Vector2.zero;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            input.x = 1;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            input.x = -1;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            input.z = 1;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            input.z = -1;
        }
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                var diff = new Vector3(hit.point.z, 0, hit.point.x) - new Vector3(transform.position.z, 0, transform.position.x);
                //new Vector3(hit.point.z, 0, hit.point.x) - new Vector3(transform.position.z, 0, transform.position.x);
                input = new Vector3(1 * diff.x,1,-1 * diff.z);
                //Debug.Log(diff + "=>" + diff.normalized);
            }
        }
        if (input != Vector3.zero)
        {
            var move = Vector3.MoveTowards(transform.localEulerAngles, transform.localEulerAngles + (input * constraints), Time.deltaTime * speed);
            x = Mathf.MoveTowards(x, input.x * constraints, Time.deltaTime * speed);
            z = Mathf.MoveTowards(z, input.z * constraints, Time.deltaTime * speed);
        }
        else
        {
            x = Mathf.MoveTowards(x, 0, Time.deltaTime * speed);
            z = Mathf.MoveTowards(z, 0, Time.deltaTime * speed);
        }
        transform.rotation = Quaternion.Euler(new Vector3(x + offset.x, 0+offset.y, z + offset.z));


    }
}
