// Advanced Dynamic Shaders
// Cristian Pop - https://boxophobic.com/

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Boxophobic;

[HelpURL("https://docs.google.com/document/d/1PG_9bb0iiFGoi_yQd8IX0K3sMohuWqyq6_qa4AyVpFw/edit#heading=h.d4azvp42k3l7")]
[DisallowMultipleComponent]
[ExecuteInEditMode]
public class ADSGlobalDebug : MonoBehaviour
{
    public enum DebugModeEnum
    {
        Off = -1,
        DebugADS = 0,
        DebugMesh = 1,
    };

    public enum DebugADSEnum
    {
        //Off = -1,

        MotionMask1 = 11,
        MotionMask2 = 12,
        MotionMask3 = 13,
        MotionVariation = 14,
        MotionTurbulence = 15,

        GlobalTint = 31,
        GlobalSize = 32,

        MaterialLighting = 61,
        MaterialInstancing = 62,
        //shaderType = 63,
        //MaterialIssues = 64,
    };

    public enum DebugMeshEnum
    {
        //Off = -1,

        VertexColorR = 101,
        VertexColorG = 102,
        VertexColorB = 103,
        VertexColorA = 104,

        VertexPositionX = 105,
        VertexPositionY = 106,
        VertexPositionZ = 107,

        UVCoordZeroX = 111,
        UVCoordZeroY = 112,
        UVCoordZeroZ = 113,
        UVCoordZeroW = 114,

        UVCoordTwoX = 121,
        UVCoordTwoY = 122,
        UVCoordTwoZ = 123,
        UVCoordTwoW = 124,

        UVCoordThreeX = 131,
        UVCoordThreeY = 132,
        UVCoordThreeZ = 133,
        UVCoordThreeW = 134,

        UVCoordFourX = 141,
        UVCoordFourY = 142,
        UVCoordFourZ = 143,
        UVCoordFourW = 144,
    };

    [BCategory("Debug")]
    public int category_Debug;

    [Space(10)]
    public DebugModeEnum debugMode = DebugModeEnum.Off;

    [BInteractive(1)]
    public int interactiveDebugADS;
    public DebugADSEnum debugADS = DebugADSEnum.MotionMask1;

    [BInteractive(1)]
    public int interactiveDebugMesh;
    public DebugMeshEnum debugMesh = DebugMeshEnum.VertexColorR;

    [BCategory("Remap")]
    public int category_Remap;

    [BInteractive(1)]
    public int interactiveMin;
    //[Space(10)]
    public float remapMin = 0.0f;

    [BInteractive(1)]
    public int interactiveMax;
    public float remapMax = 1.0f;

    [BInteractive("ON")]
    public int interactiveReset;    

    private Shader debugShader;
    private bool debugShader_ON = false;
    private int debugValue = -1;

    void Start()
    {
        debugShader = Shader.Find("Utils/ADS Debug");
        gameObject.name = "ADS Global Debug";

        remapMin = 0.0f;
        remapMax = 1.0f;
    }

    void Update()
    {

        if (Application.isPlaying)
        {
            return;
        }

        if (SceneView.lastActiveSceneView != null)
        {
            if (debugMode == DebugModeEnum.Off)
            {
                if (debugShader_ON == true)
                {
                    SceneView.lastActiveSceneView.SetSceneViewShaderReplace(null, null);
                    SceneView.lastActiveSceneView.Repaint();

                    debugShader_ON = false;
                }

                interactiveDebugADS = 0;
                interactiveDebugMesh = 0;
                interactiveMin = 0;
                interactiveMax = 0;
            }
            else
            {
                if (debugMode == DebugModeEnum.DebugADS)
                {
                    debugValue = (int)debugADS;

                    interactiveDebugADS = 1;
                    interactiveDebugMesh = 0;
                    interactiveMin = 1;
                    interactiveMax = 1;
                }
                else if (debugMode == DebugModeEnum.DebugMesh)
                {
                    debugValue = (int)debugMesh;

                    interactiveDebugADS = 0;
                    interactiveDebugMesh = 1;
                    interactiveMin = 1;
                    interactiveMax = 1;
                }

                SceneView.lastActiveSceneView.SetSceneViewShaderReplace(debugShader, null);
                SceneView.lastActiveSceneView.Repaint();

                debugShader_ON = true;

                Shader.SetGlobalFloat("ADS_DebugMode", debugValue);
                Shader.SetGlobalFloat("ADS_DebugMin", remapMin);
                Shader.SetGlobalFloat("ADS_DebugMax", remapMax);
            }
        }

    }
}
#endif

