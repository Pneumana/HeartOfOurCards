using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScrollMover : MonoBehaviour
{
    bool cheated;
    Vector3 uppos;
    // Start is called before the first frame update
    void Start()
    {
        uppos = transform.position + transform.up * 6.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse2) && !cheated)
        {
            Debug.LogWarning("Cheating to skip combat");
            transform.position = uppos;
            cheated = true;
            RunManager.instance.CMDChangeSharedStat("Stamina", 1);
        }
    }
}
