using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public void Start()
    {
        Manager.onEnter += TeleportObject;
    }

    private void TeleportObject(Vector3 pos)
    {
        transform.position = pos;
    }
}
