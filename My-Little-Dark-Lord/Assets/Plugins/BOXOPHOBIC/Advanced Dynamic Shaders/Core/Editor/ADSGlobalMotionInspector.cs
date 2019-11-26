// Advanced Dynamic Shaders
// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using Boxophobic;

[CanEditMultipleObjects]
[CustomEditor(typeof(ADSGlobalMotion))]
public class ASDGlobalMotionInspector : Editor
{

	//private ADSGlobalMotion targetScript;

    private string[] excludeProps = new string[] { "m_Script"};

    private Color guiColor;
    private string bannerText;
    private string helpURL;

    void OnEnable()
    {

        //targetScript = (ADSGlobalMotion)target;

        bannerText = "ADS Global Motion";
        helpURL = "https://docs.google.com/document/d/1PG_9bb0iiFGoi_yQd8IX0K3sMohuWqyq6_qa4AyVpFw/edit#heading=h.b6n5ylhvvzzk";

        // Check if Light or Dark Unity Skin
        // Set the Banner and Logo Textures
        if (EditorGUIUtility.isProSkin) 
		{
            guiColor = new Color(1f, 0.754f, 0.186f);
        } 
		else 
		{
            guiColor = BConst.ColorDarkGray;
		}

	}

	public override void OnInspectorGUI()
    {

        BEditorGUI.DrawBanner(guiColor, bannerText, helpURL);        
        DrawInspector();
        //DrawWarnings ();
        BEditorGUI.DrawLogo();

	}

	void DrawInspector()
    {

		serializedObject.Update ();

        excludeProps = new string[] { "m_Script"};

        DrawPropertiesExcluding(serializedObject, excludeProps);

        serializedObject.ApplyModifiedProperties ();

		GUILayout.Space (20);

	} 

//	void DrawWarnings(){
//
//	}
}
