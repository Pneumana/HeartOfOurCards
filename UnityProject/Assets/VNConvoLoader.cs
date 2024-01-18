using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VNConvoLoader : MonoBehaviour
{
    public TextFieldConversation conversation;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("LateStart", 0f);
    }
    void LateStart()
    {
        DialougeDisplayer.instance.LoadNew(conversation);

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
