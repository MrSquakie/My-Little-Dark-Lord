using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordInfo : ScriptableObject
{
    public float damageRating;
    public GameObject mesh;
    public AudioClip[] slashNoises;
    public AudioClip[] hitNoises;
}
