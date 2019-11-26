using UnityEngine;
using UnityEditor;
using Boxophobic;
using System.IO;
using System.Collections.Generic;

public class ADSConverter : EditorWindow
{

    public string[] ChannelEnum = new string[]
    {
        "None",
        "Vertex Color R", "Vertex Color G", "Vertex Color B", "Vertex Color A",
        "Vertex Position X", "Vertex Position Y", "Vertex Position Z",
        "UV Coord0 X", "UV Coord0 Y", "UV Coord0 Z", "UV Coord0 W",
        "UV Coord2 X", "UV Coord2 Y", "UV Coord2 Z", "UV Coord2 W",
        "UV Coord3 X", "UV Coord3 Y", "UV Coord3 Z", "UV Coord3 W",
        "UV Coord4 X", "UV Coord4 Y", "UV Coord4 Z", "UV Coord4 W"
    };

//    public string[] VariationEnumCustom = new string[]
//    {
//        "None",
//        "VertexColor R", "VertexColor G", "VertexColor B", "VertexColor A",
//        "VertexPos X", "VertexPos Y", "VertexPos Z",
//        "UV Coord0 X", "UV Coord0 Y", "UV Coord0 Z", "UV Coord0 W",
//        "UV Coord2 X", "UV Coord2 Y", "UV Coord2 Z", "UV Coord2 W",
//        "UV Coord3 X", "UV Coord3 Y", "UV Coord3 Z", "UV Coord3 W",
//        "UV Coord4 X", "UV Coord4 Y", "UV Coord4 Z", "UV Coord4 W",
//        "Auto Generate (may no be accurate)"
//    };

//    public string[] VariationEnum = new string[]
//    {
//        "None",
//        "From Mesh",
//        "Auto Generate (may no be accurate)"
//    };

    //public string[] UVEnum = new string[]
    //{
    //    "None",
    //    "UV Coord0", "UV Coord2", "UV Coord3", "UV Coord4",
    //};

    public enum MeshPackingEnum
    {
        Generic = 0,
        Cloth = 1,
        Grass = 5,
        Plant = 10,
        Tree = 20,
    };
    public MeshPackingEnum meshPacking;

    public enum GrassTypeEnum
    {
        Custom = 0,
    };
    public GrassTypeEnum grassType = GrassTypeEnum.Custom;

    public enum PlantTypeEnum
    {
        Custom = 0,
    };
    public PlantTypeEnum plantType = PlantTypeEnum.Custom;

    public enum TreeTypeEnum
    {
        Custom = 0,
        SpeedTree = 1,
        TreeIt = 2,
    };
    public TreeTypeEnum treeType = TreeTypeEnum.Custom;

    public int motionMask;
    public int motionMask2;
    public int motionMask3;
    public int motionVariation;
    public int vertexAO;
    //public int scanUV;

    public float maxHeight;

    public bool motionMaskInvert;
    public bool motionMask2Invert;
    public bool motionMask3Invert;

    Mesh inputMesh;
    float[] meshChannel;
    List<Vector4> meshUV;
    string packingType;

    Color guiColor;
    string bannerText;
    string helpURL;
    static ADSConverter Window;

    [MenuItem("Window/BOXOPHOBIC/Advanced Dynamic Shaders/Converter")]
    public static void ShowWindow()
    {

        Window = GetWindow<ADSConverter>(false, "ADS Converter", true);
        Window.minSize = new Vector2(480, 540);
        //Window.maxSize = new Vector2(600f, 300f);

    }

