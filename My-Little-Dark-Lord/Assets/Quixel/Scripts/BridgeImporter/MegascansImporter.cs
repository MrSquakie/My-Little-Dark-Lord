#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;
using Newtonsoft.Json.Linq;

namespace Quixel
{
    /// <summary>
    /// Completely rewritten importer with lots of commenting... Should be quite a bit faster than the previous one too.
    /// For those looking to customize, please be aware that some of the methods in here do not use built in Unity API calls.
    /// Ajwad Imran - Technical Artist @ Quixel.
    /// Lee Devonald - (Former) Technical Artist @ Quixel.
    /// </summary>
    public class MegascansImporter : Editor
    {
        private bool plant = false;
        private string assetName;
        private string id;
        private string type;
        private int assetType;
        private string mapName;
        private string finalName;
        private string tempTexName;
        private string assetResolution = "";

        private string path;
        private int dispType;
        private int texPack;
        private int lodFadeMode;
        private int shaderType;
        private bool setupCollision = false;
        private bool generateTerrainNormal = false;

        private bool applyToSelection;
        private bool addAssetToScene;

        private string texPath;
        private string matPath;
        private string meshPath;

        private Material finalMat;
        private Material billboardMat;
        private Material hpMat;

        private bool highPoly = false;
        private bool isAlembic = false;
        private bool hasBillboardLOD = false;
        private bool hasBillboardLODOnly = false;

        private JObject mapNames;

        private float currentOperationCount = 0;
        private float maxNumberOfOperations = 0;
        private string progressHeading = "Processing Asset";
        private string progressMessage = "Processing Asset";

        /// <summary>
        /// Takes an imported JSON object, and breaks it into relevant components and data.
        /// Then calls relevant functions for actual import of asset.
        /// </summary>
        /// <param name="objectList"></param>
        public string ImportMegascansAssets(JObject objectList)
        {
            var startTime = System.DateTime.Now;
            string activeLOD = (string)objectList["activeLOD"];
            string minLOD = (string)objectList["minLOD"];

            isAlembic = false;
            plant = false;
            highPoly = false;
            hasBillboardLOD = false;
            hasBillboardLODOnly = false;
            mapName = "";

            //get texture components from the current object.
            JArray textureComps = (JArray)objectList["components"];

            //get mesh components from the current object.
            JArray meshComps = (JArray)objectList["meshList"];
            
            //Map name overrides.
            mapNames = (JObject)objectList["mapNameOverride"];
            //run a check to see if we're using Unity 5 or below, and then if we're trying to import a high poly mesh. if so, let the user know we are aborting the import.
            if (meshComps.Count > 0)
            {
                isAlembic = Path.GetExtension((string)meshComps[0]["path"]) == ".abc";
            }

            hasBillboardLOD = MegascansMeshUtils.ContainsLowestLOD((JArray)objectList["lodList"], minLOD);

            assetType = (int)objectList["meshVersion"];
            type = (string)objectList["type"];
            if (type.ToLower().Contains("3dplant"))
            {
                plant = true;
                if (minLOD == activeLOD)
                {
                    hasBillboardLODOnly = true;
                }
            }

            try {
                LoadPreferences();
                MegascansUtilities.CalculateNumberOfOperations(objectList, dispType, texPack, shaderType, generateTerrainNormal, hasBillboardLODOnly);
                path = ConstructPath(objectList);

                if(path == null || path == "") {
                    Debug.Log("Asset: " + (string)objectList["name"] + " already exist in the Project. Please delete/rename the existing folder and re-import this asset.");
                    AssetDatabase.Refresh();
                    return null;
                }

                GetShaderType();
            } catch (Exception ex) {
                Debug.Log("Error setting import path.");
                Debug.Log(ex.ToString());
                MegascansUtilities.HideProgressBar();
            }

            try {
                //process textures
                ProcessTextures(textureComps);
                if(finalMat == null && !(plant && hasBillboardLODOnly)) {
                    Debug.Log("The import path is incorrect. Asset import aborted.");
                    return null;
                } else
                {
                    if (type.ToLower().Contains("surface") && applyToSelection)
                    {
                        foreach(MeshRenderer render in MegascansUtilities.GetSelectedMeshRenderers())
                        {
                            render.material = finalMat;
                        }
                    }
                }
            } catch (Exception ex) {
                Debug.Log("Error importing textures.");
                Debug.Log(ex.ToString());
                MegascansUtilities.HideProgressBar();
            }
               
            //process meshes
            if (meshComps == null && !type.Contains("surface"))
            {
                Debug.LogError("No meshes found. Please double check your export settings.");
                Debug.Log("Import failed.");
                return null;
            }

            if (meshComps.Count > 0)
            {
                if (activeLOD == "high")
                {
                    //detect if we're trying to import a high poly mesh...
                    string msg = "You are about to import a high poly mesh. \nThese meshes are usually millions of polygons and can cause instability to your project. \nWould you like to proceed?";
                    if (EditorUtility.DisplayDialog("WARNING!", msg, "Yes", "No"))
                    {
#if UNITY_EDITOR_WIN
                        hpMat = new Material(finalMat.shader);
                        hpMat.CopyPropertiesFromMaterial(finalMat);
                        hpMat.SetTexture("_NormalMap", null);
                        hpMat.SetTexture("_BumpMap", null);
                        hpMat.DisableKeyword("_NORMALMAP_TANGENT_SPACE");
                        hpMat.DisableKeyword("_NORMALMAP");
                        hpMat.name = MegascansUtilities.FixSpaces(new string[] { hpMat.name, "HighPoly" });
                        string hpMatDir = MegascansUtilities.FixSpaces(new string[] { matPath, "HighPoly.mat" });
                        AssetDatabase.CreateAsset(hpMat, hpMatDir);
#endif
                        highPoly = true;
                    }
                }
                try {
                    //process meshes and prefabs
                    PrefabData prefData = new PrefabData(path, finalName, lodFadeMode, highPoly, addAssetToScene, setupCollision, hasBillboardLOD, isAlembic, false, finalMat, hpMat, billboardMat, new List<string>(), new List<List<string>>());
                    MegascansMeshUtils.ProcessMeshes(objectList, path, highPoly, plant, prefData);
                } catch (Exception ex) {
                    Debug.Log("Error importing meshes.");
                    Debug.Log(ex.ToString());
                    MegascansUtilities.HideProgressBar();
                }
            }

            var endTime = System.DateTime.Now;
            var totalTime = endTime - startTime;
            Debug.Log("Asset Import Time: " + totalTime);
            AssetDatabase.Refresh();
            MegascansUtilities.HideProgressBar();
            //Application.GarbageCollectUnusedAssets();
            return path;
        }

