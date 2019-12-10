using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Gaia;
using System;
using System.IO;
using PWCommon2;
using Gaia.Pipeline;
using System.Linq;

namespace ProcedualWorlds.Gaia.PackageSystem
{
    public class PackageInstallerUtils
    {
        //Public
        public static bool m_installShaders = false;
        public static float m_timer;
        //Private
        private static List<string> m_foldersToProcess = new List<string>();
        private static Material[] m_vegetationMaterialLibrary;
        private static Material[] m_waterMaterialLibrary;
        private static bool m_installMaterials = false;
        private static float m_progressTimer;
        private static GaiaConstants.EnvironmentRenderer m_renderPipeline;
        private static string m_unityVersion;
        private static Shader m_vegetationShaderToInstall;
        private static Shader m_waterShaderToInstall;
        private static UnityPipelineProfile m_gaiaPipelineProfile;

        //Private const strings
        private const string m_vegetationShaderKeyWord = "Vegetation";
        private const string m_waterShaderKeyWord = "Water";
        private const string m_builtInKeyWord = "SP";
        private const string m_lightweightKeyWord = "LW";
        private const string m_highDefinitionKeyWord = "HD";

        public static void StartInstallation(string unityVersion, GaiaConstants.EnvironmentRenderer renderPipeline, Material[] vegetationMaterialLibrary, Material[] waterMaterialLibrary, UnityPipelineProfile pipelineProfile)
        {
            //Set settings
            m_vegetationMaterialLibrary = vegetationMaterialLibrary;
            m_waterMaterialLibrary = waterMaterialLibrary;
            m_progressTimer = m_timer;
            m_renderPipeline = renderPipeline;
            m_unityVersion = unityVersion;
            m_gaiaPipelineProfile = pipelineProfile;

            //Checks if the material library is empty
            if (vegetationMaterialLibrary.Length == 0)
            {
                Debug.LogError("Vegetation Material Library is empty for the active pipeline and render pipeline. Please check the pipeline profile that it contains the necessary information");
                FinishInstallingPackages();
                return;
            }

            //Checks if the material library is empty
            if (waterMaterialLibrary.Length == 0)
            {
                Debug.LogError("Water Material Library is empty for the active pipeline and render pipeline. Please check the pipeline profile that it contains the necessary information");
                FinishInstallingPackages();
                return;
            }

            //Popup dialog to proceed
            if (EditorUtility.DisplayDialog("Importing Shaders and Materials", "You are about to install new shaders and materials to targeted pipeline and unity version. Please make sure you're using the correct SRP before you proceed. Are you sure you want to proceed?", "Yes", "No"))
            {
                EditorUtility.DisplayProgressBar("Preparing Installation", "Preparing shader directories...", 0.5f);

                StartInstallingPackage();
            }
            else
            {
                //Finish and exit
                FinishInstallingPackages();
            }
        }

        /// <summary>
        /// Start install process
        /// </summary>
        private static void StartInstallingPackage2()
        {
            var manager = EditorWindow.GetWindow<GaiaManagerEditor>(false, "Gaia Manager");
            //Manager can be null if the dependency package installation is started upon opening the manager window.
            if (manager != null)
            {
                manager.Close();
            }
            m_installShaders = false;
            EditorApplication.update += EditorUpdate;
        }

        /// <summary>
        /// Finish and exit installation
        /// </summary>
        private static void FinishInstallingPackages()
        {
            EditorUtility.ClearProgressBar();

            m_installShaders = false;
            m_installMaterials = false;

            m_vegetationShaderToInstall = null;
            m_waterShaderToInstall = null;

            var manager = EditorWindow.GetWindow<GaiaManagerEditor>(false, "Gaia Manager");
            //Manager can be null if the dependency package installation is started upon opening the manager window.
            if (manager != null)
            {
                manager.Show();
            }
        }

