using UnityEngine;

[CreateAssetMenu(fileName = "New Armor Info", menuName = "equipment/ArmorInfo")]
public class ArmorInfo : ScriptableObject
{
    public float armorRating;
    public AudioClip hitSoundEffect;
    public GameObject mesh;
}