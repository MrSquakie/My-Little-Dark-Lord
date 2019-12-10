using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Gaia
{


    public enum CollisionMaskType { TerrainTree, Tag } //Layer }
    [System.Serializable]
    public class CollisionMask
    {
        public bool m_active = true;
        public bool m_invert = false;
        public CollisionMaskType m_type;
        public int m_treePrototypeId= -99;
        public string m_tag;
        public float m_Radius;
        public string m_layer;

        public void Bake()
        {
            if (m_active)
            {
                switch (m_type)
                {
                    case CollisionMaskType.TerrainTree:
                        GaiaSessionManager.GetSessionManager().m_collisionMaskCache.BakeAllTreeCollisions(m_treePrototypeId, m_Radius);
                        break;
                    case CollisionMaskType.Tag:
                        GaiaSessionManager.GetSessionManager().m_collisionMaskCache.BakeAllTagCollisions(m_tag,m_Radius);
                        break;
                }
            }
        }

    }
}
