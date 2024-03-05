using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineDebugHUD : MonoBehaviour
{
    string activated = "";
    public void TryStartGame(string scene)
    {
        activated = scene;
        //RunManager.instance.TryStartGame(scene);
    }
    private void Update()
    {
        if (activated!="")
        {
            RunManager.instance.TryStartGame(activated);
        }
    }
    public void QuitToMenu()
    {
        AmbidexterousManager.Instance.QuitToMenu();
    }

}
