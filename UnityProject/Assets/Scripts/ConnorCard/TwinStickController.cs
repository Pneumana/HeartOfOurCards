using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class TwinStickController : MonoBehaviour
{
    public float speed;
    public Vector3 lookPosition;

    public GameObject cube;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var hIn = Input.GetAxis("Horizontal");
        var vIn = Input.GetAxis("Vertical");

        var mouseX = Input.GetAxis("Mouse X");
        var mouseY = Input.GetAxis("Mouse Y");

/*        if (Gamepad.current.rightStickButton.isPressed)
        {
            Debug.Log("right stick pressed");
        }*/
        /*if(mouseX==0)
            mouseX = Gamepad.current.rightStick.value.x;
        if(mouseY==0)
            mouseY = Gamepad.current.rightStick.value.y;*/
        /*        var ctrlX = Input.GetAxis("HorizontalCTRL");
                var ctrlY = Input.GetAxis("VerticalCTRL");*/

        //var lookIn = Vector2.Max(new Vector2(mouseX, mouseY), new Vector2(ctrlX, ctrlY));

        var lookIn = new Vector2(mouseX, mouseY);

        transform.position += new Vector3 (hIn, 0, vIn) * speed * Time.deltaTime;
        //Debug.Log("mousex=" + lookIn + ", mousey=" + lookIn);
        cube.transform.position += new Vector3(mouseX, 0, mouseY) * speed * Time.deltaTime;


        Vector3 fwd = cube.transform.position - transform.position;
        if (mouseX != 0 && mouseY != 0)
        {
            fwd = new Vector3(mouseX, 0, mouseY);
        }

        transform.forward = fwd;

    }
}