        /// <summary>
        /// Start installation
        /// </summary>
        private static void StartInstallingPackage()
        {
            bool updatesChanges = false;
            string shaderRootFolder = GaiaDirectories.GetShaderDirectory();
            if (m_gaiaPipelineProfile == null)
            {
                m_gaiaPipelineProfile = GetPipelineProfile();
            }

            //Add Shader setup here
            string[] folders = Directory.GetDirectories(shaderRootFolder, ".", SearchOption.AllDirectories);
            foreach (string folderName in folders)
            {
                CleanUpFolder(folderName);
            }

            AssetDatabase.Refresh();



            m_unityVersion = Application.unityVersion;
            m_unityVersion = m_unityVersion.Remove(m_unityVersion.LastIndexOf(".")).Replace(".", "_0");
            string keyWordToSearch = "";
            if (m_installShaders)
            {
                if (m_renderPipeline == GaiaConstants.EnvironmentRenderer.BuiltIn)
                {
                    keyWordToSearch = m_builtInKeyWord;
                }
                else if (m_renderPipeline == GaiaConstants.EnvironmentRenderer.Lightweight)
                {
                    keyWordToSearch = m_lightweightKeyWord;
                }
                else
                {
                    keyWordToSearch = m_highDefinitionKeyWord;
                }

                foreach (string folderName in folders)
                {
                    if (!folderName.EndsWith("PWS Functions") && !folderName.EndsWith("PWS Procedural") && !folderName.EndsWith("PWS Water Pro"))
                    {
                        if (folderName.Contains(keyWordToSearch + " " + m_unityVersion))
                        {
                            updatesChanges = RemoveFileSuffix(folderName);
                            if (folderName.Contains("Vegetation"))
                            {
                                m_vegetationShaderToInstall = GetShaderFile(folderName, "Vegetation");
                            }
                            else if (folderName.Contains("Water"))
                            {
                                m_waterShaderToInstall = GetShaderFile(folderName, "Water");
                            }
                        }
                        else
                        {
                            updatesChanges = AddFileSuffix(folderName);
                        }
                    }
                }

                if (updatesChanges)
                {
                    AssetDatabase.Refresh();
                }

                StartInstallingPackage2();
            }

            if (m_installMaterials)
            {
                EditorApplication.update -= EditorUpdate;
           
                if (m_vegetationShaderToInstall == null || m_waterShaderToInstall == null)
                {
                    Debug.LogError("Unable to load the Vegetation or Water shader");
                }
                else
                {
                    //Change all vegetation materials to the correct shader
                    foreach (Material vegetationMaterial in m_vegetationMaterialLibrary)
                    {
                        vegetationMaterial.shader = m_vegetationShaderToInstall;
                        if (m_renderPipeline == GaiaConstants.EnvironmentRenderer.BuiltIn)
                        {
                            vegetationMaterial.SetFloat("_AlphaCutoffBias", 0.7f);
                        }
                        else
                        {
                            vegetationMaterial.SetFloat("_AlphaCutoffBias", 0.3f);
                        }

                        EditorUtility.SetDirty(vegetationMaterial);
                    }
                }

                if (m_waterShaderToInstall == null)
                {
                    Debug.LogError("Unable to load the Water shader");
                }
                else
                {
                    //Change all water materials to the correct shader
                    foreach (Material waterMaterial in m_waterMaterialLibrary)
                    {
                        waterMaterial.shader = m_waterShaderToInstall;
                        EditorUtility.SetDirty(m_waterShaderToInstall);
                    }
                }

                Terrain[] terrains = Terrain.activeTerrains;
                if (terrains != null)
                {
                    foreach (Terrain terrain in terrains)
                    {
                        terrain.UpdateGIMaterials();
                        terrain.Flush();
                    }
                }

                if (updatesChanges)
                {
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }

                FinishInstallingPackages();
            }           
        }