    void OnEnable()
    {

        bannerText = "ADS Converter";
        helpURL = "https://docs.google.com/document/d/1PG_9bb0iiFGoi_yQd8IX0K3sMohuWqyq6_qa4AyVpFw/edit#heading=h.lei5ufvottrg";

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

    void OnGUI()
    {

        BEditorGUI.DrawWindowBanner(guiColor, bannerText, helpURL);

        GUILayout.BeginHorizontal();
        GUILayout.Space(25);

        GUILayout.BeginVertical();

        //BEditorGUI.DrawWindowCategory("Mesh Input");
        DrawMeshInput();

        //BEditorGUI.DrawWindowCategory("Mesh Packing");
        GUILayout.Space(10);
        DrawPacking();

        //DrawMessageLightmap();

        DrawMessageExistingFile();

        DrawButton();

        GUILayout.EndVertical();

        GUILayout.Space(20);
        GUILayout.EndHorizontal();


        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();

        BEditorGUI.DrawLogo();

        GUILayout.FlexibleSpace();
        GUILayout.Space(20);
        GUILayout.EndVertical();

    }

    void DrawMeshInput()
    {

        if (inputMesh != null)
        {
            if (inputMesh.name.Contains(" (ADSPacked QuickMask)"))
            {
                inputMesh.name = inputMesh.name.Replace(" (ADSPacked QuickMask)", "");
            }
        }

        inputMesh = (Mesh)EditorGUILayout.ObjectField("Mesh Object", inputMesh, typeof(Mesh), false);
        meshPacking = (MeshPackingEnum)EditorGUILayout.EnumPopup("Mesh Packing", meshPacking);

    }

    void DrawPacking()
    {

        if (meshPacking == MeshPackingEnum.Generic)
        {
            //ResetInvertAndValues();

            packingType = "Generic";

            motionMask = EditorGUILayout.Popup("Mask Source", motionMask, ChannelEnum);

            if (motionMask == 0)
            {
                motionMaskInvert = false;
            }
            else
            {
                motionMaskInvert = EditorGUILayout.Toggle("Mask Invert", motionMaskInvert);
            }

            motionVariation = EditorGUILayout.Popup("Variation Source", motionVariation, ChannelEnum);
        }
        else if (meshPacking == MeshPackingEnum.Cloth)
        {
            //ResetInvertAndValues();

            packingType = "Cloth";

            motionMask = EditorGUILayout.Popup("Mask Source", motionMask, ChannelEnum);

            if (motionMask == 0)
            {
                motionMaskInvert = false;
            }
            else
            {
                motionMaskInvert = EditorGUILayout.Toggle("Mask Invert", motionMaskInvert);
            }

            motionVariation = EditorGUILayout.Popup("Variation Source", motionVariation, ChannelEnum);
        }
        else if (meshPacking == MeshPackingEnum.Grass)
        {
            if (grassType == GrassTypeEnum.Custom)
            {
                //ResetInvertAndValues();

                packingType = "Grass";

                grassType = (GrassTypeEnum)EditorGUILayout.EnumPopup("Grass Type", grassType);

                GUILayout.Space(10);

                motionMask = EditorGUILayout.Popup("Grass Mask Source", motionMask, ChannelEnum);

                if (motionMask == 0)
                {
                    motionMaskInvert = false;
                }
                else
                {
                    motionMaskInvert = EditorGUILayout.Toggle("Grass Mask Invert", motionMaskInvert);
                }

                motionVariation = EditorGUILayout.Popup("Variation Source", motionVariation, ChannelEnum);
            }
        }
        else if (meshPacking == MeshPackingEnum.Plant)
        {
            if (plantType == PlantTypeEnum.Custom)
            {
                //ResetInvertAndValues();

                packingType = "Plant";

                plantType = (PlantTypeEnum)EditorGUILayout.EnumPopup("Plant Type", plantType);

                GUILayout.Space(10);

                motionMask = EditorGUILayout.Popup("Plant Mask Source", motionMask, ChannelEnum);

                if (motionMask == 0)
                {
                    motionMaskInvert = false;
                }
                else
                {
                    motionMaskInvert = EditorGUILayout.Toggle("Plant Mask Invert", motionMaskInvert);
                }

                motionMask2 = EditorGUILayout.Popup("Leaf Mask Source", motionMask2, ChannelEnum);

                if (motionMask2 == 0)
                {
                    motionMask2Invert = false;
                }
                else
                {
                    motionMask2Invert = EditorGUILayout.Toggle("Leaf Mask Invert", motionMask2Invert);
                }

                motionVariation = EditorGUILayout.Popup("Variation Source", motionVariation, ChannelEnum);
            }
        }
        else if (meshPacking == MeshPackingEnum.Tree)
        {
            if (treeType == TreeTypeEnum.Custom)
            {
                //ResetInvertAndValues();

                packingType = "Tree";

                treeType = (TreeTypeEnum)EditorGUILayout.EnumPopup("Tree Type", treeType);

                GUILayout.Space(10);

                motionMask = EditorGUILayout.Popup("Tree Mask Source", motionMask, ChannelEnum);

                if (motionMask == 0)
                {
                    motionMaskInvert = false;
                }
                else
                {
                    motionMaskInvert = EditorGUILayout.Toggle("Tree Mask Invert", motionMaskInvert);
                }

                motionMask2 = EditorGUILayout.Popup("Branch Mask Source", motionMask2, ChannelEnum);

                if (motionMask2 == 0)
                {
                    motionMask2Invert = false;
                }
                else
                {
                    motionMask2Invert = EditorGUILayout.Toggle("Branch Mask Invert", motionMask2Invert);
                }

                motionMask3 = EditorGUILayout.Popup("Leaf Mask Source", motionMask3, ChannelEnum);

                if (motionMask3 == 0)
                {
                    motionMask3Invert = false;
                }
                else
                {
                    motionMask3Invert = EditorGUILayout.Toggle("Leaf Mask Invert", motionMask3Invert);
                }

                vertexAO = EditorGUILayout.Popup("Vertex AO Source", vertexAO, ChannelEnum);

                motionVariation = EditorGUILayout.Popup("Variation Source", motionVariation, ChannelEnum);

                //scanUV = EditorGUILayout.Popup("Scan UV Source", scanUV, UVEnum);
            }
            else if (treeType == TreeTypeEnum.SpeedTree)
            {
                packingType = "Tree SpeedTree";

                treeType = (TreeTypeEnum)EditorGUILayout.EnumPopup("Tree Type", treeType);
            }
            else if (treeType == TreeTypeEnum.TreeIt)
            {
                packingType = "Tree TreeIt";

                treeType = (TreeTypeEnum)EditorGUILayout.EnumPopup("Tree Type", treeType);

                GUILayout.Space(10);

                maxHeight = EditorGUILayout.Slider("Tree Height", maxHeight, 0.0f, 100.0f);
            }
        }

    }

    void DrawMessageExistingFile()
    {

        if (inputMesh != null)
        {
            var meshPath = AssetDatabase.GetAssetPath(inputMesh);
            var assetName = Path.GetFileName(meshPath);
            var path = meshPath.Replace(assetName, "");

            if (AssetDatabase.LoadAssetAtPath<Object>(path + inputMesh.name + " (ADSPacked " + packingType + ").asset") != null)
            {
                GUILayout.Space(20);
                EditorGUILayout.HelpBox("A mesh with the same name already exist. Mesh Packer will overrride it!", MessageType.Info, true);
                //GUILayout.Space(10);
            }
        }

    }

    //void DrawMessageLightmap()
    //{
    //    GUILayout.Space(20);
    //    EditorGUILayout.HelpBox("For Lightmap usage, your mesh need to have Lightmap UVs!", MessageType.Info, true);
    //}

    void DrawButton()
    {

        if (inputMesh == null)
        {
            GUI.enabled = false;
        }

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        GUILayout.Label("");
        if (GUILayout.Button("Pack Mesh", GUILayout.Width(160)))
        {
            Packing();
        }
        GUILayout.Label("");
        GUILayout.EndHorizontal();
        GUI.enabled = true;

    }

    void Packing()
    {
        if (meshPacking == MeshPackingEnum.Generic)
        {
            GenericPacking();
        }
        else if (meshPacking == MeshPackingEnum.Cloth)
        {
            ClothPacking();
        }
        else if (meshPacking == MeshPackingEnum.Grass)
        {
            if (grassType == GrassTypeEnum.Custom)
            {
                GrassPackingCustom();
            }
        }
        else if (meshPacking == MeshPackingEnum.Plant)
        {
            if (plantType == PlantTypeEnum.Custom)
            {
                PlantPackingCustom();
            }
        }
        else if (meshPacking == MeshPackingEnum.Tree)
        {
            if (treeType == TreeTypeEnum.Custom)
            {
                TreePackingCustom();
            }
            else if (treeType == TreeTypeEnum.SpeedTree)
            {
                TreePackingSpeedTree();
            }
            else if (treeType == TreeTypeEnum.TreeIt)
            {
                TreePackingTreeIt();
            }
        }
    }

    void SaveMesh(Mesh adsMesh)
    {

        var meshPath = AssetDatabase.GetAssetPath(inputMesh);
        var assetName = Path.GetFileName(meshPath);
        var path = meshPath.Replace(assetName, "");

        if (AssetDatabase.LoadAssetAtPath<Object>(path + inputMesh.name + " (ADSPacked " + packingType + ").asset") != null)
        {
            var adsMeshFile = AssetDatabase.LoadAssetAtPath<Mesh>(path + inputMesh.name + " (ADSPacked " + packingType + ").asset");
            adsMeshFile.Clear();
            EditorUtility.CopySerialized(adsMesh, adsMeshFile);

            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(path + inputMesh.name + " (ADSPacked " + packingType + ").asset"));
        }
        else
        {
            AssetDatabase.CreateAsset(adsMesh, path + inputMesh.name + " (ADSPacked " + packingType + ").asset");

            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(path + inputMesh.name + " (ADSPacked " + packingType + ").asset"));
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

    }

