using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnorTiltCard : MonoBehaviour
{
    public float constraints;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        
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
        if (input != Vector3.zero)
        {
            var move = Vector3.MoveTowards(transform.localEulerAngles, transform.localEulerAngles + (input * constraints), Time.deltaTime * speed);
            transform.rotation = Quaternion.Euler(move);
        }
        else
        {
/*            //
            var move = Vector3.MoveTowards(transform.localEulerAngles, Vector3.zero, Time.deltaTime * speed);
            transform.rotation = Quaternion.Euler(move);*/
        }
    }
}