        private static void CleanUpFolder(string folderName)
        {
            if (!folderName.EndsWith("PWS Functions") && !folderName.EndsWith("PWS Procedural") && !folderName.EndsWith("PWS Water Pro"))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(folderName);
                var files = dirInfo.GetFiles();

                bool isSRPFolder = folderName.Contains("Vegetation HD") ||
                                    folderName.Contains("Vegetation LW") ||
                                    folderName.Contains("Vegetation UP") ||
                                    folderName.Contains("Ocean Pro HD") ||
                                    folderName.Contains("Ocean Pro LW") ||
                                    folderName.Contains("Ocean Pro UP");
                List<FileInfo> deletionCandidates = new List<FileInfo>();
                foreach (FileInfo file in files)
                {
                    if (!file.Name.EndsWith("meta"))
                    {
                        List<FileInfo> duplicates = files.Where(x => !x.Name.EndsWith("meta") && x.Name.Remove(x.Name.LastIndexOf(x.Extension)) == file.Name.Remove(file.Name.LastIndexOf(file.Extension))).ToList();
                        if (duplicates.Count() > 1)
                        {
                            foreach (FileInfo duplicateFile in duplicates)
                            {
                                if (isSRPFolder && !duplicateFile.Extension.EndsWith("file"))
                                {
                                    deletionCandidates.Add(duplicateFile);
                                }
                                if (!isSRPFolder && duplicateFile.Extension.EndsWith("file"))
                                {
                                    deletionCandidates.Add(duplicateFile);
                                }
                            }
                        }
                    }
                }

                for (int i = 0; i < deletionCandidates.Count(); i++)
                {
                    FileUtil.DeleteFileOrDirectory(deletionCandidates[i].FullName);
                }
            }
        }

        private static void EditorUpdate()
        {
            m_timer -= Time.deltaTime;
            if (m_timer < 0)
            {
                m_installMaterials = true;
                StartInstallingPackage();
            }
            else
            {
                EditorUtility.DisplayProgressBar("Preparing Materials", "Preparing to upgrade material shaders...", m_progressTimer / m_timer);
            }
        }

        /// <summary>
        /// Removes Suffix in file formats required
        /// </summary>
        /// <param name="path"></param>
        private static bool RemoveFileSuffix(string path)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            var files = dirInfo.GetFiles();
            bool changes = false;
            foreach (FileInfo file in files)
            {
                if (file.Extension.EndsWith("file"))
                {
                    string fileName = file.FullName;
                    File.Move(fileName, fileName.Remove(fileName.Length - 4, 4));
                    changes = true;
                }
            }

            if (changes)
            {
                AssetDatabase.Refresh();
            }

            return changes;
        }

        /// <summary>
        /// Removes Suffix in file formats required
        /// </summary>
        /// <param name="path"></param>
        private static bool AddFileSuffix(string path)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            var files = dirInfo.GetFiles();
            bool changes = false;
            foreach (FileInfo file in files)
            {
                if (!file.Extension.EndsWith("file") && !file.Extension.EndsWith("meta"))
                {
                    string fileName = file.FullName;
                    File.Move(fileName, fileName.Replace(fileName, fileName + "file"));
                    changes = true;
                }
            }

            if (changes)
            {
                AssetDatabase.Refresh();
            }

            return changes;
        }

        /// <summary>
        /// Gets the shader
        /// </summary>
        /// <param name="path"></param>
        /// <param name="seachPattern"></param>
        /// <returns></returns>
        private static Shader GetShaderFile(string path, string seachPattern)
        {
            Shader returningShader = null;
            DirectoryInfo dirInfo = new DirectoryInfo(path);
            var files = dirInfo.GetFiles();
            foreach (FileInfo file in files)
            {
                if (file.Extension.EndsWith("shader") && file.Name.Contains(seachPattern))
                {
                    returningShader = AssetDatabase.LoadAssetAtPath<Shader>(GaiaUtils.GetAssetPath(file.Name));
                    return returningShader;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the pipeline profile
        /// </summary>
        /// <returns></returns>
        private static UnityPipelineProfile GetPipelineProfile()
        {
            UnityPipelineProfile profile = null;

            GaiaSettings settings = GaiaUtils.GetGaiaSettings();
            if (settings != null)
            {
                profile = settings.m_pipelineProfile;
            }

            return profile;
        }
    }
}