using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Equipment Info", menuName = "equipment/EquipmentInfo")]
public class EquipmentInfo : ScriptableObject
{
    public ScriptableObject swordInfo;
    public ScriptableObject armorInfo;
    public ScriptableObject consumables;
}