    void Remap01(float[] source)
    {

        float min = source[0];
        float max = source[0];

        for (int i = 0; i < source.Length; i++)
        {
            if (source[i] < min)
                min = source[i];

            if (source[i] > max)
                max = source[i];
        }

        for (int i = 0; i < source.Length; i++)
        {
            source[i] = (source[i] - min) / (max - min);
        }
    }

    float Remap01(float source, float min, float max)
    {

        float val = (source - min) / (max - min);
        return val;

    }

    float Remap01(float source, float max)
    {

        float val = source / max;
        return val;

    }

    float SourceInvert(float source, bool inv)
    {

        if (inv)
        {
            source = 1 - source;
        }

        return source;

    }

    float[] GetMeshSource(int c, float val)
    {
        meshChannel = new float[inputMesh.vertexCount];

        var UV0 = new List<Vector4>(inputMesh.vertexCount);
        inputMesh.GetUVs(0, UV0);

        var UV2 = new List<Vector4>(inputMesh.vertexCount);
        try
        {
            inputMesh.GetUVs(1, UV2);
        }
        catch
        {
            Debug.Log("The Input Mesh does not have UV Channel 2");
        }

        var UV3 = new List<Vector4>(inputMesh.vertexCount);
        try
        {
            inputMesh.GetUVs(2, UV3);
        }
        catch
        {
            Debug.Log("The Input Mesh does not have UV Channel 3");
        }

        var UV4 = new List<Vector4>(inputMesh.vertexCount);
        try
        {
            inputMesh.GetUVs(3, UV4);
        }
        catch
        {
            Debug.Log("The Input Mesh does not have UV Channel 4");
        }


        switch (c)
        {
            case 0:
                for (int i = 0; i < inputMesh.vertexCount; i++)
                {
                    meshChannel[i] = val;
                }
                break;

            //Get vertex color channels
            case 1:
                try
                {
                    for (int i = 0; i < inputMesh.vertexCount; i++)
                    {
                        meshChannel[i] = inputMesh.colors[i].r;
                    }
                }
                catch
                {
                    Debug.Log("The Input Mesh does not have Vertex Color");
                }
                break;

            case 2:
                try
                {
                    for (int i = 0; i < inputMesh.vertexCount; i++)
                    {
                        meshChannel[i] = inputMesh.colors[i].g;
                    }
                }
                catch
                {
                    Debug.Log("The Input Mesh does not have Vertex Color");
                }
                break;

            case 3:
                try
                {
                    for (int i = 0; i < inputMesh.vertexCount; i++)
                    {
                        meshChannel[i] = inputMesh.colors[i].b;
                    }
                }
                catch
                {
                    Debug.Log("The Input Mesh does not have Vertex Color");
                }
                break;

            case 4:
                try
                {
                    for (int i = 0; i < inputMesh.vertexCount; i++)
                    {
                        meshChannel[i] = inputMesh.colors[i].a;
                    }
                }
                catch
                {
                    Debug.Log("The Input Mesh does not have Vertex Color");
                }
                break;

            //Get vertex position channels
            case 5:
                for (int i = 0; i < inputMesh.vertexCount; i++)
                {
                    meshChannel[i] = inputMesh.vertices[i].x;
                }
                Remap01(meshChannel);
                break;

            case 6:
                for (int i = 0; i < inputMesh.vertexCount; i++)
                {
                    meshChannel[i] = inputMesh.vertices[i].y;
                }
                Remap01(meshChannel);
                break;

            case 7:
                for (int i = 0; i < inputMesh.vertexCount; i++)
                {
                    meshChannel[i] = inputMesh.vertices[i].z;
                }
                Remap01(meshChannel);
                break;

            //Get UV0 channels
            case 8:
                for (int i = 0; i < inputMesh.vertexCount; i++)
                {
                    meshChannel[i] = UV0[i].x;
                }
                Remap01(meshChannel);
                break;

            case 9:
                for (int i = 0; i < inputMesh.vertexCount; i++)
                {
                    meshChannel[i] = UV0[i].y;
                }
                Remap01(meshChannel);
                break;

            case 10:
                for (int i = 0; i < inputMesh.vertexCount; i++)
                {
                    meshChannel[i] = UV0[i].z;
                }
                Remap01(meshChannel);
                break;

            case 11:
                for (int i = 0; i < inputMesh.vertexCount; i++)
                {
                    meshChannel[i] = UV0[i].w;
                }
                Remap01(meshChannel);
                break;

            //Get UV2 channels
            case 12:
                try
                {
                    for (int i = 0; i < inputMesh.vertexCount; i++)
                    {
                        meshChannel[i] = UV2[i].x;
                    }
                    Remap01(meshChannel);
                }
                catch
                {
                    Debug.Log("The Input Mesh does not have UV Channel 2");
                }
                break;

            case 13:
                try
                {
                    for (int i = 0; i < inputMesh.vertexCount; i++)
                    {
                        meshChannel[i] = UV2[i].y;
                    }
                    Remap01(meshChannel);
                }
                catch
                {
                    Debug.Log("The Input Mesh does not have UV Channel 2");
                }
                break;

            case 14:
                try
                {
                    for (int i = 0; i < inputMesh.vertexCount; i++)
                    {
                        meshChannel[i] = UV2[i].z;
                    }
                    Remap01(meshChannel);
                }
                catch
                {
                    Debug.Log("The Input Mesh does not have UV Channel 2");
                }
                break;

            case 15:
                try
                {
                    for (int i = 0; i < inputMesh.vertexCount; i++)
                    {
                        meshChannel[i] = UV2[i].w;
                    }
                    Remap01(meshChannel);
                }
                catch
                {
                    Debug.Log("The Input Mesh does not have UV Channel 2");
                }
                break;

            //Get UV3 channels
            case 16:
                try
                {
                    for (int i = 0; i < inputMesh.vertexCount; i++)
                    {
                        meshChannel[i] = UV3[i].x;
                    }
                    Remap01(meshChannel);
                }
                catch
                {
                    Debug.Log("The Input Mesh does not have UV Channel 3");
                }
                break;

            case 17:
                try
                {
                    for (int i = 0; i < inputMesh.vertexCount; i++)
                    {
                        meshChannel[i] = UV3[i].y;
                    }
                    Remap01(meshChannel);
                }
                catch
                {
                    Debug.Log("The Input Mesh does not have UV Channel 3");
                }
                break;

            case 18:
                try
                {
                    for (int i = 0; i < inputMesh.vertexCount; i++)
                    {
                        meshChannel[i] = UV3[i].z;
                    }
                    Remap01(meshChannel);
                }
                catch
                {
                    Debug.Log("The Input Mesh does not have UV Channel 3");
                }
                break;

            case 19:
                try
                {
                    for (int i = 0; i < inputMesh.vertexCount; i++)
                    {
                        meshChannel[i] = UV3[i].w;
                    }
                    Remap01(meshChannel);
                }
                catch
                {
                    Debug.Log("The Input Mesh does not have UV Channel 3");
                }
                break;

            //Get UV4 channels
            case 20:
                try
                {
                    for (int i = 0; i < inputMesh.vertexCount; i++)
                    {
                        meshChannel[i] = UV4[i].x;
                    }
                    Remap01(meshChannel);
                }
                catch
                {
                    Debug.Log("The Input Mesh does not have UV Channel 4");
                }
                break;

            case 21:
                try
                {
                    for (int i = 0; i < inputMesh.vertexCount; i++)
                    {
                        meshChannel[i] = UV4[i].y;
                    }
                    Remap01(meshChannel);
                }
                catch
                {
                    Debug.Log("The Input Mesh does not have UV Channel 4");
                }
                break;

            case 22:
                try
                {
                    for (int i = 0; i < inputMesh.vertexCount; i++)
                    {
                        meshChannel[i] = UV4[i].z;
                    }
                    Remap01(meshChannel);
                }
                catch
                {
                    Debug.Log("The Input Mesh does not have UV Channel 4");
                }
                break;

            case 23:
                try
                {
                    for (int i = 0; i < inputMesh.vertexCount; i++)
                    {
                        meshChannel[i] = UV4[i].w;
                    }
                    Remap01(meshChannel);
                }
                catch
                {
                    Debug.Log("The Input Mesh does not have UV Channel 4");
                }
                break;
            case 24:
                for (int i = 0; i < inputMesh.vertexCount; i++)
                {
                    meshChannel[i] = -99;
                }

                for (int i = 0; i < inputMesh.triangles.Length; i += 3)
                {
                    var index1 = inputMesh.triangles[i + 0];
                    var index2 = inputMesh.triangles[i + 1];
                    var index3 = inputMesh.triangles[i + 2];

                    float variation = 0;

                    if (meshChannel[index1] != -99)
                    {
                        variation = meshChannel[index1];
                    }
                    else if (meshChannel[index2] != -99)
                    {
                        variation = meshChannel[index2];
                    }
                    else if (meshChannel[index3] != -99)
                    {
                        variation = meshChannel[index3];
                    }
                    else
                    {
                        variation = Random.Range(0.0f, 1.0f);
                    }

                    meshChannel[index1] = variation;
                    meshChannel[index2] = variation;
                    meshChannel[index3] = variation;
                }
                break;
        }

        return meshChannel;
    }

