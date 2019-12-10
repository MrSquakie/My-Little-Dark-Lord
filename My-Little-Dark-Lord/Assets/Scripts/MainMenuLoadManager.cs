using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuLoadManager : MonoBehaviour
{
    public int _levelToLoad;
    public void LoadScene()
    {
        SceneManager.LoadScene(_levelToLoad);
    }
}
