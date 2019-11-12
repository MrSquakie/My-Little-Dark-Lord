using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    // Start is called before the first frame update
    public delegate void OnEnter(Vector3 pos);

    public static event OnEnter onEnter;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            onEnter(new Vector3(5, 2, 0));
    }
}
