// Advanced Dynamic Shaders
// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using Boxophobic;

[CanEditMultipleObjects]
[CustomEditor(typeof(ADSMotionHelper))]
public class ADSMotionHelperInspector : Editor 
{
	private ADSMotionHelper targetScript;

    private static readonly string excludeScript = "m_Script";

    private Color bannerColor;
    private string bannerText;
    private string helpURL;

	void OnEnable()
    {		
		targetScript = (ADSMotionHelper)target;

        bannerText = "ADS Motion Helper";
        helpURL = "https://docs.google.com/document/d/1PG_9bb0iiFGoi_yQd8IX0K3sMohuWqyq6_qa4AyVpFw/edit#heading=h.19cm1zdplgma";

        // Check if Light or Dark Unity Skin
        // Set the Banner and Logo Textures
        if (EditorGUIUtility.isProSkin) 
		{
            bannerColor = BConst.ColorLightGray;
        } 
		else 
		{
            bannerColor = BConst.ColorDarkGray;
        }
    }

	public override void OnInspectorGUI()
    {
        BEditorGUI.DrawBanner(bannerColor, bannerText, helpURL);
        DrawRefresh();
        DrawInspector ();
        DrawDone();
    }

    void DrawInspector()
    {
        serializedObject.Update();

        DrawPropertiesExcluding(serializedObject, excludeScript);

        serializedObject.ApplyModifiedProperties();
    }

    void DrawRefresh()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("");

        if (GUILayout.Button("Refresh"))
        {
            targetScript.refreshed1 = 1;
            targetScript.refreshed2 = 1;
            targetScript.refreshed3 = 1;

            targetScript.update = true;

            targetScript.GetAllMaterials();
            targetScript.GetMaterialProperties();
        }

        GUILayout.Label("");
        GUILayout.EndHorizontal();
    }

    void DrawDone()
    {
        GUILayout.Space(10);

        GUILayout.BeginHorizontal();

        GUILayout.Label("");
        if (GUILayout.Button("Done"))
        {            
            targetScript.DestroyComponent();
            GUIUtility.ExitGUI();
        }
        GUILayout.Label("");

        GUILayout.EndHorizontal();

        GUILayout.Space(10);
    }

    //   void DrawWarnings()
    //   {

    //	if (targetScript.warningMissingADSMesh == true) 
    //	{
    //		EditorGUILayout.HelpBox ("The gameobject should have valid MeshFilter component with a Mesh attached!", MessageType.Warning, true);
    //		GUILayout.Space (20);
    //	}
    //}
}
