using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Text;
using PWCommon2;
using Gaia.Internal;
using System.Collections.Generic;
using System.Linq;

namespace Gaia
{
    /// <summary>
    /// 
    /// The obj export part of this utility is sourced from http://wiki.unity3d.com/index.php?title=TerrainObjExporter, and adapted largely untouched into 
    /// Gaia as a convenience helper.
    /// 
    /// Authors:
    /// Eric Haines (Eric5h5): original.
    /// Yun Kyu Choi: C# conversion.
    /// Bit Barrel media: progress bar fix.
    /// 
    /// </summary>
    enum ExportSelection { AllActiveTerrains, SingleTerrainOnly }
    enum SaveFormat { Triangles, Quads }
    enum SaveResolution { Full = 0, Half, Quarter, Eighth, Sixteenth }
    enum TextureExportMethod { OrthographicBake, BaseMapExport }
    enum BakeLighting { NeutralLighting, CurrentSceneLighting }
    enum TextureExportResolution { x32 = 32, x64 = 64, x128 = 128, x256 = 256, x512 = 512, x1024 = 1024, x2048 = 2048, x4096 = 4096, x8192 = 8192 }

    [System.Serializable]
    class ExportTerrainLODSettings
    {

        public SaveResolution m_saveResolution = SaveResolution.Half;
        public bool m_exportTextures = true;
        public bool m_exportNormalMaps = true;
        public bool m_exportSplatmaps = true;
        public bool m_addToLODGroup = true;
        public bool m_createMaterials = true;
        public LayerMask m_bakeLayerMask = ~0; //equals "Everything"
        public TextureExportMethod m_textureExportMethod = TextureExportMethod.OrthographicBake;
        public TextureExportResolution m_textureExportResolution = TextureExportResolution.x2048;
        public BakeLighting m_bakeLighting = BakeLighting.NeutralLighting;
        public string namePrefix;
    }


    class ExportTerrain : EditorWindow, IPWEditor
    {
        private EditorUtils m_editorUtils;
        bool m_deactivateOriginalTerrains = true;
        SaveFormat m_saveFormat = SaveFormat.Triangles;
        ExportSelection m_exportSelection = ExportSelection.AllActiveTerrains;
        bool m_setupGameObjects = true;
        List<ExportTerrainLODSettings> m_exportTerrainLODSettings = new List<ExportTerrainLODSettings>();


        static TerrainData terrain;
        static Vector3 terrainPos;

        int tCount = 0;
        int counter;
        int totalCount;
        int progressUpdateInterval = 10000;
        private Vector2 m_scrollPosition;
        private List<Terrain> m_processedTerrains = new List<Terrain>();
        private int m_currentTerrainCount;
        private List<GameObject> m_createdLODParents = new List<GameObject>();

        public bool PositionChecked { get; set; }

        /*
        [MenuItem("Window/Gaia/Export Terrain To Obj...")]
        static void Init()
        {
            terrain = null;
            Terrain terrainObject = Selection.activeObject as Terrain;
            if (!terrainObject)
            {
                terrainObject = Terrain.activeTerrain;
            }
            if (terrainObject)
            {
                terrain = terrainObject.terrainData;
                terrainPos = terrainObject.transform.position;
            }

            EditorWindow.GetWindow<ExportTerrain>().Show();
        }
         */