        #region Texture Processing Methods

        /// <summary>
        /// Process textures from Megascans asset import.
        /// </summary>
        /// <param name="textureComponents"></param>
        /// <returns></returns>
        void ProcessTextures(JArray textureComponents)
        {
            //create a subdirectory for textures.
            texPath = MegascansUtilities.ValidateFolderCreate(path, "Textures");
            texPath += "/" + finalName.Replace(" ", "_").Replace("$lod", "").Replace("$variation", "").Replace("$resolution", assetResolution);

            matPath = MegascansUtilities.ValidateFolderCreate(path, "Materials");
            matPath += "/" + finalName.Replace(" ", "_").Replace("$mapName", "").Replace("$resolution", "").Replace("$lod", "").Replace("$variation", "");

            //Attempt to store all the paths we might need to get our textures.
            //It's quicker to do this, than create an array and loop through it continually using a lot of if-statements later on.
            string albedo = null;
            string opacity = null;
            string normals = null;
            string metallic = null;
            string specular = null;
            string AO = null;
            string gloss = null;
            string displacement = null;
            string roughness = null;
            string translucency = null;

            //Search the JSON array for each texture type, leave it null if it doesn't exist. This is important as we use the null check later.
            for (int i = 0; i < textureComponents.Count; ++i)
            {
                albedo = (string)textureComponents[i]["type"] == "albedo" ? (string)textureComponents[i]["path"] : albedo;
                albedo = (albedo == null && (string)textureComponents[i]["type"] == "diffuse") ? (string)textureComponents[i]["path"] : albedo;
                opacity = (string)textureComponents[i]["type"] == "opacity" ? (string)textureComponents[i]["path"] : opacity;
                normals = (string)textureComponents[i]["type"] == "normal" ? (string)textureComponents[i]["path"] : normals;
                metallic = (string)textureComponents[i]["type"] == "metalness" ? (string)textureComponents[i]["path"] : metallic;
                specular = (string)textureComponents[i]["type"] == "specular" ? (string)textureComponents[i]["path"] : specular;
                AO = (string)textureComponents[i]["type"] == "ao" ? (string)textureComponents[i]["path"] : AO;
                gloss = (string)textureComponents[i]["type"] == "gloss" ? (string)textureComponents[i]["path"] : gloss;
                displacement = (string)textureComponents[i]["type"] == "displacement" ? (string)textureComponents[i]["path"] : displacement;
                roughness = (string)textureComponents[i]["type"] == "roughness" ? (string)textureComponents[i]["path"] : roughness;
                translucency = (string)textureComponents[i]["type"] == "translucency" ? (string)textureComponents[i]["path"] : translucency;
            }

            //make sure we never try to import the high poly normalmap...
            if (normals != null)
            {
                for (int i = 0; i < 6; ++i)
                {
                    string ld = "_LOD" + i.ToString();
                    string n = normals.Replace("Bump", ld);
                    if (File.Exists(n))
                    {
                        normals = n;
                        break;
                    }
                }
            }

            if (!(assetType > 1 && plant && hasBillboardLODOnly))
                finalMat = ReadWriteAllTextures(albedo, opacity, normals, metallic, specular, AO, gloss, displacement, roughness, translucency);

            if (assetType > 1 && plant && hasBillboardLOD)
            {
#if UNITY_EDITOR_WIN
                string[] pathParts = albedo.Split('\\');
#endif
#if UNITY_EDITOR_OSX
                string[] pathParts = albedo.Split('/');
#endif
                string[] nameParts = pathParts[pathParts.Length - 1].Split('_');
                albedo = albedo == null ? null : albedo.Replace("Atlas", "Billboard").Replace(nameParts[0], "Billboard");
                opacity = opacity == null ? null : opacity.Replace("Atlas", "Billboard").Replace(nameParts[0], "Billboard");
                normals = normals == null ? null : normals.Replace("Atlas", "Billboard").Replace(nameParts[0], "Billboard");
                metallic = metallic == null ? null : metallic.Replace("Atlas", "Billboard").Replace(nameParts[0], "Billboard");
                specular = specular == null ? null : specular.Replace("Atlas", "Billboard").Replace(nameParts[0], "Billboard");
                AO = AO == null ? null : AO.Replace("Atlas", "Billboard").Replace(nameParts[0], "Billboard");
                gloss = gloss == null ? null : gloss.Replace("Atlas", "Billboard").Replace(nameParts[0], "Billboard");
                displacement = displacement == null ? null : displacement.Replace("Atlas", "Billboard").Replace(nameParts[0], "Billboard");
                roughness = roughness == null ? null : roughness.Replace("Atlas", "Billboard").Replace(nameParts[0], "Billboard");
                translucency = translucency == null ? null : translucency.Replace("Atlas", "Billboard").Replace(nameParts[0], "Billboard");
                texPath = MegascansUtilities.FixSpaces(new string[] { texPath, "Billboard" });
                matPath = MegascansUtilities.FixSpaces(new string[] { matPath, "Billboard" });
                billboardMat = ReadWriteAllTextures(albedo, opacity, normals, metallic, specular, AO, gloss, displacement, roughness, translucency);
            }
        }

