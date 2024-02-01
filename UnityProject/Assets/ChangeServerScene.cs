using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeServerScene : MonoBehaviour
{
    public void Clicked(string scene)
    {
        AmbidexterousManager.Instance.ServerChangeScene(scene);
    }
}
