using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeController : MonoBehaviour
{
    public Animator animator => GetComponent<Animator>();

    public void FadeIn()
    {
        animator.SetBool("Fade", true);
    }
    public void FadeOut()
    {
        animator.SetBool("Fade", false);
    }
}