        /// <summary>
        /// Creates materials needed for the asset.
        /// </summary>
        /// <returns></returns>
        Material CreateMaterial()
        {
            MegascansUtilities.UpdateProgressBar(1.0f, "Processing Asset " + assetName, "Creating material...");
            if ((shaderType == 0 || shaderType == 1) && isAlembic)
            {
                Debug.Log("Alembic files are not supported in LWRP/HDRP. Please change your export file format in Bridge or change your SRP in Unity.");
                return null;
            }

            try {
                string rp = matPath + ".mat";
                Material mat = (Material)AssetDatabase.LoadAssetAtPath(rp, typeof(Material));
                if (!mat)
                {
                    mat = new Material(Shader.Find("Standard"));
                    AssetDatabase.CreateAsset(mat, rp);
                    AssetDatabase.Refresh();
                    if (shaderType < 1)
                    {
                        mat.shader = Shader.Find("HDRenderPipeline/Lit");
#if UNITY_2018_3 || UNITY_2019
                        mat.shader = Shader.Find("HDRP/Lit");
#endif
                        mat.SetInt("_DisplacementMode", dispType);
                    }
                    if (shaderType > 0)
                    {
                        if (MegascansUtilities.isLegacy())
                        {
                            mat.shader = Shader.Find("LightweightPipeline/Standard (Physically Based)");
                        } else
                        {
                            mat.shader = Shader.Find("Lightweight Render Pipeline/Lit");
                        }
                    }
                    if (shaderType > 1)
                    {
                        if(isAlembic)
                        {
                            mat.shader = Shader.Find("Alembic/Standard");
                            if (texPack > 0)
                            {
                                mat.shader = Shader.Find("Alembic/Standard (Specular setup)");
                            }
                        } else
                        {
                            mat.shader = Shader.Find("Standard");
                            if (texPack > 0)
                            {
                                mat.shader = Shader.Find("Standard (Specular setup)");
                            }
                        }
                    }
                }
                return mat;
            } catch (Exception ex) {
                Debug.Log("Exception: " + ex.ToString());
                MegascansUtilities.HideProgressBar();
                return null;
            }
        }

