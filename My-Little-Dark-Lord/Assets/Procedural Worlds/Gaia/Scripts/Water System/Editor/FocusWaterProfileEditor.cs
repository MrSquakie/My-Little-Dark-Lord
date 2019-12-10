using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Gaia
{
    [CustomEditor(typeof(FocusWaterProfile))]
    public class FocusWaterProfileEditor : Editor
    {
        private FocusWaterProfile m_profile;

        private void OnEnable()
        {
            m_profile = (FocusWaterProfile)target;
        }

        public override void OnInspectorGUI()
        {
            if (m_profile == null)
            {
                m_profile = (FocusWaterProfile)target;
            }

            if (GUILayout.Button("Focus Water Profile"))
            {
                m_profile.FocusProfile();
            }
        }
    }
}