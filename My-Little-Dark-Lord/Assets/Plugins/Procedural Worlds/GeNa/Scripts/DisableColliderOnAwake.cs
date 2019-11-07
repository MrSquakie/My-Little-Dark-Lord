using UnityEngine;

namespace GeNa
{
    /// <summary>
    /// A class that will disable an objects colliders on awake
    /// </summary>
    public class DisableColliderOnAwake : MonoBehaviour
    {
        // Kill the collider
        void Awake()
        {
            SphereCollider sc = GetComponent<SphereCollider>();
            if (sc != null)
            {
                sc.enabled = false;
            }
        }
    }
}
