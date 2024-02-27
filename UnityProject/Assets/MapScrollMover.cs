using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScrollMover : MonoBehaviour
{
    bool cheated;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse2) && !cheated)
        {
            Debug.LogWarning("Cheating to skip combat");
            transform.position += transform.up * 6.5f;
            cheated = true;
        }
    }
}
