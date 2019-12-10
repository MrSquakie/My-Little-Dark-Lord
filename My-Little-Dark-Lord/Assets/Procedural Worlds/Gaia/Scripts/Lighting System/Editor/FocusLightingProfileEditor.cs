using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Gaia
{
    [CustomEditor(typeof(FocusLightingProfile))]
    public class FocusLightingProfileEditor : Editor
    {
        private FocusLightingProfile m_profile;

        private void OnEnable()
        {
            m_profile = (FocusLightingProfile)target;
        }

        public override void OnInspectorGUI()
        {
            if (m_profile == null)
            {
                m_profile = (FocusLightingProfile)target;
            }

            if (GUILayout.Button("Focus Lighting Profile"))
            {
                m_profile.FocusProfile();
            }
        }
    }
}