        /// <summary>
        /// Previous version of the importer would loop through a list of texture paths, and use a bunch of if-statements and do things accordingly.
        /// This version just takes in every texture path and if it's not null, does the thing. Less looping, better overall performance.
        /// </summary>
        /// <param name="albedo"></param>
        /// <param name="opacity"></param>
        /// <param name="normals"></param>
        /// <param name="metallic"></param>
        /// <param name="specular"></param>
        /// <param name="AO"></param>
        /// <param name="gloss"></param>
        /// <param name="displacement"></param>
        /// <param name="roughness"></param>
        /// <param name="translucency"></param>
        Material ReadWriteAllTextures(string albedo, string opacity, string normals, string metallic, string specular, string AO, string gloss, string displacement, string roughness, string translucency)
        {
            try {
                Material mat = CreateMaterial();

                if(mat == null) {
                    return null;
                }

                //create a new work thread for each texture to be processed.
                //Pack the opacity into the alpha channel of albedo if it exists.
                string texMapName = (string)mapNames["Albedo"];
                tempTexName = texPath.Contains("$mapName")? texPath.Replace("$mapName", texMapName): texPath + texMapName;
                string p = tempTexName + ".png";
                mapName = opacity != null ? "Albedo + Alpha" : "Albedo";
                MegascansUtilities.UpdateProgressBar(1.0f, "Processing Asset " + assetName, "Importing texture: " + mapName);
                Texture2D tex = MegascansImageUtils.PackTextures(albedo, opacity, p);
            
                mat.SetTexture("_MainTex", tex);
                mat.SetTexture("_BaseColorMap", tex);

                if (shaderType == 1)
                {
                    mat.SetTexture("_BaseMap", tex);
                    mat.SetColor("_BaseColor", Color.white);
                }

                if (opacity != null)
                {
                    if (shaderType > 0)
                    {
                        mat.SetFloat("_AlphaClip", 1);
                        mat.SetFloat("_Mode", 1);
                        mat.SetFloat("_Cull", 1);
                        mat.EnableKeyword("_ALPHATEST_ON");
                    }
                    else
                    {
                        mat.SetInt("_AlphaCutoffEnable", 1);
                        mat.SetFloat("_AlphaCutoff", 0.333f);
                        mat.SetInt("_DoubleSidedEnable", 1);

                        mat.SetOverrideTag("RenderType", "TransparentCutout");
                        mat.SetInt("_ZTestGBuffer", (int)UnityEngine.Rendering.CompareFunction.Equal);
                        mat.SetInt("_CullMode", (int)UnityEngine.Rendering.CullMode.Off);
                        mat.SetInt("_CullModeForward", (int)UnityEngine.Rendering.CullMode.Back);
                        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                        mat.SetInt("_ZWrite", 1);
                        mat.renderQueue = 2450;
                        mat.SetInt("_ZTestGBuffer", (int)UnityEngine.Rendering.CompareFunction.Equal);

                        mat.EnableKeyword("_ALPHATEST_ON");
                        mat.EnableKeyword("_DOUBLESIDED_ON");
                        mat.DisableKeyword("_BLENDMODE_ALPHA");
                        mat.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
                    }
                }

                //test to see if gloss is absent but roughness is present...
                bool useRoughness = (gloss == null && roughness != null);
                if (texPack < 1 || shaderType < 1)
                {
                    mapName = "Masks";
                    MegascansUtilities.UpdateProgressBar(1.0f, "Processing Asset " + assetName, "Importing texture: " + mapName);
                    tempTexName = texPath.Contains("$mapName") ? texPath.Replace("$mapName", "Masks"): texPath + "Masks";
                    p = tempTexName + ".png";
                    mat.SetFloat("_Metallic", 1.0f);
                    tex = MegascansImageUtils.PackTextures(metallic, AO, displacement, useRoughness ? roughness : gloss, p, useRoughness);
                    mat.SetTexture("_MaskMap", tex);
                    mat.EnableKeyword("_MASKMAP");
                    mat.SetFloat("_MaterialID", 1);
                    mat.SetTexture("_MetallicGlossMap", tex); 
                    mat.EnableKeyword("_METALLICSPECGLOSSMAP");
                    mat.EnableKeyword("_METALLICGLOSSMAP");
                }
                
                //do we need to process a specular map?
                if (texPack > 0 && specular != null)
                {
                    texMapName = (string)mapNames["Specular"];
                    mapName = "Specular + Gloss";
                    MegascansUtilities.UpdateProgressBar(1.0f, "Processing Asset " + assetName, "Importing texture: " + mapName);
                    tempTexName = texPath.Contains("$mapName") ? texPath.Replace("$mapName", texMapName): texPath + texMapName;
                    p = tempTexName + ".png";
                    tex = MegascansImageUtils.PackTextures(specular, useRoughness ? roughness : gloss, p, useRoughness);
                    mat.SetTexture("_SpecGlossMap", tex);
                    mat.SetColor("_SpecColor", new UnityEngine.Color(1.0f, 1.0f, 1.0f));
                    mat.SetColor("_SpecularColor", new UnityEngine.Color(1.0f, 1.0f, 1.0f));
                    mat.SetFloat("_WorkflowMode", 0);
                    mat.SetFloat("_MaterialID", 4);
                    mat.SetTexture("_SpecularColorMap", tex);
                    mat.EnableKeyword("_METALLICSPECGLOSSMAP");
                    mat.EnableKeyword("_SPECGLOSSMAP");
                    mat.EnableKeyword("_SPECULAR_SETUP");
                    mat.EnableKeyword("_SPECULARCOLORMAP");
                    mat.EnableKeyword("_MATERIAL_FEATURE_SPECULAR_COLOR");
                }

                //handle any textures which can just be converted in place.
                if (normals != null)
                {
                    texMapName = (string)mapNames["Normal"];
                    mapName = "Normals";
                    MegascansUtilities.UpdateProgressBar(1.0f, "Processing Asset " + assetName, "Importing texture: " + mapName);
                    tempTexName = texPath.Contains("$mapName") ? texPath.Replace("$mapName", texMapName): texPath + texMapName;
                    p = tempTexName + ".png";
                    MegascansImageUtils.CreateTexture(normals, p);
                    tex = MegascansImageUtils.TextureImportSetup(p, true, false);
                    mat.SetTexture("_BumpMap", tex);
                    mat.SetTexture("_NormalMap", tex);
                    mat.EnableKeyword("_NORMALMAP_TANGENT_SPACE");
                    mat.EnableKeyword("_NORMALMAP");
                    
                    mapName = "Normals_Terrain";
                    tempTexName = texPath.Contains("$mapName") ? texPath.Replace("$mapName", texMapName + "_Terrain"): texPath + texMapName + "_Terrain";
                    p = tempTexName + ".png";

                    if(generateTerrainNormal && type.ToLower().Contains("surface"))
                    {
                        MegascansImageUtils.ImportTerrainNormal(normals, p);
                    }
                }

                if (displacement != null && dispType > 0)
                {
                    texMapName = (string)mapNames["Displacement"];
                    mapName = "Displacement";
                    MegascansUtilities.UpdateProgressBar(1.0f, "Processing Asset " + assetName, "Importing texture: " + mapName);
                    tempTexName = texPath.Contains("$mapName") ? texPath.Replace("$mapName", texMapName): texPath + texMapName;
                    p = tempTexName + ".png";
                    MegascansImageUtils.CreateTexture(displacement, p);
                    tex = MegascansImageUtils.TextureImportSetup(p, false, false);
                    mat.SetTexture("_HeightMap", tex);
                    mat.SetTexture("_ParallaxMap", tex);
                    mat.EnableKeyword("_DISPLACEMENT_LOCK_TILING_SCALE");
                    if (dispType == 1)
                    {
                        mat.EnableKeyword("_VERTEX_DISPLACEMENT");
                        mat.EnableKeyword("_VERTEX_DISPLACEMENT_LOCK_OBJECT_SCALE");
                    }
                    if (dispType == 2)
                    {
                        mat.EnableKeyword("_PIXEL_DISPLACEMENT");
                        mat.EnableKeyword("_PIXEL_DISPLACEMENT_LOCK_OBJECT_SCALE");
                    }
                }

                //occlusion may or may not need to be packed, depending on the shader used.
                if (shaderType > 1 && AO != null)
                {
                    texMapName = (string)mapNames["AO"];
                    mapName = "AO";
                    MegascansUtilities.UpdateProgressBar(1.0f, "Processing Asset " + assetName, "Importing texture: " + mapName);
                    tempTexName = texPath.Contains("$mapName") ? texPath.Replace("$mapName", texMapName): texPath + texMapName;
                    p = tempTexName + ".png";
                    MegascansImageUtils.CreateTexture(AO, p);
                    tex = MegascansImageUtils.TextureImportSetup(p, false, false);
                    mat.SetTexture("_OcclusionMap", tex);
                    mat.EnableKeyword("_OCCLUSIONMAP");
                }

                if (translucency != null)
                {
                    texMapName = (string)mapNames["Translucency"];
                    mapName = "Translucency";
                    MegascansUtilities.UpdateProgressBar(1.0f, "Processing Asset " + assetName, "Importing texture: " + mapName);
                    tempTexName = texPath.Contains("$mapName") ? texPath.Replace("$mapName", texMapName): texPath + texMapName;
                    p = tempTexName + ".png";
                    tex = MegascansImageUtils.PackTextures(translucency, translucency, translucency, null, p);
                    mat.SetInt("_MaterialID", 0);
                    mat.SetInt("_DiffusionProfile", 1);
                    mat.SetFloat("_EnableSubsurfaceScattering", 1);
                    mat.SetTexture("_SubsurfaceMaskMap", tex);
                    mat.SetTexture("_ThicknessMap", tex);
                    if (plant)
                    {
                        mat.SetInt("_DiffusionProfile", 2);
                        mat.SetFloat("_CoatMask", 0.0f);
                        mat.SetInt("_EnableWind", 1);
                        mat.EnableKeyword("_VERTEX_WIND");
                    }
                    mat.EnableKeyword("_SUBSURFACE_MASK_MAP");
                    mat.EnableKeyword("_THICKNESSMAP");
                    mat.EnableKeyword("_MATERIAL_FEATURE_TRANSMISSION");
                }
                return mat;
            } catch (Exception ex) {
                Debug.Log("Exception: " + ex.ToString());
                MegascansUtilities.HideProgressBar();
                return null;
            }
        }
        #endregion

