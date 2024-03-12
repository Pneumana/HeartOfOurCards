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
            //activated = "";
            
            RunManager.instance.TryStartGame(activated);
        }
    }
    public void QuitToMenu()
    {
        AmbidexterousManager.Instance.QuitToMenu();
    }
    public void StartNewSeason()
    {
        AmbidexterousManager.Instance.ServerChangeSeed(Random.Range(int.MinValue, int.MaxValue));
        RunManager.instance.completedRooms.Clear();
        RunManager.instance.explorableRooms.Clear();
        RunManager.instance.revealedRooms.Clear();
        RunManager.instance.TryStartGame("ConnorTest");
    }
}
