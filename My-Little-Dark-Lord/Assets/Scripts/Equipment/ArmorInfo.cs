using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ArmorInfo : ScriptableObject
{
    public float armorRating;
    public AudioClip hitSoundEffect;
    public GameObject mesh;
}
