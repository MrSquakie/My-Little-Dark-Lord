using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomIdle : MonoBehaviour
{
    public Animator animator => GetComponent<Animator>();

    public IEnumerator Start()
    {
        Debug.Log("Yo");
        animator.SetInteger("randomAnimation", Random.Range(0,3));
        yield return new WaitForSeconds(2);
    }
}