    float[] GenerateMeshVariation()
    {
        // Testing new aproach
        //Mesh aMesh = new Mesh();
        //aMesh.Clear();

        //aMesh.vertices = inputMesh.vertices;
        //aMesh.normals = inputMesh.normals;
        //aMesh.uv = inputMesh.uv;
        //aMesh.triangles = inputMesh.triangles;

        //Debug.Log(inputMesh.triangles.Length);

        //var aMaxDelta = 0.01f;

        //var verts = aMesh.vertices;
        //var normals = aMesh.normals;
        //var uvs = aMesh.uv;
        //List<int> newVerts = new List<int>();
        //int[] map = new int[verts.Length];
        //// create mapping and filter duplicates.
        //for (int i = 0; i < verts.Length; i++)
        //{
        //    var p = verts[i];
        //    var n = normals[i];
        //    var uv = uvs[i];
        //    bool duplicate = false;
        //    for (int i2 = 0; i2 < newVerts.Count; i2++)
        //    {
        //        int a = newVerts[i2];
        //        if (
        //            (verts[a] - p).sqrMagnitude <= aMaxDelta && // compare position
        //            Vector3.Angle(normals[a], n) <= aMaxDelta && // compare normal
        //            (uvs[a] - uv).sqrMagnitude <= aMaxDelta // compare first uv coordinate
        //            )
        //        {
        //            map[i] = i2;
        //            duplicate = true;
        //            break;
        //        }
        //    }
        //    if (!duplicate)
        //    {
        //        map[i] = newVerts.Count;
        //        newVerts.Add(i);
        //    }
        //}
        //// create new vertices
        //var verts2 = new Vector3[newVerts.Count];
        //var normals2 = new Vector3[newVerts.Count];
        //var uvs2 = new Vector2[newVerts.Count];
        //for (int i = 0; i < newVerts.Count; i++)
        //{
        //    int a = newVerts[i];
        //    verts2[i] = verts[a];
        //    normals2[i] = normals[a];
        //    uvs2[i] = uvs[a];
        //}
        //// map the triangle to the new vertices
        //var tris = aMesh.triangles;
        //for (int i = 0; i < tris.Length; i++)
        //{
        //    tris[i] = map[tris[i]];
        //}

        //aMesh.vertices = verts2;
        //aMesh.normals = normals2;
        //aMesh.uv = uvs2;
        //aMesh.triangles = tris;

        //Debug.Log(verts2.Length);

        //var tempMeshVariation = new float[aMesh.vertexCount];

        //for (int i = 0; i < aMesh.vertexCount; i++)
        //{
        //    tempMeshVariation[i] = -99;            
        //}

        //for (int i = 0; i < aMesh.triangles.Length; i += 3)
        //{
        //    var index1 = aMesh.triangles[i + 0];
        //    var index2 = aMesh.triangles[i + 1];
        //    var index3 = aMesh.triangles[i + 2];

        //    float variation = 0;

        //    if (tempMeshVariation[index1] != -99)
        //    {
        //        variation = tempMeshVariation[index1];
        //    }
        //    else if (tempMeshVariation[index2] != -99)
        //    {
        //        variation = tempMeshVariation[index2];
        //    }
        //    else if (tempMeshVariation[index3] != -99)
        //    {
        //        variation = tempMeshVariation[index3];
        //    }
        //    else
        //    {
        //        variation = Random.Range(0.0f, 1.0f);
        //    }

        //    tempMeshVariation[index1] = variation;
        //    tempMeshVariation[index2] = variation;
        //    tempMeshVariation[index3] = variation;
        //}

        //var meshVariation = new float[inputMesh.vertexCount];

        //for (int i = 0; i < inputMesh.triangles.Length; i += 3)
        //{
        //    var index1 = inputMesh.triangles[i + 0];
        //    var index2 = inputMesh.triangles[i + 1];
        //    var index3 = inputMesh.triangles[i + 2];

        //    meshVariation[index1] = tempMeshVariation[index1];
        //    meshVariation[index2] = tempMeshVariation[index2];
        //    meshVariation[index3] = tempMeshVariation[index3];
        //}

        // Good Enough approach
        var meshVariation = new float[inputMesh.vertexCount];

        for (int i = 0; i < inputMesh.vertexCount; i++)
        {
            meshVariation[i] = -99;
        }

        for (int i = 0; i < inputMesh.triangles.Length; i += 3)
        {
            var index1 = inputMesh.triangles[i + 0];
            var index2 = inputMesh.triangles[i + 1];
            var index3 = inputMesh.triangles[i + 2];

            float variation = 0;

            if (meshVariation[index1] != -99)
            {
                variation = meshVariation[index1];
            }
            else if (meshVariation[index2] != -99)
            {
                variation = meshVariation[index2];
            }
            else if (meshVariation[index3] != -99)
            {
                variation = meshVariation[index3];
            }
            else
            {
                variation = Random.Range(0.0f, 1.0f);
            }

            meshVariation[index1] = variation;
            meshVariation[index2] = variation;
            meshVariation[index3] = variation;
        }

        return meshVariation;

    }

