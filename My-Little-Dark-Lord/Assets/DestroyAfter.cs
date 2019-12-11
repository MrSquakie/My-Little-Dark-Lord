using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfter : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(DestroyEnumerator());
        }
    }

    public IEnumerator DestroyEnumerator()
    {
        yield return new WaitForSeconds(2);
        print("Deleted collision");
        Destroy(this);
    }
}
