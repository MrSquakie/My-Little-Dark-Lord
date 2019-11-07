// Copyright © 2018 Procedural Worlds Pty Limited.  All Rights Reserved.
using System;
using UnityEngine;

namespace GeNa.Internal
{
    /// <summary>
    /// Serialisable & Simplified version of the builtin struct: Contains information about a tree placed in the Terrain game object.
    /// </summary>
    [System.Serializable]
    public struct STreeInstance : IEquatable<STreeInstance>, IEquatable<TreeInstance>
    {
        //
        // Summary:
        //     Position of the tree.
        public Vector3 position;
        ////
        //// Summary:
        ////     Width scale of this instance (compared to the prototype's size).
        //public float widthScale;
        ////
        //// Summary:
        ////     Height scale of this instance (compared to the prototype's size).
        //public float heightScale;
        //
        // Summary:
        //     Read-only. Rotation of the tree on X-Z plane (in radians).
        public float rotation;
        ////
        //// Summary:
        ////     Color of this instance.
        //public Color32 color;
        ////
        //// Summary:
        ////     Lightmap color calculated for this instance.
        //public Color32 lightmapColor;
        //
        // Summary:
        //     Index of this instance in the TerrainData.treePrototypes array.
        public int prototypeIndex;

        public STreeInstance(TreeInstance instance)
        {
            position = instance.position;
            rotation = instance.rotation;
            prototypeIndex = instance.prototypeIndex;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, [{2}])", position, rotation, prototypeIndex);
        }

        public static implicit operator STreeInstance(TreeInstance treeInstance)
        {
            return new STreeInstance(treeInstance);
        }

        /// <summary>
        /// Compare two instances.
        /// </summary>
        public bool Equals(STreeInstance other)
        {
            return position == other.position &&
                Mathf.Abs(rotation - other.rotation) < 1E-3f &&
                prototypeIndex == other.prototypeIndex;
        }

        /// <summary>
        /// Compare to the builtin (but not serialisable) tree insance.
        /// </summary>
        public bool Equals(TreeInstance other)
        {
            return CloseEnough(position, other.position) && Mathf.Abs(rotation - other.rotation) < 1E-3f &&
                prototypeIndex == other.prototypeIndex;
        }

        /// <summary>
        /// Compares to positions and determines if they are close enough to be referring to the same tree.
        /// </summary>
        private static bool CloseEnough(Vector3 a, Vector3 b)
        {
            return Mathf.Abs(a.x - b.x) < 1E-2f &&
                Mathf.Abs(a.y - b.y) < 1E-2f &&
                Mathf.Abs(a.z - b.z) < 1E-2f;
        }
    }
}