    List<Vector4> GetMeshUV(int c)
    {
        meshUV = new List<Vector4>(inputMesh.vertexCount);

        var UV0 = new List<Vector4>(inputMesh.vertexCount);
        inputMesh.GetUVs(0, UV0);

        var UV2 = new List<Vector4>(inputMesh.vertexCount);
        try
        {
            inputMesh.GetUVs(1, UV2);
        }
        catch
        {
            Debug.Log("The Input Mesh does not have UV Channel 2");
        }

        var UV3 = new List<Vector4>(inputMesh.vertexCount);
        try
        {
            inputMesh.GetUVs(2, UV3);
        }
        catch
        {
            Debug.Log("The Input Mesh does not have UV Channel 3");
        }

        var UV4 = new List<Vector4>(inputMesh.vertexCount);
        try
        {
            inputMesh.GetUVs(3, UV4);
        }
        catch
        {
            Debug.Log("The Input Mesh does not have UV Channel 4");
        }


        switch (c)
        {
            //Get UV channels
            case 1:
                meshUV = UV0;
                break;

            case 2:
                try
                {
                    meshUV = UV2;
                }
                catch
                {
                    Debug.Log("The Input Mesh does not have UV Channel 2");
                }
                break;

            case 3:
                try
                {
                    meshUV = UV3;
                }
                catch
                {
                    Debug.Log("The Input Mesh does not have UV Channel 3");
                }
                break;

            case 4:
                try
                {
                    meshUV = UV4;
                }
                catch
                {
                    Debug.Log("The Input Mesh does not have UV Channel 4");
                }
                break;
        }

        return meshUV;
    }