        void OnEnable()
        {
            terrain = null;
            Terrain terrainObject = Selection.activeObject as Terrain;
            if (!terrainObject)
            {
                terrainObject = Terrain.activeTerrain;
            }
            if (terrainObject)
            {
                terrain = terrainObject.terrainData;
            }

            if (m_editorUtils == null)
            {
                if (tCount == 0) { };
                // Get editor utils for this
                m_editorUtils = PWApp.GetEditorUtils(this);
            }
            titleContent = m_editorUtils.GetContent("WindowTitle");

            if (m_exportTerrainLODSettings.Count <= 0)
            {
                m_exportTerrainLODSettings.Add(new ExportTerrainLODSettings() { m_saveResolution =  SaveResolution.Full, m_textureExportResolution= TextureExportResolution.x4096 });
                m_exportTerrainLODSettings.Add(new ExportTerrainLODSettings() { m_saveResolution = SaveResolution.Half, m_textureExportResolution = TextureExportResolution.x2048 });
                m_exportTerrainLODSettings.Add(new ExportTerrainLODSettings() { m_saveResolution = SaveResolution.Quarter, m_textureExportResolution = TextureExportResolution.x1024 });
                m_exportTerrainLODSettings.Add(new ExportTerrainLODSettings() { m_saveResolution = SaveResolution.Eighth, m_textureExportResolution = TextureExportResolution.x512 });
                m_exportTerrainLODSettings.Add(new ExportTerrainLODSettings() { m_saveResolution = SaveResolution.Sixteenth, m_textureExportResolution = TextureExportResolution.x256 });
            }
        }


        void OnGUI()
        {
            m_editorUtils.Initialize();
            m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition);
            m_editorUtils.Panel("ExportTerrainPanel", ExportTerrainPanel, true);
            GUILayout.EndScrollView();
        }