        #region Formatting Utilities

        /// <summary>
        /// Gets importer settings stored in app registry.
        /// Without this, the asset would have incorrect pathing, and would not be able to create the correct materials etc.
        /// </summary>
        void LoadPreferences()
        {
            path = MegascansUtilities.FixPath(EditorPrefs.GetString("QuixelDefaultPath", "Quixel/Megascans/"));
            dispType = EditorPrefs.GetInt("QuixelDefaultDisplacement");
            texPack = EditorPrefs.GetInt("QuixelDefaultTexPacking");
            shaderType = EditorPrefs.GetInt("QuixelDefaultShader");
            lodFadeMode = EditorPrefs.GetInt("QuixelDefaultLodFadeMode", 1);
            setupCollision = EditorPrefs.GetBool("QuixelDefaultSetupCollision", true);
            generateTerrainNormal = EditorPrefs.GetBool("QuixelDefaultTerrainNormal", false);
            applyToSelection = EditorPrefs.GetBool("QuixelDefaultApplyToSelection", false);
            addAssetToScene = EditorPrefs.GetBool("QuixelDefaultAddAssetToScene", false);
        }

        /// <summary>
        /// Returns the final directory for our asset, creating subfolders where necessary in the 'Assets' directory.
        /// </summary>
        /// <param name="jsonPath"></param>
        /// <returns></returns>
        string ConstructPath(JObject objectList) {
            ///
            /// Unity doesn't allow you to create objects in directories which don't exist.
            /// So in this function, we create any and all necessary subdirectories that are required.
            /// We return the final subdirectory, which is used later in the asset creation too.
            /// A ton of project path validation
            /// Make sure path is "Assets/...." not "D:/Unity Projects/My Project/Assets/...." otherwise the AssetDatabase cannot write files to it.
            /// Lastly I also match the path with the Application DataPath in order to make sure this is the right path selected from the Bridge.
            ///

            AssetDatabase.Refresh();

            string defPath = "";
            bool addNextPathPart = false;

            if ((string)objectList["exportPath"] != "") {
                path = (string)objectList["exportPath"];
            } else
            {
                defPath = "Assets";
                addNextPathPart = true;
            }

            string[] pathParts = MegascansUtilities.FixSlashes(path).Split('/');
            
            List<string> finalPathParts = new List<string> ();
            
            foreach(string part in pathParts){
                if(part == "Assets" && !addNextPathPart) {
                    addNextPathPart = true;
                }

                if(addNextPathPart) {
                    finalPathParts.Add(part);
                }
            }

            if(!addNextPathPart) {
                return null;
            }

            //First, create the user specified path from the importer settings.
            
            if (finalPathParts.Count > 0) {
                for (int i = 0; i < finalPathParts.Count; i++) {
                    defPath = MegascansUtilities.ValidateFolderCreate(defPath, finalPathParts[i]); //FixSlashes(Path.Combine(defPath, finalPathParts[i]));//ValidateFolderCreate(defPath, finalPathParts[i]);
                }
            }

            if (!AssetDatabase.IsValidFolder(defPath)) {
                return null;
            }
            
            //then create check to see if the asset type subfolder exists, create it if it doesn't.
            defPath = MegascansUtilities.ValidateFolderCreate(defPath, MegascansUtilities.GetAssetType((string)objectList["path"]));

            GetAssetFolderName(objectList);
            string finalFolderName = finalName.Replace("$mapName", "").Replace("$resolution", "").Replace("$lod", "").Replace("$variation", "");
            defPath = MegascansUtilities.ValidateFolderCreate(defPath, finalFolderName);
            return defPath;
        }

