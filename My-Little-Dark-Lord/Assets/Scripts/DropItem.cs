using UnityEngine;
using System.Collections;

public class DropItem : MonoBehaviour
{

    public GameObject CollectableItem;

    public void Drop()
    {

        if (CollectableItem != null)
        {

            Instantiate(CollectableItem, transform.position, transform.rotation);

        }
    }
}