using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Gaia;
using System;
using System.IO;
using PWCommon2;

namespace ProcedualWorlds.Gaia.PackageSystem
{
    public class PackageInstallerUtils
    {
        //Public
        public static bool m_installPackage1 = false;
        public static float m_timer;
        //Private
        private static string m_package1;
        private static string m_package2;
        private static bool m_installPackage2 = false;
        private static float m_progressTimer;


        public static void StartInstallation(string package1, string package2)
        {
            m_package1 = AssetUtils.GetAssetPath(package1 + ".unitypackage");
            m_package2 = AssetUtils.GetAssetPath(package2 + ".unitypackage");
            m_progressTimer = m_timer;

            if (string.IsNullOrEmpty(m_package1))
            {
                Debug.LogError(package1 + " Could not be found... Exiting");
                FinishInstallingPackages();
                return;
            }

            if (string.IsNullOrEmpty(m_package2))
            {
                Debug.LogError(package2 + " Could not be found... Exiting");
                FinishInstallingPackages();
                return;
            }

            if (EditorUtility.DisplayDialog("Importing Packages", "You're about to import " + m_package1 + " and " + m_package2 + " Are you sure you want to proceed?", "Yes", "No"))
            {
                EditorUtility.DisplayProgressBar("Preparing Packages", "Preparing water package...", 0.5f);
                StartInstallingPackage();
            }
        }

        private static void StartInstallingPackage2()
        {
            m_installPackage1 = false;
            EditorApplication.update += EditorUpdate;
        }

        private static void FinishInstallingPackages()
        {
            EditorUtility.ClearProgressBar();

            m_installPackage1 = false;
            m_installPackage2 = false;
        }

        private static void StartInstallingPackage()
        {
            if (m_installPackage1)
            {
                if (!string.IsNullOrEmpty(m_package1))
                {
                    //EditorUtility.DisplayProgressBar("Installing Packages", "Installing water package...", 0.5f);
                    AssetDatabase.ImportPackage(m_package1, false);
                    AssetDatabase.SaveAssets();
                    StartInstallingPackage2();
                }
                else
                {
                    Debug.LogError("Package " + m_package1 + " could not be found in your project");
                }
            }

            if (m_installPackage2)
            {
                EditorApplication.update -= EditorUpdate;
                if (!string.IsNullOrEmpty(m_package2))
                {
                    //EditorUtility.DisplayProgressBar("Installing Packages", "Installing vegetation package...", 0.85f);
                    AssetDatabase.ImportPackage(m_package2, false);
                    AssetDatabase.SaveAssets();
                    FinishInstallingPackages();
                }
                else
                {
                    Debug.LogError("Package " + m_package2 + " could not be found in your project");
                }
            }           
        }

        private static void EditorUpdate()
        {
            m_timer -= Time.deltaTime;
            if (m_timer < 0)
            {
                m_installPackage2 = true;
                StartInstallingPackage();
            }
            else
            {
                EditorUtility.DisplayProgressBar("Preparing Packages", "Preparing vegetation package...", m_progressTimer / m_timer);
            }
        }
    }
}