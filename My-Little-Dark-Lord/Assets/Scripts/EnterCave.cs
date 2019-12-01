using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterCave : MonoBehaviour
{

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Player") || collision.gameObject.layer.Equals("Player"))
        {
            SceneManager.LoadScene("Dungeon");
        }
    }
}
