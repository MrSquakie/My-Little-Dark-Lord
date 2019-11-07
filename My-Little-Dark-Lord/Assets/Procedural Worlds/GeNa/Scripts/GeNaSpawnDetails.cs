using System;
using UnityEngine;
using System.Collections.Generic;
using PWCommon2;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GeNa
{
    /// <summary>
    /// Resources are the things that can be spawned
    /// </summary>
    [System.Serializable]
    public struct SpawnDetails
    {
        #region Transform data
        /// <summary>
        /// Parent of an item. - Null if root object
        /// </summary>
        public Transform Parent;

        public Vector3 Position;
        public Vector3 LocalPosition;
        public Vector3 Rotation;
        public Vector3 LocalRotation;
        public Vector3 Scale;
        public Vector3 LocalScale;

        #endregion

        public SpawnDetails(GameObject parent, Vector3 position, Vector3 localPosition,
            Vector3 rotation, Vector3 localRotation, Vector3 scale, Vector3 localScale)
        {
            if (parent != null)
            {
                Parent = parent.transform;
            }
            else
            {
                Parent = null;
            }

            Position = position;
            LocalPosition = localPosition;
            Rotation = rotation;
            LocalRotation = localRotation;
            Scale = scale;
            LocalScale = localScale;        
        }


        public override string ToString()
        {
            return string.Format("[{0} p:{1} lp:{2} r:{3} lr:{4} s:{5} ls:{6}]", Parent, Position, 
                LocalPosition, Rotation, LocalRotation, Scale, LocalScale);
        }
    }
}
