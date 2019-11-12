using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelegateTesting : MonoBehaviour
{
    public delegate void ChangeColor(Color newColor);

    public ChangeColor onColorChange;

    public delegate void OnComplete();

    public OnComplete onComplete;


 

    public void Start()
    {
        onColorChange = UpdateColor;
        onColorChange(Color.green);

        onComplete += Task;
        onComplete += Task2;
        onComplete += Task3;

        onComplete -= Task2;
        
        if (onComplete != null)
            onComplete();   
    }

    public void UpdateColor(Color newColor)
    {
        Debug.Log("Changing color to: " + newColor.ToString());
    }

    public void Task()
    {
        Debug.Log("Task1 Finished");
    }

    public void Task2()
    {
        Debug.Log("Task2 Finished");

    }

    public void Task3()
    {
        Debug.Log("Task3 Finished");

    }

}
