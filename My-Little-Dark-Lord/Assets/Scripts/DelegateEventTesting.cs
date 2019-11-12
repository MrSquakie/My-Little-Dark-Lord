using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelegateEventTesting : MonoBehaviour
{
    public delegate void ClickAction();

    public static event ClickAction onClick;
    public void ButtonClick()
    {
        if (onClick != null)
            onClick();
    }

}