        /// <summary>
        /// This function attempts to create a folder name
        /// </summary>
        void GetAssetFolderName(JObject objectList)
        {
            try {
                assetResolution = "";
                string namingConvention = (string)objectList["namingConvention"];
                finalName = namingConvention;
                if (namingConvention != "" && namingConvention != null){

                    if(namingConvention.Contains("$id"))
                    {
                        finalName = finalName.Replace("$id",(string)objectList["id"]);
                    }

                    if (namingConvention.Contains("$name"))
                    {
                        finalName = finalName.Replace("$name", (string)objectList["name"]);
                    }

                    if (namingConvention.Contains("$type"))
                    {
                        finalName = finalName.Replace("$type", (string)objectList["type"]);
                    }

                    if (namingConvention.Contains("$resolution"))
                    {
                        assetResolution = (string)objectList["resolution"];
                    }
                    return;
                }
            } catch(Exception ex) {
                Debug.Log(ex);
                MegascansUtilities.HideProgressBar();
            }

            //then create a unique subfolder for the asset.
            assetName = (string)objectList["name"];
            id = (string)objectList["id"];
            finalName = MegascansUtilities.FixSpaces(new string[] {assetName.Replace(" ", "_"), id});
            Debug.Log("Final name:" + finalName);
        }

        /// <summary>
        /// This function attempts to auto-detect which template the project is using. Defaults to Legacy/Standard if all else fails.
        /// </summary>
        void GetShaderType()
        {
            shaderType = EditorPrefs.GetInt("QuixelDefaultShader");
            if (shaderType == 3)
            {
                //attempt to auto-detect a settings file for Lightweight or HD pipelines
                switch (MegascansUtilities.getCurrentPipeline())
                {
                    case Pipeline.HDRP:
                        shaderType = 0;
                        break;
                    case Pipeline.LWRP:
                        shaderType = 1;
                        break;
                    case Pipeline.Standard:
                        shaderType = 2;
                        break;
                    default:
                        shaderType = 2;
                        break;
                }
            }
        }

        #endregion
    }
}
#endif