    //Unused
    //void ResetInvertAndValues()
    //{

    //    if (resetInvertAndValues == false)
    //    {
    //        motionMaskInvert = false;
    //        motionMask2Invert = false;
    //        motionMask3Invert = false;
    //        motionVariationInvert = false;
    //        vertexAOInvert = false;

    //        motionMaskValue = 0.0f;
    //        motionMask2Value = 0.0f;
    //        motionMask3Value = 0.0f;
    //        motionVariationValue = 0.0f;
    //        vertexAOValue = 0.0f;

    //        resetInvertAndValues = true;
    //    }

    //}

    // Not working
    //void HandleInvertAndValues(string label, int input, bool invert, float val)
    //{

    //    if (input == 0)
    //    {
    //        invert = false;
    //    }
    //    else if (input == 1)
    //    {
    //        invert = false;
    //        val = EditorGUILayout.Slider(label + " Value", val, -1.0f, 1.0f);
    //    }
    //    else
    //    {
    //        invert = EditorGUILayout.Toggle(label + " Invert", invert);
    //    }

    //}

    void GenericPacking()
    {

        Color[] colors = new Color[inputMesh.vertexCount];

        var mMask = GetMeshSource(motionMask, 0.0f);
        var mVariation = GetMeshSource(motionVariation, 2.0f);

        for (int i = 0; i < inputMesh.vertexCount; i++)
        {
            colors[i] = new Color(SourceInvert(mMask[i], motionMaskInvert), 0.0f, 0.0f, mVariation[i]);
        }

        Mesh adsMesh = new Mesh();
        adsMesh = Instantiate(inputMesh);
        adsMesh.name = inputMesh.name + " (ADSPacked " + packingType + ")";
        adsMesh.colors = colors;

        SaveMesh(adsMesh);

    }

