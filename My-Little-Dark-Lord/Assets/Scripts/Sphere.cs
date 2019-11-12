using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        DelegateEventTesting.onClick += fall;
    }

    public void fall()
    {
        GetComponent<Rigidbody>().useGravity = true;
    }
}