        private void ExportTerrainPanel(bool helpEnabeld)
        {
            if (Terrain.activeTerrains.Count()<=0)
            {
                EditorGUILayout.HelpBox(m_editorUtils.GetTextValue("NoTerrain"), MessageType.Warning);
            }

            m_exportSelection = (ExportSelection)m_editorUtils.EnumPopup("ExportSelection", m_exportSelection, helpEnabeld);
            m_saveFormat = (SaveFormat)m_editorUtils.EnumPopup("ExportFormat", m_saveFormat, helpEnabeld);
            m_setupGameObjects = m_editorUtils.Toggle("SetupGameObjects", m_setupGameObjects, helpEnabeld);

            if (m_setupGameObjects)
            {
                m_deactivateOriginalTerrains = m_editorUtils.Toggle("DeactivateOriginalTerrains", m_deactivateOriginalTerrains, helpEnabeld);
            }
            else
            {
                m_deactivateOriginalTerrains = false;
            }
            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            m_editorUtils.Heading("LODSettings");

            int LODLevel = 0;

            foreach (ExportTerrainLODSettings LODSettings in m_exportTerrainLODSettings)
            {
                GUILayout.Space(EditorGUIUtility.singleLineHeight);
                m_editorUtils.Label(new GUIContent("LOD Level " + LODLevel.ToString()));
                LODSettings.namePrefix = "LOD" + LODLevel.ToString() + "_";
                LODSettings.m_saveResolution = (SaveResolution)m_editorUtils.EnumPopup("Resolution", LODSettings.m_saveResolution, helpEnabeld);

                LODSettings.m_exportTextures = m_editorUtils.Toggle("ExportTextures", LODSettings.m_exportTextures, helpEnabeld);

                if (LODSettings.m_exportTextures)
                {
                    LODSettings.m_textureExportMethod = (TextureExportMethod)m_editorUtils.EnumPopup("TextureExportMethod", LODSettings.m_textureExportMethod, helpEnabeld);

                    if (LODSettings.m_textureExportMethod == TextureExportMethod.OrthographicBake)
                    {
                        LODSettings.m_textureExportResolution = (TextureExportResolution)m_editorUtils.EnumPopup("TextureResolution", LODSettings.m_textureExportResolution, helpEnabeld);
                        LODSettings.m_bakeLayerMask = GaiaEditorUtils.LayerMaskField(m_editorUtils.GetContent("BakeMask"), LODSettings.m_bakeLayerMask);
                        m_editorUtils.InlineHelp("BakeMask", helpEnabeld);
                        LODSettings.m_bakeLighting = (BakeLighting)m_editorUtils.EnumPopup("BakeLighting", LODSettings.m_bakeLighting, helpEnabeld);
                    }


                }

                LODSettings.m_exportNormalMaps = m_editorUtils.Toggle("ExportNormalMaps", LODSettings.m_exportNormalMaps, helpEnabeld);
                LODSettings.m_exportSplatmaps = m_editorUtils.Toggle("ExportSplatmaps", LODSettings.m_exportSplatmaps, helpEnabeld);
                LODSettings.m_createMaterials = m_editorUtils.Toggle("CreateMaterials", LODSettings.m_createMaterials, helpEnabeld);

                LODLevel++;
            }

            GUILayout.Space(EditorGUIUtility.singleLineHeight);
            GUILayout.BeginHorizontal();
            if (m_exportTerrainLODSettings.Count > 1)
            {
                if (m_editorUtils.Button("RemoveLODLevel"))
                {
                    m_exportTerrainLODSettings.RemoveAt(m_exportTerrainLODSettings.Count() - 1);
                }
            }
            if (m_editorUtils.Button("AddLODLevel"))
            {
                m_exportTerrainLODSettings.Add(new ExportTerrainLODSettings());
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(EditorGUIUtility.singleLineHeight);


            if (m_editorUtils.Button("ExportButton"))
            {
                m_processedTerrains.Clear();
                m_createdLODParents.Clear();
                m_currentTerrainCount = 0;
                List<Light> deactivatedLights = new List<Light>();

                var originalAmbientMode = RenderSettings.ambientMode;
                var originalAmbientColor = RenderSettings.ambientSkyColor;
                var originalLODBias = QualitySettings.lodBias;

                QualitySettings.lodBias = 100;

                foreach (ExportTerrainLODSettings LODSettings in m_exportTerrainLODSettings)
                {

                    if (LODSettings.m_exportTextures && LODSettings.m_textureExportMethod == TextureExportMethod.OrthographicBake && LODSettings.m_bakeLighting == BakeLighting.NeutralLighting)
                    {
                        //Set up neutral ambient lighting
                        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
                        RenderSettings.ambientSkyColor = Color.white;

                        //Switch off all active lights in the scene as they would interfere with the baking for this moder
                        var allLights = Resources.FindObjectsOfTypeAll<Light>();
                        foreach (Light light in allLights)
                        {
                            if (light.isActiveAndEnabled)
                            {
                                light.enabled = false;
                                deactivatedLights.Add(light);
                            }
                        }
                    }

                    try
                    {
                        Export(LODSettings);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Error during Terrain Export: " + ex.Message);
                    }
                    finally
                    {
                        //Restore original lighting
                        foreach (Light light in deactivatedLights)
                        {
                            light.enabled = true;
                        }
                        RenderSettings.ambientMode = originalAmbientMode;
                        RenderSettings.ambientSkyColor = originalAmbientColor;

                        QualitySettings.lodBias = originalLODBias;
                    }
                }

                foreach (GameObject LODParent in m_createdLODParents)
                {
                    LOD[] lods = new LOD[m_exportTerrainLODSettings.Count()];
                    for (int i = 0; i < lods.Count(); i++)
                    {
                        Renderer[] renderers= new Renderer[1];
                        renderers[0] = LODParent.transform.GetChild(i).GetComponent<Renderer>();
                        lods[i] = new LOD() { fadeTransitionWidth = 0.1f, renderers = renderers, screenRelativeTransitionHeight = ((float)(lods.Count()+1) - (float)(i +1)) / (float)(lods.Count()+1) };
                    }
                    LODParent.GetComponent<LODGroup>().SetLODs(lods);
                }


                if (m_deactivateOriginalTerrains)
                {
                    foreach (Terrain t in m_processedTerrains)
                    {
                        t.gameObject.SetActive(false);
                    }
                }

            }
        }

        void Export(ExportTerrainLODSettings LODSettings)
        {

            List<Terrain> selectedTerrains = new List<Terrain>();

            if (m_exportSelection == ExportSelection.SingleTerrainOnly)
            {
                selectedTerrains.Add(Terrain.activeTerrain);
            }
            else
            {
                selectedTerrains = Terrain.activeTerrains.ToList();
            }

            int maxTerrainCount = selectedTerrains.Count() * m_exportTerrainLODSettings.Count();
            

            foreach (Terrain terrain in selectedTerrains)
            {
                m_currentTerrainCount++;

                EditorUtility.DisplayProgressBar("Exporting Mesh " + m_currentTerrainCount.ToString() + " of " + maxTerrainCount, "Mesh Export", (float)m_currentTerrainCount / (float) maxTerrainCount);
                string objFileName = ExportToObj(terrain, LODSettings);

                string splatMapFileName = "";

                if (LODSettings.m_exportSplatmaps)
                {
                    splatMapFileName = ExportSplatmap(terrain, LODSettings.namePrefix);
                }

                int i = 0;
                //we support up to 32 splatmap files
                while (File.Exists(splatMapFileName + i.ToString() + ".png") && i<32)
                {
                    AssetDatabase.ImportAsset(splatMapFileName + i.ToString() + ".png");
                    i++;
                }



                string textureFileName = "";
                if (LODSettings.m_exportTextures)
                {
                    EditorUtility.DisplayProgressBar("Exporting Mesh " + m_currentTerrainCount.ToString() + " of " + maxTerrainCount, "Texture Export", (float)m_currentTerrainCount / (float)maxTerrainCount);

                    switch (LODSettings.m_textureExportMethod)
                    {
                        case TextureExportMethod.BaseMapExport:
                             textureFileName = ExportBaseMap(terrain, LODSettings.namePrefix);
                            AssetDatabase.ImportAsset(textureFileName);
                            break;
                        case TextureExportMethod.OrthographicBake:
                            textureFileName = GaiaDirectories.GetTerrainMeshExportDirectory() + "/" + LODSettings.namePrefix + terrain.name + "_Baked.png";
                            OrthographicBake.BakeTerrain(terrain, (int)LODSettings.m_textureExportResolution, (int)LODSettings.m_textureExportResolution,  textureFileName, LODSettings.m_bakeLayerMask);
                            AssetDatabase.ImportAsset(textureFileName);
                            break;

                    }

                    //set texture to repeat mode to clamp to reduce seams between meshes
                    var importer = AssetImporter.GetAtPath(textureFileName) as TextureImporter;
                    if (importer != null)
                    {
                        importer.maxTextureSize = (int)LODSettings.m_textureExportResolution;
                        importer.sRGBTexture = false;
                        importer.wrapMode = TextureWrapMode.Clamp;
                        AssetDatabase.ImportAsset(textureFileName);
                    }
                }

                string normalMapFileName = "";

                if (LODSettings.m_exportNormalMaps)
                {
                    normalMapFileName = GaiaDirectories.GetTerrainMeshExportDirectory() + "/" + LODSettings.namePrefix  + terrain.name + "_Normal.png";
                    Texture2D normalMap = GaiaUtils.CalculateNormals(terrain);
                    ImageProcessing.WriteTexture2D(normalMapFileName, normalMap);
                    AssetDatabase.ImportAsset(normalMapFileName, ImportAssetOptions.ForceUpdate);
                    var importer = AssetImporter.GetAtPath(normalMapFileName) as TextureImporter;
                    if (importer != null)
                    {
                        importer.textureType = TextureImporterType.NormalMap;
                        importer.wrapMode = TextureWrapMode.Clamp;
                        AssetDatabase.ImportAsset(normalMapFileName);
                    }

                }


                MeshRenderer meshRenderer = null;

                if (m_setupGameObjects)
                {
                    EditorUtility.DisplayProgressBar("Exporting Mesh " + m_currentTerrainCount.ToString() + " of " + maxTerrainCount, "Creating Game Object", (float)m_currentTerrainCount / (float)maxTerrainCount);
                    Mesh mesh = (Mesh)AssetDatabase.LoadAssetAtPath(objFileName, typeof(Mesh));
                    mesh.RecalculateNormals();
                    GameObject newGO = new GameObject();
                    newGO.name = objFileName.Split('/').Last().Replace(".obj", "");
                    newGO.transform.position = terrain.transform.position;
                    MeshFilter filter = newGO.AddComponent<MeshFilter>();
                    filter.mesh = mesh;
                    meshRenderer = newGO.AddComponent<MeshRenderer>();
                    if (m_exportTerrainLODSettings.Count > 1)
                    {
                        string parentName = "LODMesh" + terrain.name;
                        GameObject LODGroupParent = GameObject.Find(parentName);
                        
                        if (LODGroupParent==null)
                        {
                            LODGroupParent = new GameObject();
                            LODGroupParent.name = parentName;
                            LODGroupParent.transform.position = terrain.transform.position;
                            LODGroupParent.AddComponent<LODGroup>();
                            m_createdLODParents.Add(LODGroupParent);
                        }
                        newGO.transform.parent = LODGroupParent.transform;
                        newGO.transform.localPosition = Vector3.zero;
                    }

                }

                if (LODSettings.m_createMaterials)
                {
                    EditorUtility.DisplayProgressBar("Exporting Mesh " + m_currentTerrainCount.ToString() + " of " + maxTerrainCount, "Creating Material", (float)m_currentTerrainCount / (float)maxTerrainCount);

                    Material mat = new Material(Shader.Find("Standard"));
                    string matFileName = objFileName.Replace(".obj", ".mat");

                    if (LODSettings.m_exportTextures)
                    {
                        Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(textureFileName, typeof(Texture2D));
                        mat.SetTexture("_MainTex", texture);
                    }

                    if (LODSettings.m_exportNormalMaps)
                    {
                        Texture2D normalMap = (Texture2D)AssetDatabase.LoadAssetAtPath(normalMapFileName, typeof(Texture2D));
                        mat.SetTexture("_BumpMap", normalMap);
                        //overemphasize normals on lower resolutions
                        float normalStrength = 1f;
                        switch (LODSettings.m_saveResolution)
                        {
                            case SaveResolution.Sixteenth:
                                normalStrength = 4f;
                                break;
                            case SaveResolution.Eighth:
                                normalStrength = 3f;
                                break;
                            case SaveResolution.Quarter:
                                normalStrength = 2f;
                                break;
                            case SaveResolution.Half:
                                normalStrength = 1f;
                                break;
                            case SaveResolution.Full:
                                normalStrength = 1f;
                                break;
                        }

                        mat.SetFloat("_BumpScale", normalStrength);
                        mat.EnableKeyword("_NORMALMAP");
                    }


                    mat.SetFloat("_Glossiness", 0f);
                    
                    if (m_setupGameObjects)
                    {
                        meshRenderer.material = mat;
                    }

                    AssetDatabase.CreateAsset(mat, matFileName);
                    AssetDatabase.ImportAsset(matFileName, ImportAssetOptions.ForceUpdate);
                }

                if (!m_processedTerrains.Contains(terrain))
                {
                    m_processedTerrains.Add(terrain);
                }
                
            }
            if (LODSettings.m_textureExportMethod == TextureExportMethod.OrthographicBake)
            {
                OrthographicBake.RemoveOrthoCam();
            }
            //EditorUtility.DisplayProgressBar("Saving file to disc.", "This might take a while...", 1f);
            //EditorWindow.GetWindow<ExportTerrain>().Close();
            EditorUtility.ClearProgressBar();
        }

        /// <summary>
        /// Export the selected splatmap texture as a PNG or all
        /// </summary>
        /// <param name="path">Path to save it as</param>
        /// <param name="textureIdx">The texture to save</param>
        public string ExportSplatmap(Terrain terrain, string LODPrefix)
        {
            if (terrain == null)
            {
                Debug.LogError("No terrain, unable to export splatmaps");
                return "";
            }

            string path = GaiaDirectories.GetTerrainMeshExportDirectory() + "/" + LODPrefix + terrain.name + "_SplatMap";

            int width = terrain.terrainData.alphamapWidth;
            int height = terrain.terrainData.alphamapHeight;
            int layers = terrain.terrainData.alphamapLayers;

            float[,,] splatMaps = terrain.terrainData.GetAlphamaps(0, 0, width, height);

            GaiaUtils.CompressToMultiChannelFileImage(splatMaps, path, TextureFormat.RGBA32, true, false);
          
            return path;
        }

        string ExportBaseMap(Terrain terrain, string LODPrefix)
        {
            string fname = GaiaDirectories.GetTerrainMeshExportDirectory() + "/" + LODPrefix + terrain.name + "_BaseMap";
           // fname = Path.Combine(path, PWCommon2.Utils.FixFileName(mgr.PhysicalTerrainArray[tileX, tileZ].name + "_BaseMap"));

            Texture2D[] terrainSplats = terrain.terrainData.alphamapTextures;

            GaiaSplatPrototype[] terrainSplatPrototypes = GaiaSplatPrototype.GetGaiaSplatPrototypes(terrain);
            int width = terrainSplats[0].width;
            int height = terrainSplats[0].height;
            float dimensions = width * height;

            //Get the average colours of the terrain textures by using the highest mip
            Color[] averageSplatColors = new Color[terrainSplatPrototypes.Length];
            for (int protoIdx = 0; protoIdx < terrainSplatPrototypes.Length; protoIdx++)
            {
                GaiaSplatPrototype proto = terrainSplatPrototypes[protoIdx];
                Texture2D tmpTerrainTex = ResizeTexture(proto.texture, TextureFormat.ARGB32, 8, width, height, true, false, false);
                Color[] maxMipColors = tmpTerrainTex.GetPixels(tmpTerrainTex.mipmapCount - 1);
                averageSplatColors[protoIdx] = new Color(maxMipColors[0].r, maxMipColors[0].g, maxMipColors[0].b, maxMipColors[0].a);
            }


            //Create the new texture
            Texture2D colorTex = new Texture2D(width, height, TextureFormat.RGBAFloat, false);
            colorTex.name = terrain.name + "_BaseMap";
            colorTex.wrapMode = TextureWrapMode.Repeat;
            colorTex.filterMode = FilterMode.Bilinear;
            colorTex.anisoLevel = 8;
            float xInv = 1f / width;
            float zInv = 1f / height;
            for (int x = 0; x < width; x++)
            {
                //if (x % 250 == 0)
                //{
                //    EditorUtility.DisplayProgressBar("Baking Textures", "Ingesting terrain basemap : " + terrain.name + "..", (float)(x * width) / dimensions);
                //}

                for (int z = 0; z < height; z++)
                {
                    int splatColorIdx = 0;
                    Color mapColor = Color.black;
                    for (int splatIdx = 0; splatIdx < terrainSplats.Length; splatIdx++)
                    {
                        Texture2D terrainSplat = terrainSplats[splatIdx];
                        Color splatColor;
                        splatColor = terrainSplat.GetPixel(x, z);
                      

                        if (splatColorIdx < averageSplatColors.Length)
                        {
                            mapColor = Color.Lerp(mapColor, averageSplatColors[splatColorIdx++], splatColor.r);
                        }
                        if (splatColorIdx < averageSplatColors.Length)
                        {
                            mapColor = Color.Lerp(mapColor, averageSplatColors[splatColorIdx++], splatColor.g);
                        }
                        if (splatColorIdx < averageSplatColors.Length)
                        {
                            mapColor = Color.Lerp(mapColor, averageSplatColors[splatColorIdx++], splatColor.b);
                        }
                        if (splatColorIdx < averageSplatColors.Length)
                        {
                            mapColor = Color.Lerp(mapColor, averageSplatColors[splatColorIdx++], splatColor.a);
                        }
                        //if (alphaMask != null)
                        //{
                        //    mapColor.a = alphaMask[xInv * x, zInv * z];
                        //}
                        //else
                        //{
                        //mapColor.a = 1f;
                        //}
                    }
                    colorTex.SetPixel(x, z, mapColor);
                }
            }
            colorTex.Apply();

            //EditorUtility.DisplayProgressBar("Baking Textures", "Encoding terrain basemap : " + terrain.name + "..", 0f);

            //Save it
            byte[] content = colorTex.EncodeToPNG();
            fname += ".png";
            File.WriteAllBytes(fname, content);

            //AssetDatabase.ImportAsset(fname);

            //Shut it up
            //EditorUtility.ClearProgressBar();

            return fname;
        }

        /// <summary>
        /// Resize the supplied texture, also handles non rw textures and makes them rm
        /// </summary>
        /// <param name="texture">Source texture</param>
        /// <param name="width">Width of new texture</param>
        /// <param name="height">Height of new texture</param>
        /// <param name="mipmap">Generate mipmaps</param>
        /// <param name="linear">Use linear colour conversion</param>
        /// <returns>New texture</returns>
        public static Texture2D ResizeTexture(Texture2D texture, TextureFormat format, int aniso, int width, int height, bool mipmap, bool linear, bool compress)
        {
            RenderTexture rt;
            if (linear)
            {
                rt = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
            }
            else
            {
                rt = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.sRGB);
            }
            bool prevRgbConversionState = GL.sRGBWrite;
            if (linear)
            {
                GL.sRGBWrite = false;
            }
            else
            {
                GL.sRGBWrite = true;
            }
            Graphics.Blit(texture, rt);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = rt;
            Texture2D newTexture = new Texture2D(width, height, format, mipmap, linear);
            newTexture.name = texture.name + " X";
            newTexture.anisoLevel = aniso;
            newTexture.filterMode = texture.filterMode;
            newTexture.wrapMode = texture.wrapMode;
            newTexture.mipMapBias = texture.mipMapBias;
            newTexture.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            newTexture.Apply(true);

            if (compress)
            {
                newTexture.Compress(true);
                newTexture.Apply(true);
            }

            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(rt);
            GL.sRGBWrite = prevRgbConversionState;
            return newTexture;
        }