    void ClothPacking()
    {

        Color[] colors = new Color[inputMesh.vertexCount];

        var mMask = GetMeshSource(motionMask, 0.0f);
        var mVariation = GetMeshSource(motionVariation, 2.0f);

        for (int i = 0; i < inputMesh.vertexCount; i++)
        {
            colors[i] = new Color(SourceInvert(mMask[i], motionMaskInvert), 0.0f, 0.0f, mVariation[i]);
        }

        Mesh adsMesh = new Mesh();
        adsMesh = Instantiate(inputMesh);
        adsMesh.name = inputMesh.name + " (ADSPacked " + packingType + ")";
        adsMesh.colors = colors;

        SaveMesh(adsMesh);

    }

    void GrassPackingCustom()
    {

        Color[] colors = new Color[inputMesh.vertexCount];
        var UV4 = new List<Vector3>();
        inputMesh.GetUVs(0, UV4);

        var mMask = GetMeshSource(motionMask, 0.0f);
        var mVariation = GetMeshSource(motionVariation, 2.0f);

        for (int i = 0; i < inputMesh.vertexCount; i++)
        {
            colors[i] = new Color(SourceInvert(mMask[i], motionMaskInvert), 0.0f, 0.0f, mVariation[i]);
            UV4[i] = new Vector3(inputMesh.vertices[i].x, inputMesh.vertices[i].y, inputMesh.vertices[i].z);
        }

        Mesh adsMesh = new Mesh();
        adsMesh = Instantiate(inputMesh);
        adsMesh.name = inputMesh.name + " (ADSPacked " + packingType + ")";
        adsMesh.colors = colors;
        adsMesh.SetUVs(3, UV4);

        SaveMesh(adsMesh);

    }

    void PlantPackingCustom()
    {

        Color[] colors = new Color[inputMesh.vertexCount];
        var UV4 = new List<Vector3>();
        inputMesh.GetUVs(0, UV4);

        var mMask = GetMeshSource(motionMask, 0.0f);
        var mMask2 = GetMeshSource(motionMask2, 0.0f);
        var mVariation = GetMeshSource(motionVariation, 2.0f);

        for (int i = 0; i < inputMesh.vertexCount; i++)
        {
            colors[i] = new Color(SourceInvert(mMask[i], motionMaskInvert), 0.0f, SourceInvert(mMask2[i], motionMask2Invert), mVariation[i]);
            UV4[i] = new Vector3(inputMesh.vertices[i].x, inputMesh.vertices[i].y, inputMesh.vertices[i].z);
        }

        Mesh adsMesh = new Mesh();
        adsMesh = Instantiate(inputMesh);
        adsMesh.name = inputMesh.name + " (ADSPacked " + packingType + ")";
        adsMesh.colors = colors;
        adsMesh.SetUVs(3, UV4);

        SaveMesh(adsMesh);

    }

    void TreePackingCustom()
    {

        Color[] colors = new Color[inputMesh.vertexCount];

        var UV0 = new List<Vector4>();
        inputMesh.GetUVs(0, UV0);

        var UV4 = new List<Vector2>();
        inputMesh.GetUVs(0, UV4);

        var mMask = GetMeshSource(motionMask, 0.0f);
        var mMask2 = GetMeshSource(motionMask2, 0.0f);
        var mMask3 = GetMeshSource(motionMask3, 0.0f);
        var vAO = GetMeshSource(vertexAO, 1.0f);
        var mVariation = GetMeshSource(motionVariation, 2.0f);
        var mLeafAmount = GenerateMeshVariation();

        for (int i = 0; i < inputMesh.vertexCount; i++)
        {
            colors[i] = new Color(SourceInvert(mMask[i], motionMaskInvert), SourceInvert(mMask2[i], motionMask2Invert), SourceInvert(mMask3[i], motionMask3Invert), mVariation[i]);
            UV0[i] = new Vector4(UV0[i].x, UV0[i].y, vAO[i], mLeafAmount[i]);
        }

        //if (scanUV != 0)
        //{
        //    UV4 = GetMeshUV(scanUV);
        //}

        Mesh adsMesh = new Mesh();
        adsMesh = Instantiate(inputMesh);
        adsMesh.name = inputMesh.name + " (ADSPacked " + packingType + ")";
        adsMesh.colors = colors;
        adsMesh.SetUVs(0, UV0);
        adsMesh.SetUVs(3, UV4);

        SaveMesh(adsMesh);

    }

