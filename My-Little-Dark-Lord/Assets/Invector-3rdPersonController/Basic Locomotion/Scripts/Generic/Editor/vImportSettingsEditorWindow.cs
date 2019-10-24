using UnityEditor;
using UnityEngine;

public class vImportSettingsEditorWindow : EditorWindow
{
    public bool show;
    public static vImportSettingsEditorWindow instance;
    public const string vSettingsShowAgainKey = "SHOWVIMPORTSETTINGS";
    public const string vSettingsResourcesPath = "Assets/Invector-3rdPersonController/Basic Locomotion/Resources/vProjectSettings.unitypackage";

    GUISkin skin;
    Vector2 rect = new Vector2(380, 240);

    [InitializeOnLoadMethod]
    public static void InitWindow()
    {
        if (!EditorPrefs.HasKey(vSettingsShowAgainKey) || !EditorPrefs.GetBool(vSettingsShowAgainKey))
        {
            if (!IsOpen())
            {
                instance = EditorWindow.GetWindow<vImportSettingsEditorWindow>();
                instance.Show();
            }
        }
    }

    [MenuItem("Invector/Import ProjectSettings")]
    public static void InitWindow2()
    {
        if (!IsOpen())
        {
            instance = EditorWindow.GetWindow<vImportSettingsEditorWindow>();
            instance.show = EditorPrefs.GetBool(vSettingsShowAgainKey);
            instance.Show();
        }
    }

    public static bool IsOpen()
    {
        vImportSettingsEditorWindow[] windows = Resources.FindObjectsOfTypeAll<vImportSettingsEditorWindow>();
        if (windows != null && windows.Length > 0)
        {
            return true;
        }
        return false;
    }

    public void OnGUI()
    {
        this.titleContent = new GUIContent("First Run");
        this.minSize = rect;
        this.maxSize = rect;
        if (!skin) skin = Resources.Load("skin") as GUISkin;
        GUI.skin = skin;

        GUILayout.BeginVertical("window");

        GUILayout.BeginHorizontal();

        EditorGUILayout.HelpBox("This Template requires a custom InputManager, Layers, Tags and a PhysicsManager.\n \n" +
            "By importing our ProjectSettings it will overwrite your previous settings. \n \n" +
            "If you want to keep your settings you can see what this template requires in order to work correctly on our Online Documentation and add manually later. \n \n" +
            "We always recommend to import the Template into a New and Clean Project, using it as a base to build your game.", MessageType.Warning, true);

        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if (GUILayout.Button("Import Project Settings"))
        {
            AssetDatabase.ImportPackage(vSettingsResourcesPath, true);
        }

        GUILayout.BeginHorizontal();

        var _show = EditorGUILayout.Toggle(show, GUILayout.MaxWidth(14));

        if (_show != show)
        {
            EditorPrefs.SetBool(vSettingsShowAgainKey, _show);
            show = _show;
        }

        var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleLeft };

        EditorGUILayout.LabelField("Don't display this window again", style, GUILayout.ExpandWidth(true));

        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }
}
