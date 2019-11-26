// Cristian Pop - https://boxophobic.com/

using UnityEngine;
using UnityEditor;
using System;

namespace Boxophobic
{

    public class BBannerDrawer : MaterialPropertyDrawer
    {

        protected string bannerText;
        protected string bannerSubText;
        protected Color bannerColor;

        protected string title;
        protected string subtitle;

        public BBannerDrawer()
        {
            title = null;
            subtitle = null;
        }

        public BBannerDrawer(string t)
        {
            title = t;
            subtitle = null;
        }

        public BBannerDrawer(string t, string s)
        {
            title = t;
            subtitle = s;
        }

        public override void OnGUI(Rect position, MaterialProperty prop, String label, MaterialEditor materialEditor)
        {

            //EditorGUI.DrawRect(position, new Color(0, 1, 0, 0.05f));

            Material material = materialEditor.target as Material;

            if (title == null && subtitle == null)
            {
                title = prop.displayName;
                subtitle = null;
            }

            bannerText = title;
            bannerSubText = subtitle;

            if (EditorGUIUtility.isProSkin)
            {
                bannerColor = Boxophobic.BConst.ColorLightGray;
            }
            else
            {
                bannerColor = Boxophobic.BConst.ColorDarkGray;
            }

            Boxophobic.BEditorGUI.DrawMaterialBanner(bannerColor, bannerText, bannerSubText, material.shader);

        }

        public override float GetPropertyHeight(MaterialProperty prop, string label, MaterialEditor editor)
        {

            return 0;

        }
    }

}