    void TreePackingSpeedTree()
    {

        Color[] colors = new Color[inputMesh.vertexCount];

        var UV0 = new List<Vector4>();
        var UV3 = new List<Vector4>();
        var UV4 = new List<Vector2>();
        inputMesh.GetUVs(0, UV0);
        inputMesh.GetUVs(2, UV3);
        inputMesh.GetUVs(0, UV4);

        var mLeafAmount = GenerateMeshVariation();

        // Remap Branch Mask, Leaf Mask and Variation to 0-1
        float maxBranch = UV0[0].z;
        float maxLeaf = UV3[0].x;
        float maxVariation = UV0[0].w;

        for (int i = 0; i < UV0.Count; i++)
        {
            if (UV0[i].z > maxBranch)
            {
                maxBranch = UV0[i].z;
            }
            if (UV3[i].x > maxLeaf)
            {
                maxLeaf = UV3[i].x;
            }
            if (UV0[i].w > maxVariation)
            {
                maxVariation = UV0[i].w;
            }
        }

        // Remap Trunk Mask to 0-1
        float maxTrunk = inputMesh.vertices[0].y;
        float minTrunk = inputMesh.vertices[0].y;

        for (int i = 0; i < inputMesh.vertexCount; i++)
        {
            if (inputMesh.vertices[i].y > maxTrunk)
            {
                maxTrunk = inputMesh.vertices[i].y;
            }
            if (inputMesh.vertices[i].y < minTrunk)
            {
                minTrunk = inputMesh.vertices[i].y;
            }
        }

        for (int i = 0; i < inputMesh.vertexCount; i++)
        {
            //Height Mask (VertexPos.y), Branch Mask (UV0.z), Leaf Mask (UV3.x), Variation(UV0.w)
            colors[i] = new Color(Remap01(inputMesh.vertices[i].y, minTrunk, maxTrunk), UV0[i].z / maxBranch, UV3[i].x / maxLeaf, UV0[i].w / maxVariation);
            //Mesh UV0, Branch Blending(UV3.z), AmbientOcclusion (VertexColor Red) 
            UV0[i] = new Vector4(UV0[i].x, UV0[i].y, inputMesh.colors[i].r, mLeafAmount[i]);
        }

        Mesh adsMesh = new Mesh();
        adsMesh = Instantiate(inputMesh);
        adsMesh.name = inputMesh.name + " (ADSPacked " + packingType + ")";
        adsMesh.colors = colors;
        adsMesh.SetUVs(0, UV0);
        adsMesh.SetUVs(3, UV4);
        //adsMesh.vertices = mesh.vertices;
        //adsMesh.triangles = mesh.triangles;
        //adsMesh.uv = mesh.uv;
        //adsMesh.normals = mesh.normals;
        //adsMesh.colors = colors;
        //adsMesh.tangents = mesh.tangents;

        SaveMesh(adsMesh);

    }

    void TreePackingTreeIt()
    {

        Color[] colors = new Color[inputMesh.vertexCount];

        var UV0 = new List<Vector4>();
        var UV4 = new List<Vector2>();
        inputMesh.GetUVs(0, UV0);
        inputMesh.GetUVs(0, UV4);

        var mLeafAmount = GenerateMeshVariation();

        for (int i = 0; i < inputMesh.vertexCount; i++)
        {
            //Height Mask (VertexPos.y), Branch Mask (VertexColor Blue), Leaf Mask (VertexColor Red), Variation (VertexColor Green)
            colors[i] = new Color(Remap01(inputMesh.vertices[i].y, maxHeight), inputMesh.colors[i].b, inputMesh.colors[i].r, inputMesh.colors[i].g);
            //Mesh UV0, AmbientOcclusion (None)
            UV0[i] = new Vector4(UV0[i].x, UV0[i].y, 1.0f, mLeafAmount[i]);
        }

        Mesh adsMesh = new Mesh();
        adsMesh = Instantiate(inputMesh);
        adsMesh.name = inputMesh.name + " (ADSPacked " + packingType + ")";
        adsMesh.colors = colors;
        adsMesh.SetUVs(0, UV0);
        adsMesh.SetUVs(3, UV4);
        //adsMesh.vertices = mesh.vertices;
        //adsMesh.triangles = mesh.triangles;
        //adsMesh.uv = mesh.uv;
        //adsMesh.normals = mesh.normals;
        //adsMesh.colors = colors;
        //adsMesh.tangents = mesh.tangents;

        SaveMesh(adsMesh);

    }

}
