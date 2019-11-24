using System.Collections.Generic;
using UnityEngine;

namespace Gaia
{
    public class FollowPlayerSystem : MonoBehaviour
    {
        [Header("Global Settings")]
        public bool m_followPlayer = true;
        public List<GameObject> m_particleObjects = new List<GameObject>();
        public Transform m_player;

        public bool m_useOffset = false;
        public float m_offset = 2000f;

        private void Update()
        {
            if (m_followPlayer)
            {
                if (m_player != null)
                {
                    if (m_particleObjects != null)
                    {
                        foreach (GameObject particleObject in m_particleObjects)
                        {
                            if (!m_useOffset)
                            {
                                particleObject.transform.position = m_player.position;
                            }
                            else
                            {
                                particleObject.transform.position = new Vector3(m_player.position.x + m_offset, m_player.position.y - 250f, m_player.position.z);
                            }
                        }
                    }
                }
            }
        }
    }
}