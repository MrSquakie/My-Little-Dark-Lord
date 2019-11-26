// Advanced Dynamic Shaders
// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "ADS Shader Features", menuName = "Boxophbic/ADS Shader Features", order = 100)]
public class ADSShaderFeatures : ScriptableObject
{
    public Shader ADSShader = null;

    public int compatibility = 0;
    public int LODFade = 0;

    //public bool globalTint = true;
    //public bool globalScale = true;
    //public bool globalLeavesAmount = true;
    //public bool globalOverlay = false;
}
