// Advanced Dynamic Shaders
// Cristian Pop - https://boxophobic.com/

#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Boxophobic;

[HelpURL("https://docs.google.com/document/d/1PG_9bb0iiFGoi_yQd8IX0K3sMohuWqyq6_qa4AyVpFw/edit#heading=h.19cm1zdplgma")]
[DisallowMultipleComponent]
[ExecuteInEditMode]
public class ADSMotionHelper : MonoBehaviour
{
    
    [BCategory("Primary Motion")]
    public string category_Layer1;

    [BInteractive(1)]
    public int refreshed1 = 0;

    public float primaryMotionAmplitude = 0.0f;
    public float primaryMotionSpeed = 0.0f;
    public float primaryMotionScale = 0.0f;
    public float primaryMotionVariation = 0.0f;
    [Range(0.0f, 1.0f)]
    public float primaryMotionVertical = 0.0f;

    [BCategory("Secundary Motion")]
    public int category_Layer2 = 0;

    [BInteractive(1)]
    public int refreshed2 = 0;

    public float secundaryMotionAmplitude = 0.0f;
    public float secundaryMotionSpeed = 0.0f;
    public float secundaryMotionScale = 0.0f;
    public float secundaryMotionVariation = 0.0f;
    [Range(0.0f, 1.0f)]
    public float secundaryMotionVertical = 0.0f;

    [BCategory("Flutter Motion")]
    public int category_Layer3 = 0;

    [BInteractive(1)]
    public int refreshed3 = 0;

    public float flutterMotionAmplitude = 0.0f;
    public float flutterMotionSpeed = 0.0f;
    public float flutterMotionScale = 0.0f;
    //public float flutterMotionVariation = 0.0f;
    //[Range(0.0f, 1.0f)]
    //public float flutterMotionVertical = 0.0f;

    [BInteractive("ON")]
    public int Interactive_EnableGUI;

    [HideInInspector]
    public bool update = false;

    List<Material> allMaterials;

    void Awake()
    {

        update = false;

        refreshed1 = 0;
        refreshed2 = 0;
        refreshed3 = 0;

        GetAllMaterials();
        GetMaterialProperties();

    }

    void Update()
    {

        if (Application.isPlaying)
        {
            return;
        }

        if (Selection.Contains(gameObject) == false)
        {
            update = false;

            refreshed1 = 0;
            refreshed2 = 0;
            refreshed3 = 0;
        }

        if (update)
        {
            refreshed1 = 1;
            refreshed2 = 1;
            refreshed3 = 1;

            SetMaterialProperties();
        }

        

    }

    void OnDestroy()
    {

        GetAllMaterials();

        for (int i = 0; i < allMaterials.Count; i++)
        {
            allMaterials[i].SetFloat("_Internal_SetByScript", 0.0f);
        }

    }

    public void GetAllMaterials()
    {

        allMaterials = new List<Material>();

        if (gameObject.GetComponent<Renderer>() != false)
        {
            var subMaterials = gameObject.GetComponent<Renderer>().sharedMaterials;
            for (int i = 0; i < subMaterials.Length; i++)
            {
                allMaterials.Add(subMaterials[i]);
            }
        }

        Renderer[] children;
        children = GetComponentsInChildren<Renderer>();

        for (int i = 0; i < children.Length; i++)
        {
            var subMaterials = children[i].sharedMaterials;
            for (int m = 0; m < subMaterials.Length; m++)
            {
                allMaterials.Add(subMaterials[m]);
            }
        }

    }