        string ExportToObj(Terrain terrain, ExportTerrainLODSettings LODSettings)
        {
            string fileName = GaiaDirectories.GetTerrainMeshExportDirectory() + "/" + LODSettings.namePrefix + terrain.name + ".obj";
            terrainPos = terrain.transform.position;
            int w = terrain.terrainData.heightmapWidth;
            int h = terrain.terrainData.heightmapHeight;
            Vector3 meshScale = terrain.terrainData.size;
            int tRes = (int)Mathf.Pow(2, (int)LODSettings.m_saveResolution);
            meshScale = new Vector3(meshScale.x / (w - 1) * tRes, meshScale.y, meshScale.z / (h - 1) * tRes);
            Vector2 uvScale = new Vector2(1.0f / (w - 1), 1.0f / (h - 1));
            float[,] tData = terrain.terrainData.GetHeights(0, 0, w, h);

            w = (w - 1) / tRes + 1;
            h = (h - 1) / tRes + 1;
            Vector3[] tVertices = new Vector3[w * h];
            Vector2[] tUV = new Vector2[w * h];

            int[] tPolys;

            if (m_saveFormat == SaveFormat.Triangles)
            {
                tPolys = new int[(w - 1) * (h - 1) * 6];
            }
            else
            {
                tPolys = new int[(w - 1) * (h - 1) * 4];
            }

            // Build vertices and UVs
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    tVertices[y * w + x] = Vector3.Scale(meshScale, new Vector3(-y, tData[x * tRes, y * tRes], x));// + terrainPos;
                    tUV[y * w + x] = Vector2.Scale(new Vector2(x * tRes, y * tRes), uvScale);
                }
            }

            int index = 0;
            if (m_saveFormat == SaveFormat.Triangles)
            {
                // Build triangle indices: 3 indices into vertex array for each triangle
                for (int y = 0; y < h - 1; y++)
                {
                    for (int x = 0; x < w - 1; x++)
                    {
                        // For each grid cell output two triangles
                        tPolys[index++] = (y * w) + x;
                        tPolys[index++] = ((y + 1) * w) + x;
                        tPolys[index++] = (y * w) + x + 1;

                        tPolys[index++] = ((y + 1) * w) + x;
                        tPolys[index++] = ((y + 1) * w) + x + 1;
                        tPolys[index++] = (y * w) + x + 1;
                    }
                }
            }
            else
            {
                // Build quad indices: 4 indices into vertex array for each quad
                for (int y = 0; y < h - 1; y++)
                {
                    for (int x = 0; x < w - 1; x++)
                    {
                        // For each grid cell output one quad
                        tPolys[index++] = (y * w) + x;
                        tPolys[index++] = ((y + 1) * w) + x;
                        tPolys[index++] = ((y + 1) * w) + x + 1;
                        tPolys[index++] = (y * w) + x + 1;
                    }
                }
            }

            // Export to .obj
            StreamWriter sw = new StreamWriter(fileName);
            try
            {

                sw.WriteLine("# Unity terrain OBJ File");

                // Write vertices
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                counter = tCount = 0;
                totalCount = (tVertices.Length * 2 + (m_saveFormat == SaveFormat.Triangles ? tPolys.Length / 3 : tPolys.Length / 4)) / progressUpdateInterval;
                for (int i = 0; i < tVertices.Length; i++)
                {
                    //UpdateProgress();
                    StringBuilder sb = new StringBuilder("v ", 20);
                    // StringBuilder stuff is done this way because it's faster than using the "{0} {1} {2}"etc. format
                    // Which is important when you're exporting huge terrains.
                    sb.Append(tVertices[i].x.ToString()).Append(" ").
                        Append(tVertices[i].y.ToString()).Append(" ").
                        Append(tVertices[i].z.ToString());
                    sw.WriteLine(sb);
                }
                // Write UVs
                for (int i = 0; i < tUV.Length; i++)
                {
                    //UpdateProgress();
                    StringBuilder sb = new StringBuilder("vt ", 22);
                    sb.Append(tUV[i].y.ToString()).Append(" ").
                        Append(tUV[i].x.ToString());
                    sw.WriteLine(sb);
                }
                if (m_saveFormat == SaveFormat.Triangles)
                {
                    // Write triangles
                    for (int i = 0; i < tPolys.Length; i += 3)
                    {
                        //UpdateProgress();
                        StringBuilder sb = new StringBuilder("f ", 43);
                        sb.Append(tPolys[i] + 1).Append("/").Append(tPolys[i] + 1).Append(" ").
                            Append(tPolys[i + 1] + 1).Append("/").Append(tPolys[i + 1] + 1).Append(" ").
                            Append(tPolys[i + 2] + 1).Append("/").Append(tPolys[i + 2] + 1);
                        sw.WriteLine(sb);
                    }
                }
                else
                {
                    // Write quads
                    for (int i = 0; i < tPolys.Length; i += 4)
                    {
                        //UpdateProgress();
                        StringBuilder sb = new StringBuilder("f ", 57);
                        sb.Append(tPolys[i] + 1).Append("/").Append(tPolys[i] + 1).Append(" ").
                            Append(tPolys[i + 1] + 1).Append("/").Append(tPolys[i + 1] + 1).Append(" ").
                            Append(tPolys[i + 2] + 1).Append("/").Append(tPolys[i + 2] + 1).Append(" ").
                            Append(tPolys[i + 3] + 1).Append("/").Append(tPolys[i + 3] + 1);
                        sw.WriteLine(sb);
                    }
                }


               
            }
            catch (Exception err)
            {
                Debug.Log("Error saving file: " + err.Message);
            }
            sw.Close();
            AssetDatabase.ImportAsset(fileName);
            return fileName;
        }

        //void UpdateProgress()
        //{
        //    if (counter++ == progressUpdateInterval)
        //    {
        //        counter = 0;
        //        EditorUtility.DisplayProgressBar("Saving...", "", Mathf.InverseLerp(0, totalCount, ++tCount));
        //    }
        //}
    }
}