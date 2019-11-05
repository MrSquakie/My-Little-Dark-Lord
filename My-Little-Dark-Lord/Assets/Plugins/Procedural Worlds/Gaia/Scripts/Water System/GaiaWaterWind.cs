using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gaia
{
    [ExecuteAlways]
    public class GaiaWaterWind : MonoBehaviour
    {
        [Header("Wind Settings")]
        [HideInInspector]
        public WindZone m_windZone;
        [HideInInspector]
        public Material m_waterMaterial;

        private void Start()
        {
            if (m_windZone == null)
            {
                m_windZone = FindObjectOfType<WindZone>();
            }

            if (m_waterMaterial == null)
            {
                m_waterMaterial = gameObject.GetComponent<MeshRenderer>().sharedMaterial;
            }
        }

        private void Update()
        {
            if (m_windZone != null)
            {
                if (m_waterMaterial != null)
                {
                    m_waterMaterial.SetFloat("_WaveSpeed", m_windZone.windMain + m_windZone.windTurbulence - 1.2f);
                }
            }
        }
    }
}