using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestButton : MonoBehaviour
{
    public void Clicked(string convoOverride)
    {
        RunManager.instance.VNConvoOverride(convoOverride);
        RunManager.instance.TryStartGame("SampleScene");
    }
}
