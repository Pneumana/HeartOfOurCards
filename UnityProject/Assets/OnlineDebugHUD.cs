using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineDebugHUD : MonoBehaviour
{
    public void TryStartGame(string scene)
    {
        RunManager.instance.TryStartGame(scene);
    }
    public void QuitToMenu()
    {
        AmbidexterousManager.Instance.QuitToMenu();
    }

}
