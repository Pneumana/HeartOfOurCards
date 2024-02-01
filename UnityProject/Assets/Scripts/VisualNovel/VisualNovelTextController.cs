using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
public class VisualNovelTextController : NetworkBehaviour
{
    float skipCD;
    public VNSyncer VNSyncer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(skipCD>=0)
            skipCD-=Time.deltaTime;
        //inputs being space/click -> next
        //hold ctrl to spam advanceToNext/skip the CHO command should stop this
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKey(KeyCode.LeftControl))
        {
            AdvanceToNext();
        }
/*        if(Input.mouseScrollDelta.y > 0)
        {
            GoBack();
        }*/
    }
    public void AdvanceToNext()
    {
        if (skipCD < 0)
        {
            var VNSyncer = GameObject.Find("VNSyncer").GetComponent<VNSyncer>();
            Debug.Log("clicked to progress");
            VNSyncer.CMDProgressDialouge();
            /*var dd = GameObject.Find("DialougeDisplayer").GetComponent<DialougeDisplayer>();
            if (!dd.displayingChoice)
                dd.SkipTo(-1, false);*/
            skipCD = 0.1f;
        }

    }
    void GoBack()
    {
        
        if (skipCD < 0)
        {
            var dd = GameObject.Find("DialougeDisplayer").GetComponent<DialougeDisplayer>();
            dd.GobackOne();
            skipCD = 0.1f;
        }

    }
}
