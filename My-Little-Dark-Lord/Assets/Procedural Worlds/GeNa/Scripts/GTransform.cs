using UnityEngine;
using System.Collections.Generic;
using PWCommon2;
using System;

namespace GeNa
{
    /// <summary>
    /// Simple container that contains position, scale, and rotation.
    /// </summary>
    public struct GTransform
    {
        public Vector3 Postition;
        public Vector3 Rotation;
        public Vector3 Scale;

        public GTransform(Vector3 pos, Vector3 rot, Vector3 scale)
        {
            Postition = pos;
            Rotation = rot;
            Scale = scale;
        }
    }
}