    public void GetMaterialProperties()
    {

        for (int i = 0; i < allMaterials.Count; i++)
        {
            if (allMaterials[i].shader.name.Contains("Advanced Dynamic Shaders"))
            {
                if (allMaterials[i].HasProperty("_MotionAmplitude"))
                {
                    primaryMotionAmplitude = allMaterials[i].GetFloat("_MotionAmplitude");
                }
                if (allMaterials[i].HasProperty("_MotionSpeed"))
                {
                    primaryMotionSpeed = allMaterials[i].GetFloat("_MotionSpeed");
                }
                if (allMaterials[i].HasProperty("_MotionScale"))
                {
                    primaryMotionScale = allMaterials[i].GetFloat("_MotionScale");
                }
                if (allMaterials[i].HasProperty("_MotionVariation"))
                {
                    primaryMotionVariation = allMaterials[i].GetFloat("_MotionVariation");
                }
                if (allMaterials[i].HasProperty("_MotionVertical"))
                {
                    primaryMotionVertical = allMaterials[i].GetFloat("_MotionVertical");
                }

                if (allMaterials[i].HasProperty("_MotionAmplitude2") && allMaterials[i].GetFloat("_MotionAmplitude2") != 0)
                {
                    secundaryMotionAmplitude = allMaterials[i].GetFloat("_MotionAmplitude2");
                }
                if (allMaterials[i].HasProperty("_MotionSpeed2") && allMaterials[i].GetFloat("_MotionAmplitude2") != 0)
                {
                    secundaryMotionSpeed = allMaterials[i].GetFloat("_MotionSpeed2");
                }
                if (allMaterials[i].HasProperty("_MotionScale2") && allMaterials[i].GetFloat("_MotionAmplitude2") != 0)
                {
                    secundaryMotionScale = allMaterials[i].GetFloat("_MotionScale2");
                }
                if (allMaterials[i].HasProperty("_MotionVariation2") && allMaterials[i].GetFloat("_MotionAmplitude2") != 0)
                {
                    secundaryMotionVariation = allMaterials[i].GetFloat("_MotionVariation2");
                }
                if (allMaterials[i].HasProperty("_MotionVertical2") && allMaterials[i].GetFloat("_MotionAmplitude2") != 0)
                {
                    secundaryMotionVertical = allMaterials[i].GetFloat("_MotionVertical2");
                }

                if (allMaterials[i].HasProperty("_MotionAmplitude3") && allMaterials[i].GetFloat("_MotionAmplitude3") != 0)
                {
                    flutterMotionAmplitude = allMaterials[i].GetFloat("_MotionAmplitude3");
                }
                if (allMaterials[i].HasProperty("_MotionSpeed3") && allMaterials[i].GetFloat("_MotionSpeed3") != 0)
                {
                    flutterMotionSpeed = allMaterials[i].GetFloat("_MotionSpeed3");
                }
                if (allMaterials[i].HasProperty("_MotionScale3") && allMaterials[i].GetFloat("_MotionScale3") != 0)
                {
                    flutterMotionScale = allMaterials[i].GetFloat("_MotionScale3");
                }
                //if (allMaterials[i].HasProperty("_MotionVariation3") && allMaterials[i].GetFloat("_MotionVariation3") != 0)
                //{
                //    flutterMotionVariation = allMaterials[i].GetFloat("_MotionVariation3");
                //}
                //if (allMaterials[i].HasProperty("_MotionVertical3") && allMaterials[i].GetFloat("_MotionVertical3") != 0)
                //{
                //    flutterMotionVertical = allMaterials[i].GetFloat("_MotionVertical3");
                //}
            }
        }

    }

    void SetMaterialProperties()
    {
        for (int i = 0; i < allMaterials.Count; i++)
        {
            allMaterials[i].SetFloat("_Internal_SetByScript", 1.0f);

            if (allMaterials[i].HasProperty("_MotionAmplitude"))
            {
                allMaterials[i].SetFloat("_MotionAmplitude", primaryMotionAmplitude);
            }
            if (allMaterials[i].HasProperty("_MotionSpeed"))
            {
                allMaterials[i].SetFloat("_MotionSpeed", primaryMotionSpeed);
            }
            if (allMaterials[i].HasProperty("_MotionScale"))
            {
                allMaterials[i].SetFloat("_MotionScale", primaryMotionScale);
            }
            if (allMaterials[i].HasProperty("_MotionVariation"))
            {
                allMaterials[i].SetFloat("_MotionVariation", primaryMotionVariation);
            }
            if (allMaterials[i].HasProperty("_MotionVertical"))
            {
                allMaterials[i].SetFloat("_MotionVertical", primaryMotionVertical);
            }

            if (allMaterials[i].HasProperty("_MotionAmplitude2"))
            {
                allMaterials[i].SetFloat("_MotionAmplitude2", secundaryMotionAmplitude);
            }
            if (allMaterials[i].HasProperty("_MotionSpeed2"))
            {
                allMaterials[i].SetFloat("_MotionSpeed2", secundaryMotionSpeed);
            }
            if (allMaterials[i].HasProperty("_MotionScale2"))
            {
                allMaterials[i].SetFloat("_MotionScale2", secundaryMotionScale);
            }
            if (allMaterials[i].HasProperty("_MotionVariation2"))
            {
                allMaterials[i].SetFloat("_MotionVariation2", secundaryMotionVariation);
            }
            if (allMaterials[i].HasProperty("_MotionVertical2"))
            {
                allMaterials[i].SetFloat("_MotionVertical2", secundaryMotionVertical);
            }

            if (allMaterials[i].HasProperty("_MotionAmplitude3"))
            {
                allMaterials[i].SetFloat("_MotionAmplitude3", flutterMotionAmplitude);
            }
            if (allMaterials[i].HasProperty("_MotionSpeed3"))
            {
                allMaterials[i].SetFloat("_MotionSpeed3", flutterMotionSpeed);
            }
            if (allMaterials[i].HasProperty("_MotionScale3"))
            {
                allMaterials[i].SetFloat("_MotionScale3", flutterMotionScale);
            }
            //if (allMaterials[i].HasProperty("_MotionVariation3"))
            //{
            //    allMaterials[i].SetFloat("_MotionVariation3", flutterMotionVariation);
            //}
            //if (allMaterials[i].HasProperty("_MotionVertical3"))
            //{
            //    allMaterials[i].SetFloat("_MotionVertical3", flutterMotionVertical);
            //}
        }

    }

    public void DestroyComponent()
    {
        DestroyImmediate(this);
    }
}
#endif