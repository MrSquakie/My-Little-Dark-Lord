using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoading : MonoBehaviour
{
    [SerializeField]
    private Image _progressBar;
    [SerializeField]
    private int _levelToLoad;
    // Start is called before the first frame update
    void Start()
    {
        //start async operation
        StartCoroutine(LoadAsyncOperation());
    }

    IEnumerator LoadAsyncOperation()
    {
        //create async op
        
        AsyncOperation gameLevel = SceneManager.LoadSceneAsync(_levelToLoad);
        //take progress bar fill = async operation progress.
        while (gameLevel.progress < 1)
        {
            print("Progressing: " + gameLevel.progress);
            _progressBar.fillAmount = gameLevel.progress;
            yield return new WaitForEndOfFrame();
        }
        //when finished load the game scene
    }
}
